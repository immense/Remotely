using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Remotely.Server.Services;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Server.Hubs
{
    [Authorize]
    public class BrowserHub : Hub
    {
        public BrowserHub(
            IDataService dataService,
            SignInManager<RemotelyUser> signInManager,
            IHubContext<AgentHub> agentHubContext,
            IApplicationConfig appConfig)
        {
            SignInManager = signInManager;
            DataService = dataService;
            AgentHubContext = agentHubContext;
            AppConfig = appConfig;
        }

        public static ConcurrentDictionary<string, RemotelyUser> ConnectionIdToUserLookup { get; } = new ConcurrentDictionary<string, RemotelyUser>();
        private IApplicationConfig AppConfig { get; }
        private IDataService DataService { get; }
        private IHubContext<AgentHub> AgentHubContext { get; }
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

        public Task Chat(string message, string[] deviceIDs)
        {
            deviceIDs = DataService.FilterDeviceIDsByUserPermission(deviceIDs, RemotelyUser);
            var connections = GetActiveClientConnections(deviceIDs);
            var organizationName = DataService.GetOrganizationName(RemotelyUser.UserName);
            return AgentHubContext.Clients.Clients(connections.Select(x => x.Key).ToList()).SendAsync("Chat",
                RemotelyUser.DisplayName ?? RemotelyUser.UserName,
                message,
                organizationName,
                false,
                Context.ConnectionId);
        }

        public Task DeployScript(string fileID, string mode, string[] deviceIDs)
        {
            deviceIDs = DataService.FilterDeviceIDsByUserPermission(deviceIDs, RemotelyUser);
            var connections = GetActiveClientConnections(deviceIDs);
            var commandResult = new CommandResult()
            {
                CommandMode = mode,
                CommandText = Encoding.UTF8.GetString(DataService.GetSharedFiled(fileID).FileContents),
                SenderConnectionID = Context.ConnectionId,
                SenderUserID = Context.UserIdentifier,
                TargetDeviceIDs = connections.Select(x => x.Value.ID).ToArray(),
                OrganizationID = RemotelyUser.OrganizationID
            };
            DataService.AddOrUpdateCommandResult(commandResult);
            Clients.Caller.SendAsync("CommandResultCreated", commandResult);
            foreach (var connection in connections)
            {
                AgentHubContext.Clients.Client(connection.Key).SendAsync("DeployScript", mode, fileID, commandResult.ID, Context.ConnectionId);
            }
            return Task.CompletedTask;
        }

        public Task DownloadFile(string filePath, string deviceID)
        {
            if (DataService.DoesUserHaveAccessToDevice(deviceID, RemotelyUser))
            {
                var targetDevice = AgentHub.ServiceConnections.FirstOrDefault(x => x.Value.ID == deviceID);
                AgentHubContext.Clients.Client(targetDevice.Key).SendAsync("DownloadFile", filePath, Context.ConnectionId);
            }
            return Task.CompletedTask;
        }

        public Task ExecuteCommandOnClient(string mode, string command, string[] deviceIDs)
        {
            deviceIDs = DataService.FilterDeviceIDsByUserPermission(deviceIDs, RemotelyUser);
            var connections = GetActiveClientConnections(deviceIDs);

            var commandResult = new CommandResult()
            {
                CommandMode = mode,
                CommandText = command,
                SenderConnectionID = Context.ConnectionId,
                SenderUserID = Context.UserIdentifier,
                TargetDeviceIDs = connections.Select(x => x.Value.ID).ToArray(),
                OrganizationID = RemotelyUser.OrganizationID
            };
            DataService.AddOrUpdateCommandResult(commandResult);
            Clients.Caller.SendAsync("CommandResultCreated", commandResult);
            foreach (var connection in connections)
            {
                AgentHubContext.Clients.Client(connection.Key).SendAsync("ExecuteCommand", mode, command, commandResult.ID, Context.ConnectionId);
            }

            return Task.CompletedTask;
        }

        public override async Task OnConnectedAsync()
        {
            RemotelyUser = DataService.GetUserByID(Context.UserIdentifier);
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

        public Task RemoteControl(string deviceID)
        {
            var targetDevice = AgentHub.ServiceConnections.FirstOrDefault(x => x.Value.ID == deviceID);
            if (targetDevice.Value is null)
            {
                return Clients.Caller.SendAsync("DisplayMessage", $"The selected device is not online.", "Device is not online."); ;
            }
            if (DataService.DoesUserHaveAccessToDevice(deviceID, RemotelyUser))
            {
                var currentUsers = CasterHub.SessionInfoList.Count(x => x.Value.OrganizationID == RemotelyUser.OrganizationID);
                if (currentUsers >= AppConfig.RemoteControlSessionLimit)
                {
                    return Clients.Caller.SendAsync("DisplayMessage", $"There are already the maximum amount of active remote control sessions for your organization.", "Max number of concurrent sessions reached.");
                }
                Clients.Caller.SendAsync("ServiceID", targetDevice.Key);
                return AgentHubContext.Clients.Client(targetDevice.Key).SendAsync("RemoteControl", Context.ConnectionId, targetDevice.Key);
            }
            else
            {
                DataService.WriteEvent($"Remote control attempted by unauthorized user.  Device ID: {deviceID}.  User Name: {RemotelyUser.UserName}.", EventType.Warning, targetDevice.Value.OrganizationID);
            }
            return Task.CompletedTask;
        }

        public Task RemoveDevices(string[] deviceIDs)
        {
            var filterDevices = DataService.FilterDeviceIDsByUserPermission(deviceIDs, RemotelyUser);
            DataService.RemoveDevices(filterDevices);
            return Clients.Caller.SendAsync("RefreshDeviceList");
        }


        public Task UploadFiles(List<string> fileIDs, string transferID, string[] deviceIDs)
        {
            DataService.WriteEvent(new EventLog()
            {
                EventType = EventType.Info,
                Message = $"File transfer started by {RemotelyUser.UserName}.  File transfer IDs: {string.Join(", ", fileIDs)}.",
                TimeStamp = DateTimeOffset.Now,
                OrganizationID = RemotelyUser.OrganizationID
            });
            deviceIDs = DataService.FilterDeviceIDsByUserPermission(deviceIDs, RemotelyUser);
            var connections = GetActiveClientConnections(deviceIDs);
            foreach (var connection in connections)
            {
                AgentHubContext.Clients.Client(connection.Key).SendAsync("UploadFiles", transferID, fileIDs, Context.ConnectionId);
            }
            return Task.CompletedTask;
        }
        public Task UninstallAgents(string[] deviceIDs)
        {
            deviceIDs = DataService.FilterDeviceIDsByUserPermission(deviceIDs, RemotelyUser);
            var connections = GetActiveClientConnections(deviceIDs);
            foreach (var connection in connections)
            {
                AgentHubContext.Clients.Client(connection.Key).SendAsync("UninstallAgent");
            }
            DataService.RemoveDevices(deviceIDs);
            return Clients.Caller.SendAsync("RefreshDeviceList");
        }
        public Task UpdateTags(string deviceID, string tags)
        {
            if (DataService.DoesUserHaveAccessToDevice(deviceID, RemotelyUser))
            {
                if (tags.Length > 200)
                {
                    return Clients.Caller.SendAsync("DisplayMessage", $"Tag must be 200 characters or less. Supplied length is {tags.Length}.", "Tag must be under 200 characters.");
                }
                DataService.UpdateTags(deviceID, tags);
                return Clients.Caller.SendAsync("DisplayMessage", "Device updated successfully.", "Device updated.");
            }
            return Task.CompletedTask;
        }

        private IEnumerable<KeyValuePair<string, Device>> GetActiveClientConnections(string[] deviceIDs)
        {
            return AgentHub.ServiceConnections.Where(x =>
                x.Value.OrganizationID == RemotelyUser.OrganizationID &&
                deviceIDs.Contains(x.Value.ID)
            );
        }
        private async Task<bool> IsConnectionValid()
        {
            if (Context?.User?.Identity?.IsAuthenticated != true ||
                await SignInManager.UserManager.IsLockedOutAsync(RemotelyUser))
            {
                _ = Clients.Caller.SendAsync("LockedOut");
                Context.Abort();
                return false;
            }
            return true;
        }
    }
}
