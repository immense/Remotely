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

        [Authorize]
        [HttpPost("[action]/{mode}/{deviceID}")]
        public async Task<ActionResult<CommandContext>> ExecuteCommand(string mode, string deviceID)
        {
            var command = string.Empty;
            using (var sr = new StreamReader(Request.Body))
            {
                command = await sr.ReadToEndAsync();
            }
            var username = Request.HttpContext.User.Identity.Name;
            var user = await UserManager.FindByNameAsync(username);
            if (!DataService.DoesUserHaveAccessToDevice(deviceID, user))
            {
                return Unauthorized();
            }


            KeyValuePair<string, Device> connection = DeviceSocketHub.ServiceConnections.FirstOrDefault(x =>
                x.Value.OrganizationID == user.OrganizationID &&
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
                SenderUserID = user.Id,
                TargetDeviceIDs = new string[] { deviceID },
                OrganizationID = user.OrganizationID
            };
            DataService.AddOrUpdateCommandContext(commandContext);
            var requestID = Guid.NewGuid().ToString();
            await DeviceHub.Clients.Client(connection.Key).SendAsync("ExecuteCommandFromApi", mode, requestID, command, commandContext.ID, username);
            var success = await TaskHelper.DelayUntil(() => DeviceSocketHub.ApiScriptResults.TryGetValue(requestID, out _), TimeSpan.FromSeconds(30));
            if (!success)
            {
                return commandContext;
            }
            DeviceSocketHub.ApiScriptResults.TryGetValue(requestID, out var commandID);
            DeviceSocketHub.ApiScriptResults.Remove(requestID);
            DataService.DetachEntity(commandContext);
            var result = DataService.GetCommandContext(commandID.ToString(), username);
            return result;
        }
    }
}
