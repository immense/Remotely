using Remotely.Shared.Models;
using Remotely.Server.Data;
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

namespace Remotely.Server.Services
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

        public static ConcurrentDictionary<string, RemotelyUser> ConnectionIdToUserLookup { get; } = new ConcurrentDictionary<string, RemotelyUser>();
        private ApplicationConfig AppConfig { get; }
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
        public async Task AddGroup(string[] deviceIDs, string groupName)
        {
            groupName = groupName.Trim();
            deviceIDs = DataService.FilterDeviceIDsByUserPermission(deviceIDs, RemotelyUser);
            if (!DataService.DoesGroupExist(RemotelyUser.Id, groupName))
            {
                await Clients.Caller.SendAsync("DisplayConsoleMessage", "Permission group does not exist.");
                return;
            }
            DataService.AddPermissionToDevices(RemotelyUser.Id, deviceIDs, groupName);
            await Clients.Caller.SendAsync("DisplayConsoleMessage", "Group added.");
        }
        public async Task DeployScript(string fileID, string mode, string[] deviceIDs)
        {
            deviceIDs = DataService.FilterDeviceIDsByUserPermission(deviceIDs, RemotelyUser);
            var connections = GetActiveClientConnections(deviceIDs);
            var commandContext = new CommandContext()
            {
                CommandMode = mode,
                CommandText = Encoding.UTF8.GetString(DataService.GetSharedFiled(fileID).FileContents),
                SenderConnectionID = Context.ConnectionId,
                SenderUserID = Context.UserIdentifier,
                TargetDeviceIDs = connections.Select(x => x.Value.ID).ToArray(),
                OrganizationID = RemotelyUser.OrganizationID
            };
            DataService.AddOrUpdateCommandContext(commandContext);
            await Clients.Caller.SendAsync("CommandContextCreated", commandContext);
            foreach (var connection in connections)
            {
                await DeviceHub.Clients.Client(connection.Key).SendAsync("DeployScript", mode, fileID, commandContext.ID, Context.ConnectionId);
            }
        }

        public async Task ExecuteCommandOnClient(string mode, string command, string[] deviceIDs)
        {
            deviceIDs = DataService.FilterDeviceIDsByUserPermission(deviceIDs, RemotelyUser);
            var connections = GetActiveClientConnections(deviceIDs);

            var commandContext = new CommandContext()
            {
                CommandMode = mode,
                CommandText = command,
                SenderConnectionID = Context.ConnectionId,
                SenderUserID = Context.UserIdentifier,
                TargetDeviceIDs = connections.Select(x => x.Value.ID).ToArray(),
                OrganizationID = RemotelyUser.Organization.ID
            };
            DataService.AddOrUpdateCommandContext(commandContext);
            await Clients.Caller.SendAsync("CommandContextCreated", commandContext);
            foreach (var connection in connections)
            {
                await DeviceHub.Clients.Client(connection.Key).SendAsync("ExecuteCommand", mode, command, commandContext.ID, Context.ConnectionId);
            }
        }

        public async Task GetGroups(string[] deviceIDs)
        {
            deviceIDs = DataService.FilterDeviceIDsByUserPermission(deviceIDs, RemotelyUser);
            var result = DataService.GetDevicesAndPermissions(RemotelyUser.Id, deviceIDs);
            await Clients.Caller.SendAsync("GetGroupsResult", result);
        }
		public override async Task OnConnectedAsync()
		{
			RemotelyUser = DataService.GetUserAndPermissionsByID(Context.UserIdentifier);
			if (await IsConnectionValid() == false)
			{
				return;
			}
            ConnectionIdToUserLookup.AddOrUpdate(Context.ConnectionId, RemotelyUser, (id, ru) => RemotelyUser);

            await Clients.Caller.SendAsync("UserOptions", RemotelyUser.UserOptions);
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
            ConnectionIdToUserLookup.Remove(Context.ConnectionId, out _);
            await base.OnDisconnectedAsync(exception);
		}

		public async Task RemoteControl(string deviceID)
		{
			if (DataService.DoesUserHaveAccessToDevice(deviceID, RemotelyUser))
			{
				var targetDevice = DeviceSocketHub.ServiceConnections.FirstOrDefault(x => x.Value.ID == deviceID);
				var currentUsers = RCBrowserSocketHub.OrganizationConnectionList.Count(x => x.Value.OrganizationID == RemotelyUser.OrganizationID);
				if (currentUsers >= AppConfig.RemoteControlSessionLimit)
				{
					await Clients.Caller.SendAsync("DisplayConsoleMessage", $"There are already the maximum amount of active remote control sessions for your organization.");
					return;
				}
				await this.Clients.Caller.SendAsync("ServiceID", targetDevice.Key);
				await DeviceHub.Clients.Client(targetDevice.Key).SendAsync("RemoteControl", Context.ConnectionId, targetDevice.Key);
			}
		}

        public async Task RemoveDevices(string[] deviceIDs)
        {
            var filterDevices = DataService.FilterDeviceIDsByUserPermission(deviceIDs, RemotelyUser);
            DataService.RemoveDevices(filterDevices);
            await Clients.Caller.SendAsync("RefreshDeviceList");
        }

        public async Task RemoveGroup(string[] deviceIDs, string groupName)
        {
            groupName = groupName.Trim();
            deviceIDs = DataService.FilterDeviceIDsByUserPermission(deviceIDs, RemotelyUser);
            if (!DataService.DoesGroupExist(RemotelyUser.Id, groupName))
            {
                await Clients.Caller.SendAsync("DisplayConsoleMessage", "Permission group does not exist.");
                return;
            }
            DataService.RemovePermissionFromDevices(RemotelyUser.Id, deviceIDs, groupName);
            await Clients.Caller.SendAsync("DisplayConsoleMessage", "Group removed.");
        }
        public async Task TransferFiles(List<string> fileIDs, string transferID, string[] deviceIDs)
        {
            DataService.WriteEvent(new EventLog()
            {
                EventType = EventTypes.Info,
                Message = $"File transfer started by {RemotelyUser.UserName}.  File transfer IDs: {string.Join(", ", fileIDs)}.",
                TimeStamp = DateTime.Now,
                OrganizationID = RemotelyUser.OrganizationID
            });
            deviceIDs = DataService.FilterDeviceIDsByUserPermission(deviceIDs, RemotelyUser);
            var connections = GetActiveClientConnections(deviceIDs);
            foreach (var connection in connections)
            {
                await DeviceHub.Clients.Client(connection.Key).SendAsync("TransferFiles", transferID, fileIDs, Context.ConnectionId);
            }
        }
        public async Task UninstallClients(string[] deviceIDs)
        {
            deviceIDs = DataService.FilterDeviceIDsByUserPermission(deviceIDs, RemotelyUser);
            var connections = GetActiveClientConnections(deviceIDs);
            foreach (var connection in connections)
            {
                await DeviceHub.Clients.Client(connection.Key).SendAsync("UninstallClient");
            }
            DataService.RemoveDevices(deviceIDs);
            await Clients.Caller.SendAsync("RefreshDeviceList");
        }
        public async Task UpdateTags(string deviceID, string tag)
        {
            if (DataService.DoesUserHaveAccessToDevice(deviceID, RemotelyUser))
            {
                if (tag.Length > 200)
                {
                    await Clients.Caller.SendAsync("DisplayConsoleMessage", $"Tag must be 200 characters or less. Supplied length is {tag.Length}.");
                    return;
                }
                DataService.UpdateTags(deviceID, tag);
                await Clients.Caller.SendAsync("DisplayConsoleMessage", "Tag updated successfully.");
            }
        }

        private IEnumerable<KeyValuePair<string, Device>> GetActiveClientConnections(string[] deviceIDs)
        {
            return DeviceSocketHub.ServiceConnections.Where(x =>
                x.Value.OrganizationID == RemotelyUser.Organization.ID &&
                deviceIDs.Contains(x.Value.ID)
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
