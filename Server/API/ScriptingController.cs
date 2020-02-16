using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remotely.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using System.IO;
using Remotely.Server.Auth;

namespace Remotely.Server.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScriptingController : ControllerBase
    {
        public ScriptingController(DataService dataService, 
            UserManager<RemotelyUser> userManager,
            IHubContext<DeviceSocketHub> deviceHub)
        {
            DataService = dataService;
            UserManager = userManager;
            DeviceHub = deviceHub;
        }

        private DataService DataService { get; }
        private IHubContext<DeviceSocketHub> DeviceHub { get; }
        private UserManager<RemotelyUser> UserManager { get; }

        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        [HttpPost("[action]/{mode}/{deviceID}")]
        public async Task<ActionResult<CommandContext>> ExecuteCommand(string mode, string deviceID)
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

            KeyValuePair<string, Device> connection = DeviceSocketHub.ServiceConnections.FirstOrDefault(x =>
                x.Value.OrganizationID == orgID &&
                x.Value.ID == deviceID);

            if (string.IsNullOrWhiteSpace(connection.Key))
            {
                return NotFound();
            }

            var commandContext = new CommandContext()
            {
                CommandMode = "PSCore",
                CommandText = command,
                SenderConnectionID = string.Empty,
                SenderUserID = userID,
                TargetDeviceIDs = new string[] { deviceID },
                OrganizationID = orgID
            };
            DataService.AddOrUpdateCommandContext(commandContext);
            var requestID = Guid.NewGuid().ToString();
            await DeviceHub.Clients.Client(connection.Key).SendAsync("ExecuteCommandFromApi", mode, requestID, command, commandContext.ID, Guid.NewGuid().ToString());
            var success = await TaskHelper.DelayUntil(() => DeviceSocketHub.ApiScriptResults.TryGetValue(requestID, out _), TimeSpan.FromSeconds(30));
            if (!success)
            {
                return commandContext;
            }
            DeviceSocketHub.ApiScriptResults.TryGetValue(requestID, out var commandID);
            DeviceSocketHub.ApiScriptResults.Remove(requestID);
            DataService.DetachEntity(commandContext);
            var result = DataService.GetCommandContext(commandID.ToString(), orgID);
            return result;
        }
    }
}
