using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.API
{
    [ApiController]
    public class ScriptingController : ControllerBase
    {
        //public Task ExecuteCommand(string mode, string command, string[] deviceIDs)
        //{
        //    deviceIDs = DataService.FilterDeviceIDsByUserPermission(deviceIDs, RemotelyUser);
        //    var connections = GetActiveClientConnections(deviceIDs);

        //    var commandContext = new CommandContext()
        //    {
        //        CommandMode = mode,
        //        CommandText = command,
        //        SenderConnectionID = Context.ConnectionId,
        //        SenderUserID = Context.UserIdentifier,
        //        TargetDeviceIDs = connections.Select(x => x.Value.ID).ToArray(),
        //        OrganizationID = RemotelyUser.OrganizationID
        //    };
        //    DataService.AddOrUpdateCommandContext(commandContext);
        //    Clients.Caller.SendAsync("CommandContextCreated", commandContext);
        //    foreach (var connection in connections)
        //    {
        //        DeviceHub.Clients.Client(connection.Key).SendAsync("ExecuteCommand", mode, command, commandContext.ID, Context.ConnectionId);
        //    }

        //    return Task.CompletedTask;
        //}
    }
}
