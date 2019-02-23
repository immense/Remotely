using Remotely_Library.Models;
using Remotely_Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Remotely_Server.Services
{
    [Authorize]
    public class BrowserSocketHub : Hub
    {
        public BrowserSocketHub(
            DataService dataService, 
            SignInManager<RemotelyUser> signInManager, 
            IHubContext<DeviceSocketHub> socketHub,
            ApplicationConfig appConfig)
        {
            SignInManager = signInManager;
            DataService = dataService;
            DeviceHub = socketHub;
            AppConfig = appConfig;
        }

        private DataService DataService { get; }
        private IHubContext<DeviceSocketHub> DeviceHub { get; }
        private RemotelyUser RemotelyUser
        {
            get
            {
                return Context.Items["RemotelyUser"] as RemotelyUser;
            }
            set
            {
                Context.Items["RemotelyUser"] = value;
            }
        }
        private SignInManager<RemotelyUser> SignInManager { get; }
        private ApplicationConfig AppConfig { get; }


        public async Task AddGroup(string[] machineIDs, string groupName)
        {
            groupName = groupName.Trim();
            machineIDs = DataService.FilterMachineIDsByUserPermission(machineIDs, RemotelyUser);
            if (!DataService.DoesGroupExist(RemotelyUser.Id, groupName))
            {
                await Clients.Caller.SendAsync("DisplayConsoleMessage", "Permission group does not exist.");
                return;
            }
            DataService.AddPermissionToMachines(RemotelyUser.Id, machineIDs, groupName);
            await Clients.Caller.SendAsync("DisplayConsoleMessage", "Group added.");
        }
        public async Task RemoveGroup(string[] machineIDs, string groupName)
        {
            groupName = groupName.Trim();
            machineIDs = DataService.FilterMachineIDsByUserPermission(machineIDs, RemotelyUser);
            if (!DataService.DoesGroupExist(RemotelyUser.Id, groupName))
            {
                await Clients.Caller.SendAsync("DisplayConsoleMessage", "Permission group does not exist.");
                return;
            }
            DataService.RemovePermissionFromMachines(RemotelyUser.Id, machineIDs, groupName);
            await Clients.Caller.SendAsync("DisplayConsoleMessage", "Group removed.");
        }
        public async Task DeployScript(string fileID, string mode, string[] machineIDs)
        {
            machineIDs = DataService.FilterMachineIDsByUserPermission(machineIDs, RemotelyUser);
            var connections = GetActiveClientConnections(machineIDs);
            var commandContext = new CommandContext()
            {
                CommandMode = mode,
                CommandText = Encoding.UTF8.GetString(DataService.GetSharedFiled(fileID).FileContents),
                SenderConnectionID = Context.ConnectionId,
                SenderUserID = Context.UserIdentifier,
                TargetMachineIDs = connections.Select(x => x.Value.ID).ToArray(),
                OrganizationID = RemotelyUser.OrganizationID
            };
            DataService.AddOrUpdateCommandContext(commandContext);
            await Clients.Caller.SendAsync("CommandContextCreated", commandContext);
            foreach (var connection in connections)
            {
                await DeviceHub.Clients.Client(connection.Key).SendAsync("DeployScript", mode, fileID, commandContext.ID, Context.ConnectionId);
            }
        }
        public async Task ExecuteCommandOnClient(string mode, string command, string[] machineIDs)
        {
            machineIDs = DataService.FilterMachineIDsByUserPermission(machineIDs, RemotelyUser);
            var connections = GetActiveClientConnections(machineIDs);

            var commandContext = new CommandContext()
            {
                CommandMode = mode,
                CommandText = command,
                SenderConnectionID = Context.ConnectionId,
                SenderUserID = Context.UserIdentifier,
                TargetMachineIDs = connections.Select(x => x.Value.ID).ToArray(),
                OrganizationID = RemotelyUser.Organization.ID
            };
            DataService.AddOrUpdateCommandContext(commandContext);
            await Clients.Caller.SendAsync("CommandContextCreated", commandContext);
            foreach (var connection in connections)
            {
                await DeviceHub.Clients.Client(connection.Key).SendAsync("ExecuteCommand", mode, command, commandContext.ID, Context.ConnectionId);
            }
        }
        public override async Task OnConnectedAsync()
        {
            RemotelyUser = DataService.GetUserByID(Context?.UserIdentifier);
            if (IsConnectionValid()?.Result == false)
            {
                return;
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, RemotelyUser.Organization.ID);
            await Clients.Caller.SendAsync("UserOptions", RemotelyUser.UserOptions);
            if (AppConfig.ShowMessageOfTheDay)
            {
                try
                {
                    var wc = new WebClient();
                    var message = await wc.DownloadStringTaskAsync(new Uri("https://remotely.lucency.co/api/messageoftheday"));
                    await Clients.Caller.SendAsync("DisplayConsoleHTML", message);
                }
                catch { }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, RemotelyUser.Organization.ID);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task RemoteControl(string machineID)
        {
            if (DataService.DoesUserHaveAccessToMachine(machineID, RemotelyUser))
            {
                var targetMachine = DeviceSocketHub.ServiceConnections.FirstOrDefault(x => x.Value.ID == machineID);
                var currentUsers = RCBrowserSocketHub.OrganizationConnectionList.Count(x => x.Value.OrganizationID == RemotelyUser.OrganizationID);
                if (currentUsers >= AppConfig.RemoteControlSessionLimit)
                {
                    await Clients.Caller.SendAsync("DisplayConsoleMessage", $"There are already the maximum amount of active remote control sessions for your organization.");
                    return;
                }
                await this.Clients.Caller.SendAsync("ServiceID", targetMachine.Key);
                await DeviceHub.Clients.Client(targetMachine.Key).SendAsync("RemoteControl", Context.ConnectionId, targetMachine.Key);
            }
        }

        public async Task RemoveMachines(string[] machineIDs)
        {
            var filterMachines = DataService.FilterMachineIDsByUserPermission(machineIDs, RemotelyUser);
            DataService.RemoveMachines(filterMachines);
            await Clients.Caller.SendAsync("RefreshMachineList");
        }
        public async Task TransferFiles(List<string> fileIDs, string transferID, string[] machineIDs)
        {
            DataService.WriteEvent(new EventLog()
            {
                EventType = EventTypes.Info,
                Message = $"File transfer started by {RemotelyUser.UserName}.  File transfer IDs: {string.Join(", ", fileIDs)}.",
                TimeStamp = DateTime.Now,
                OrganizationID = RemotelyUser.OrganizationID
            });
            machineIDs = DataService.FilterMachineIDsByUserPermission(machineIDs, RemotelyUser);
            var connections = GetActiveClientConnections(machineIDs);
            foreach (var connection in connections)
            {
                await DeviceHub.Clients.Client(connection.Key).SendAsync("TransferFiles", transferID, fileIDs, Context.ConnectionId);
            }
        }
        public async Task UninstallClients(string[] machineIDs)
        {
            machineIDs = DataService.FilterMachineIDsByUserPermission(machineIDs, RemotelyUser);
            var connections = GetActiveClientConnections(machineIDs);
            foreach (var connection in connections)
            {
                await DeviceHub.Clients.Client(connection.Key).SendAsync("UninstallClient");
            }
            DataService.RemoveMachines(machineIDs);
            await Clients.Caller.SendAsync("RefreshMachineList");
        }
        public async Task UpdateTags(string machineID, string tag)
        {
            if (DataService.DoesUserHaveAccessToMachine(machineID, RemotelyUser))
            {
                if (tag.Length > 200)
                {
                    await Clients.Caller.SendAsync("DisplayConsoleMessage", $"Tag must be 200 characters or less. Supplied length is {tag.Length}.");
                    return;
                }
                DataService.UpdateTags(machineID, tag);
                await Clients.Caller.SendAsync("DisplayConsoleMessage", "Tag updated successfully.");
            }
        }

        private IEnumerable<KeyValuePair<string, Machine>> GetActiveClientConnections(string[] machineIDs)
        {
            return DeviceSocketHub.ServiceConnections.Where(x =>
                x.Value.OrganizationID == RemotelyUser.Organization.ID &&
                machineIDs.Contains(x.Value.ID)
            );
        }
        private async Task<bool> IsConnectionValid()
        {
            if (Context?.User?.Identity?.IsAuthenticated != true || 
                await SignInManager.UserManager.IsLockedOutAsync(RemotelyUser))
            {
                await Clients.Caller.SendAsync("LockedOut");
                Context.Abort();
                return false;
            }
            return true;
        }
    }
}
