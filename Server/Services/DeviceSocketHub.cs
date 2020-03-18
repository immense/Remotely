using Remotely.Shared.Models;
using Remotely.Server.Data;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;

namespace Remotely.Server.Services
{
    public class DeviceSocketHub : Hub
    {
        public DeviceSocketHub(DataService dataService,
            IHubContext<BrowserSocketHub> browserHub,
            IHubContext<RCBrowserSocketHub> rcBrowserHub)
        {
            DataService = dataService;
            BrowserHub = browserHub;
            RCBrowserHub = rcBrowserHub;
        }

        public static ConcurrentDictionary<string, Device> ServiceConnections { get; } = new ConcurrentDictionary<string, Device>();
        public static IMemoryCache ApiScriptResults { get; } = new MemoryCache(new MemoryCacheOptions());
        public IHubContext<RCBrowserSocketHub> RCBrowserHub { get; }
        private IHubContext<BrowserSocketHub> BrowserHub { get; }
        private DataService DataService { get; }
		private Device Device
		{
			get
			{
				return this.Context.Items["Device"] as Device;
			}
			set
			{
				this.Context.Items["Device"] = value;
			}
		}

		public Task BashResultViaAjax(string commandID)
		{
			var commandResult = DataService.GetCommandResult(commandID);
			return BrowserHub.Clients.Client(commandResult.SenderConnectionID).SendAsync("BashResultViaAjax", commandID, Device.ID);
		}

        public Task Chat(string message, string senderConnectionID)
        {
            return BrowserHub.Clients.Client(senderConnectionID).SendAsync("Chat", Device.ID, Device.DeviceName, message);
        }

        public Task CMDResultViaAjax(string commandID)
		{
			var commandResult = DataService.GetCommandResult(commandID);
            return BrowserHub.Clients.Client(commandResult.SenderConnectionID).SendAsync("CMDResultViaAjax", commandID, Device.ID);
		}

        public Task CommandResult(GenericCommandResult result)
		{
			result.DeviceID = Device.ID;
			var commandResult = DataService.GetCommandResult(result.CommandResultID);
			commandResult.CommandResults.Add(result);
			DataService.AddOrUpdateCommandResult(commandResult);
            return BrowserHub.Clients.Client(commandResult.SenderConnectionID).SendAsync("CommandResult", result);
		}

        public void CommandResultViaApi(string commandID, string requestID)
        {
            ApiScriptResults.Set(requestID, commandID, DateTimeOffset.Now.AddHours(1));
        }

        public Task<bool> DeviceCameOnline(Device device)
        {
            try
            {
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
                
                if (DataService.AddOrUpdateDevice(device, out var updatedDevice))
                {
                    Device = updatedDevice;
                    ServiceConnections.AddOrUpdate(Context.ConnectionId, Device, (id, d) => Device);
                    
                    var userIDs = BrowserSocketHub.ConnectionIdToUserLookup.Values.Select(x => x.Id);

                    var filteredUserIDs = DataService.FilterUsersByDevicePermission(userIDs, Device.ID);

                    var connectionIds = BrowserSocketHub.ConnectionIdToUserLookup
                                                   .Where(x => x.Value.OrganizationID == Device.OrganizationID &&
                                                                filteredUserIDs.Contains(x.Value.Id))
                                                   .Select(x => x.Key)
                                                   .ToList();

                    BrowserHub.Clients.Clients(connectionIds).SendAsync("DeviceCameOnline", Device);
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
                DataService.WriteEvent(ex, Device?.OrganizationID);
            }

            Context.Abort();
            return Task.FromResult(false);
        }

        public Task DeviceHeartbeat(Device device)
        {
            DataService.AddOrUpdateDevice(device, out var updatedDevice);
            Device = updatedDevice;

            var userIDs = BrowserSocketHub.ConnectionIdToUserLookup.Values.Select(x => x.Id);

            var filteredUserIDs = DataService.FilterUsersByDevicePermission(userIDs, Device.ID);

            var connectionIds = BrowserSocketHub.ConnectionIdToUserLookup
                                           .Where(x => x.Value.OrganizationID == Device.OrganizationID &&
                                                        filteredUserIDs.Contains(x.Value.Id))
                                           .Select(x => x.Key)
                                           .ToList();

            return BrowserHub.Clients.Clients(connectionIds).SendAsync("DeviceHeartbeat", Device);
        }


        public void DeviceSetupOptions(DeviceSetupOptions options, string deviceID)
        {
            DataService.SetDeviceSetupOptions(deviceID, options);
        }

        public Task DisplayMessage(string consoleMessage, string popupMessage, string requesterID)
        {
            return BrowserHub.Clients.Client(requesterID).SendAsync("DisplayMessage", consoleMessage, popupMessage);
		}
        public Task DownloadFile(string fileID, string requesterID)
        {
            return BrowserHub.Clients.Client(requesterID).SendAsync("DownloadFile", fileID);
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

                var connectionIds = BrowserSocketHub.ConnectionIdToUserLookup
                                                   .Where(x => x.Value.OrganizationID == Device.OrganizationID)
                                                   .Select(x => x.Key)
                                                   .ToList();

                await BrowserHub.Clients.Clients(connectionIds).SendAsync("DeviceWentOffline", Device);

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
            return BrowserHub.Clients.Client(commandResult.SenderConnectionID).SendAsync("PSCoreResult", result);
        }
		public async void PSCoreResultViaAjax(string commandID)
		{
			var commandResult = DataService.GetCommandResult(commandID);
			await BrowserHub.Clients.Client(commandResult.SenderConnectionID).SendAsync("PSCoreResultViaAjax", commandID, Device.ID);
		}

        public Task SendConnectionFailedToViewers(List<string> viewerIDs)
        {
            return RCBrowserHub.Clients.Clients(viewerIDs).SendAsync("ConnectionFailed");
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
            return BrowserHub.Clients.Client(requesterID).SendAsync("TransferCompleted", transferID);
        }
        public Task WinPSResultViaAjax(string commandID)
        {
            var commandResult = DataService.GetCommandResult(commandID);
            return BrowserHub.Clients.Client(commandResult.SenderConnectionID).SendAsync("WinPSResultViaAjax", commandID, Device.ID);
        }
    }
}
