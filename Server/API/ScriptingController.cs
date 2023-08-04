using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using Remotely.Shared.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;
using Remotely.Shared.Enums;
using Remotely.Server.Auth;
using Immense.RemoteControl.Shared.Helpers;
using Remotely.Shared;
using Remotely.Server.Extensions;
using Remotely.Shared.Entities;
using Remotely.Shared.Interfaces;

namespace Remotely.Server.API;

[ApiController]
[Route("api/[controller]")]
public class ScriptingController : ControllerBase
{
    private readonly IHubContext<AgentHub, IAgentHubClient> _agentHubContext;

    private readonly IDataService _dataService;
    private readonly IAgentHubSessionCache _serviceSessionCache;
    private readonly IExpiringTokenService _expiringTokenService;

    private readonly UserManager<RemotelyUser> _userManager;

    public ScriptingController(UserManager<RemotelyUser> userManager,
        IDataService dataService,
        IAgentHubSessionCache serviceSessionCache,
        IExpiringTokenService expiringTokenService,
        IHubContext<AgentHub, IAgentHubClient> agentHub)
    {
        _dataService = dataService;
        _serviceSessionCache = serviceSessionCache;
        _expiringTokenService = expiringTokenService;
        _userManager = userManager;
        _agentHubContext = agentHub;
    }

    [ServiceFilter(typeof(ApiAuthorizationFilter))]
    [HttpPost("[action]/{mode}/{deviceID}")]
    public async Task<ActionResult<ScriptResult>> ExecuteCommand(string mode, string deviceID)
    {
        if (!Request.Headers.TryGetOrganizationId(out var orgId))
        {
            return Unauthorized();
        }

        if (!Enum.TryParse<ScriptingShell>(mode, true, out var shell))
        {
            return BadRequest("Unable to parse shell type.  Use either PSCore, WinPS, Bash, or CMD.");
        }

        var command = string.Empty;
        using (var sr = new StreamReader(Request.Body))
        {
            command = await sr.ReadToEndAsync();
        }

        if (Request.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var username = Request.HttpContext.User.Identity.Name;
            var userResult = await _dataService.GetUserByName($"{username}");

            if (!userResult.IsSuccess)
            {
                return Unauthorized();
            }

            if (!_dataService.DoesUserHaveAccessToDevice(deviceID, userResult.Value))
            {
                return Unauthorized();
            }
        }

        if (!_serviceSessionCache.TryGetByDeviceId(deviceID, out var device))
        {
            return NotFound();
        }

        if (!_serviceSessionCache.TryGetConnectionId(deviceID, out var connectionId))
        {
            return NotFound();
        }

        if (device.OrganizationID != orgId)
        {
            return Unauthorized();
        }

        var requestID = Guid.NewGuid().ToString();
        var authToken = _expiringTokenService.GetToken(Time.Now.AddMinutes(AppConstants.ScriptRunExpirationMinutes));

        // TODO: Replace with new invoke capability in .NET 7.
        await _agentHubContext.Clients.Client(connectionId).ExecuteCommandFromApi(
            shell, 
            authToken, 
            requestID, 
            command,
            User?.Identity?.Name ?? "API Key");

        var success = await WaitHelper.WaitForAsync(() => AgentHub.ApiScriptResults.TryGetValue(requestID, out _), TimeSpan.FromSeconds(30));
        if (!success)
        {
            return NotFound();
        }
        AgentHub.ApiScriptResults.TryGetValue(requestID, out var commandId);
        AgentHub.ApiScriptResults.Remove(requestID);

        var scriptResult = await _dataService.GetScriptResult($"{commandId}", orgId);
        if (!scriptResult.IsSuccess)
        {
            return NotFound();
        }
        return scriptResult.Value;
    }
}
