using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using Remotely.Shared.Utilities;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Remotely.Shared.Enums;
using Remotely.Server.Auth;
using Immense.RemoteControl.Server.Abstractions;
using Immense.RemoteControl.Shared.Helpers;
using Remotely.Shared;

namespace Remotely.Server.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScriptingController : ControllerBase
    {
        private readonly IHubContext<ServiceHub> _agentHubContext;

        private readonly IDataService _dataService;
        private readonly IServiceHubSessionCache _serviceSessionCache;
        private readonly IExpiringTokenService _expiringTokenService;

        private readonly UserManager<RemotelyUser> _userManager;

        public ScriptingController(UserManager<RemotelyUser> userManager,
            IDataService dataService,
            IServiceHubSessionCache serviceSessionCache,
            IExpiringTokenService expiringTokenService,
            IHubContext<ServiceHub> agentHub)
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
            if (!Enum.TryParse<ScriptingShell>(mode, true, out var shell))
            {
                return BadRequest("Unable to parse shell type.  Use either PSCore, WinPS, Bash, or CMD.");
            }

            var command = string.Empty;
            using (var sr = new StreamReader(Request.Body))
            {
                command = await sr.ReadToEndAsync();
            }

            var userID = string.Empty;
            if (Request.HttpContext.User.Identity.IsAuthenticated)
            {
                var username = Request.HttpContext.User.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);
                userID = user.Id;
                if (!_dataService.DoesUserHaveAccessToDevice(deviceID, user))
                {
                    return Unauthorized();
                }

            }

            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            if (!_serviceSessionCache.TryGetByDeviceId(deviceID, out var device))
            {
                return NotFound();
            }

            if (!_serviceSessionCache.TryGetConnectionId(deviceID, out var connectionId))
            {
                return NotFound();
            }

            if (device.OrganizationID != orgID)
            {
                return Unauthorized();
            }

            var requestID = Guid.NewGuid().ToString();
            var authToken = _expiringTokenService.GetToken(Time.Now.AddMinutes(AppConstants.ScriptRunExpirationMinutes));

            await _agentHubContext.Clients.Client(connectionId).SendAsync("ExecuteCommandFromApi", shell, authToken, requestID, command, User?.Identity?.Name);

            var success = await WaitHelper.WaitForAsync(() => ServiceHub.ApiScriptResults.TryGetValue(requestID, out _), TimeSpan.FromSeconds(30));
            if (!success)
            {
                return NotFound();
            }
            ServiceHub.ApiScriptResults.TryGetValue(requestID, out var commandID);
            ServiceHub.ApiScriptResults.Remove(requestID);
            var result = _dataService.GetScriptResult(commandID.ToString(), orgID);
            return result;
        }
    }
}
