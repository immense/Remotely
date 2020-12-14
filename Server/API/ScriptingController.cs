using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Remotely.Server.Attributes;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using Remotely.Shared.Helpers;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScriptingController : ControllerBase
    {
        public ScriptingController(IDataService dataService,
            UserManager<RemotelyUser> userManager,
            IHubContext<AgentHub> agentHub)
        {
            DataService = dataService;
            UserManager = userManager;
            AgentHubContext = agentHub;
        }

        private IDataService DataService { get; }
        private IHubContext<AgentHub> AgentHubContext { get; }
        private UserManager<RemotelyUser> UserManager { get; }

        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        [HttpPost("[action]/{mode}/{deviceID}")]
        public async Task<ActionResult<CommandResult>> ExecuteCommand(string mode, string deviceID)
        {
            var command = string.Empty;
            using (var sr = new StreamReader(Request.Body))
            {
                command = await sr.ReadToEndAsync();
            }

            var userID = string.Empty;
            if (Request.HttpContext.User.Identity.IsAuthenticated)
            {
                var username = Request.HttpContext.User.Identity.Name;
                var user = await UserManager.FindByNameAsync(username);
                userID = user.Id;
                if (!DataService.DoesUserHaveAccessToDevice(deviceID, user))
                {
                    return Unauthorized();
                }

            }

            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            KeyValuePair<string, Device> connection = AgentHub.ServiceConnections.FirstOrDefault(x =>
                x.Value.OrganizationID == orgID &&
                x.Value.ID == deviceID);

            if (string.IsNullOrWhiteSpace(connection.Key))
            {
                return NotFound();
            }

            var commandResult = new CommandResult()
            {
                CommandMode = "PSCore",
                CommandText = command,
                SenderConnectionID = string.Empty,
                SenderUserID = userID,
                TargetDeviceIDs = new string[] { deviceID },
                OrganizationID = orgID
            };
            DataService.AddOrUpdateCommandResult(commandResult);
            var requestID = Guid.NewGuid().ToString();
            await AgentHubContext.Clients.Client(connection.Key).SendAsync("ExecuteCommandFromApi", mode, requestID, command, commandResult.ID, Guid.NewGuid().ToString());
            var success = await TaskHelper.DelayUntilAsync(() => AgentHub.ApiScriptResults.TryGetValue(requestID, out _), TimeSpan.FromSeconds(30));
            if (!success)
            {
                return commandResult;
            }
            AgentHub.ApiScriptResults.TryGetValue(requestID, out var commandID);
            AgentHub.ApiScriptResults.Remove(requestID);
            DataService.DetachEntity(commandResult);
            var result = DataService.GetCommandResult(commandID.ToString(), orgID);
            return result;
        }
    }
}
