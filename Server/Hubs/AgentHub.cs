using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Remotely.Server.Services;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Hubs
{
    public class AgentHub : Hub
    {
        public AgentHub(IDataService dataService,
            IApplicationConfig appConfig,
            IHubContext<BrowserHub> browserHubContext,
            IHubContext<ViewerHub> viewerHubContext)
        {
            DataService = dataService;
            BrowserHubContext = browserHubContext;
            ViewerHubContext = viewerHubContext;
            AppConfig = appConfig;
        }

        public static IMemoryCache ApiScriptResults { get; } = new MemoryCache(new MemoryCacheOptions());
        public static ConcurrentDictionary<string, Device> ServiceConnections { get; } = new ConcurrentDictionary<string, Device>();
        public IApplicationConfig AppConfig { get; }
        public IHubContext<ViewerHub> ViewerHubContext { get; }
        private IHubContext<BrowserHub> BrowserHubContext { get; }
        private IDataService DataService { get; }
        private Device Device
        {
            get
            {
                return Context.Items["Device"] as Device;
            }
            set
            {
                Context.Items["Device"] = value;
            }
        }

        public Task BashResultViaAjax(string commandID)
        {
            var commandResult = DataService.GetCommandResult(commandID);
            return BrowserHubContext.Clients.Client(commandResult.SenderConnectionID).SendAsync("BashResultViaAjax", commandID, Device.ID);
        }

        public Task Chat(string message, bool disconnected, string senderConnectionID)
        {
            if (BrowserHub.ConnectionIdToUserLookup.ContainsKey(senderConnectionID))
            {
                return BrowserHubContext.Clients.Client(senderConnectionID).SendAsync("Chat", Device.ID, Device.DeviceName, message, disconnected);
            }
            else
            {
                return Clients.Caller.SendAsync("Chat", string.Empty, string.Empty, string.Empty, true, senderConnectionID);
            }
        }

        public Task CMDResultViaAjax(string commandID)
        {
            var commandResult = DataService.GetCommandResult(commandID);
            return BrowserHubContext.Clients.Client(commandResult.SenderConnectionID).SendAsync("CMDResultViaAjax", commandID, Device.ID);
        }

        public Task CommandResult(GenericCommandResult result)
        {
            result.DeviceID = Device.ID;
            var commandResult = DataService.GetCommandResult(result.CommandResultID);
            commandResult.CommandResults.Add(result);
            DataService.AddOrUpdateCommandResult(commandResult);
            return BrowserHubContext.Clients.Client(commandResult.SenderConnectionID).SendAsync("CommandResult", result);
        }

        public void CommandResultViaApi(string commandID, string requestID)
        {
            ApiScriptResults.Set(requestID, commandID, DateTimeOffset.Now.AddHours(1));
        }

        public Task<bool> DeviceCameOnline(Device device)
        {
            try
            {
                if (CheckForDeviceBan(device.ID, device.DeviceName))
                {
                    return Task.FromResult(false);
                }

                if (ServiceConnections.Any(x => x.Value.ID == device.ID))
                {
                    DataService.WriteEvent(new EventLog()
                    {
                        EventType = EventType.Info,
                        OrganizationID = device.OrganizationID,
                        Message = $"Device connection for {device?.DeviceName} was denied because it is already connected."
                    });
                    return Task.FromResult(false);
                }

                var ip = Context.GetHttpContext()?.Connection?.RemoteIpAddress;
                if (ip != null && ip.IsIPv4MappedToIPv6)
                {
                    ip = ip.MapToIPv4();
                }
                device.PublicIP = ip?.ToString();

                if (CheckForDeviceBan(device.PublicIP))
                {
                    return Task.FromResult(false);
                }

                if (DataService.AddOrUpdateDevice(device, out var updatedDevice))
                {
                    Device = updatedDevice;
                    ServiceConnections.AddOrUpdate(Context.ConnectionId, Device, (id, d) => Device);

                    var userIDs = BrowserHub.ConnectionIdToUserLookup.Values.Select(x => x.Id);

                    var filteredUserIDs = DataService.FilterUsersByDevicePermission(userIDs, Device.ID);

                    var connectionIds = BrowserHub.ConnectionIdToUserLookup
                                                   .Where(x => x.Value.OrganizationID == Device.OrganizationID &&
                                                                filteredUserIDs.Contains(x.Value.Id))
                                                   .Select(x => x.Key)
                                                   .ToList();

                    BrowserHubContext.Clients.Clients(connectionIds).SendAsync("DeviceCameOnline", Device);
                    return Task.FromResult(true);
                }
                else
                {
                    // Organization wasn't found.
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                DataService.WriteEvent(ex, device?.OrganizationID);
            }

            Context.Abort();
            return Task.FromResult(false);
        }

        public Task DeviceHeartbeat(Device device)
        {
            if (CheckForDeviceBan(device.ID, device.DeviceName))
            {
                return Task.CompletedTask;
            }

            var ip = Context.GetHttpContext()?.Connection?.RemoteIpAddress;
            if (ip != null && ip.IsIPv4MappedToIPv6)
            {
                ip = ip.MapToIPv4();
            }
            device.PublicIP = ip?.ToString();

            if (CheckForDeviceBan(device.PublicIP))
            {
                return Task.CompletedTask;
            }


            DataService.AddOrUpdateDevice(device, out var updatedDevice);
            Device = updatedDevice;

            var userIDs = BrowserHub.ConnectionIdToUserLookup.Values.Select(x => x.Id);

            var filteredUserIDs = DataService.FilterUsersByDevicePermission(userIDs, Device.ID);

            var connectionIds = BrowserHub.ConnectionIdToUserLookup
                                           .Where(x => x.Value.OrganizationID == Device.OrganizationID &&
                                                        filteredUserIDs.Contains(x.Value.Id))
                                           .Select(x => x.Key)
                                           .ToList();

            return BrowserHubContext.Clients.Clients(connectionIds).SendAsync("DeviceHeartbeat", Device);
        }

        public Task DisplayMessage(string consoleMessage, string popupMessage, string requesterID)
        {
            return BrowserHubContext.Clients.Client(requesterID).SendAsync("DisplayMessage", consoleMessage, popupMessage);
        }

        public Task DownloadFile(string fileID, string requesterID)
        {
            return BrowserHubContext.Clients.Client(requesterID).SendAsync("DownloadFile", fileID);
        }

        public Task DownloadFileProgress(int progressPercent, string requesterID)
        {
            return BrowserHubContext.Clients.Client(requesterID).SendAsync("DownloadFileProgress", progressPercent);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Device != null)
            {
                DataService.DeviceDisconnected(Device.ID);

                Device.IsOnline = false;

                var connectionIds = BrowserHub.ConnectionIdToUserLookup
                                                   .Where(x => x.Value.OrganizationID == Device.OrganizationID)
                                                   .Select(x => x.Key)
                                                   .ToList();

                await BrowserHubContext.Clients.Clients(connectionIds).SendAsync("DeviceWentOffline", Device);

                ServiceConnections.Remove(Context.ConnectionId, out _);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public Task PSCoreResult(PSCoreCommandResult result)
        {
            result.DeviceID = Device.ID;
            var commandResult = DataService.GetCommandResult(result.CommandResultID);
            commandResult.PSCoreResults.Add(result);
            DataService.AddOrUpdateCommandResult(commandResult);
            return BrowserHubContext.Clients.Client(commandResult.SenderConnectionID).SendAsync("PSCoreResult", result);
        }

        public async void PSCoreResultViaAjax(string commandID)
        {
            var commandResult = DataService.GetCommandResult(commandID);
            await BrowserHubContext.Clients.Client(commandResult.SenderConnectionID).SendAsync("PSCoreResultViaAjax", commandID, Device.ID);
        }

        public Task SendConnectionFailedToViewers(List<string> viewerIDs)
        {
            return ViewerHubContext.Clients.Clients(viewerIDs).SendAsync("ConnectionFailed");
        }

        public Task SendServerVerificationToken()
        {
            return Clients.Caller.SendAsync("ServerVerificationToken", Device.ServerVerificationToken);
        }

        public void SetServerVerificationToken(string verificationToken)
        {
            Device.ServerVerificationToken = verificationToken;
            DataService.SetServerVerificationToken(Device.ID, verificationToken);
        }

        public Task TransferCompleted(string transferID, string requesterID)
        {
            return BrowserHubContext.Clients.Client(requesterID).SendAsync("TransferCompleted", transferID);
        }

        public Task WinPSResultViaAjax(string commandID)
        {
            var commandResult = DataService.GetCommandResult(commandID);
            return BrowserHubContext.Clients.Client(commandResult.SenderConnectionID).SendAsync("WinPSResultViaAjax", commandID, Device.ID);
        }

        private bool CheckForDeviceBan(params string[] deviceIdNameOrIPs)
        {
            foreach (var device in deviceIdNameOrIPs)
            {
                if (string.IsNullOrWhiteSpace(device))
                {
                    continue;
                }

                if (AppConfig.BannedDevices.Any(x => !string.IsNullOrWhiteSpace(x) &&
                    x.Equals(device, StringComparison.OrdinalIgnoreCase)))
                {
                    DataService.WriteEvent($"Device ID/name/IP ({device}) is banned.  Sending uninstall command.", null);

                    _ = Clients.Caller.SendAsync("UninstallAgent");
                    return true;
                }
            }
           
            return false;
        }
    }
}
