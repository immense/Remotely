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

namespace Remotely.Server.Services
{
    public class DeviceSocketHub : Hub
    {
        public DeviceSocketHub(DataService dataService, IHubContext<BrowserSocketHub> browserHub, IHubContext<RCBrowserSocketHub> rcBrowserHub)
        {
            DataService = dataService;
            BrowserHub = browserHub;
            RCBrowserHub = rcBrowserHub;
        }

		public static ConcurrentDictionary<string, Device> ServiceConnections { get; } = new ConcurrentDictionary<string, Device>();
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

		public async void BashResultViaAjax(string commandID)
		{
			var commandContext = DataService.GetCommandContext(commandID);
			await BrowserHub.Clients.Client(commandContext.SenderConnectionID).SendAsync("BashResultViaAjax", commandID, Device.ID);
		}

		public async void CMDResultViaAjax(string commandID)
		{
			var commandContext = DataService.GetCommandContext(commandID);
			await BrowserHub.Clients.Client(commandContext.SenderConnectionID).SendAsync("CMDResultViaAjax", commandID, Device.ID);
		}

		public async Task CommandResult(GenericCommandResult result)
		{
			result.DeviceID = Device.ID;
			var commandContext = DataService.GetCommandContext(result.CommandContextID);
			commandContext.CommandResults.Add(result);
			DataService.AddOrUpdateCommandContext(commandContext);
			await BrowserHub.Clients.Client(commandContext.SenderConnectionID).SendAsync("CommandResult", result);
		}

        public async Task DeviceCameOnline(Device device)
        {
            try
            {
                if (ServiceConnections.Any(x => x.Value.ID == device.ID))
                {
                    DataService.WriteEvent(new EventLog()
                    {
                        EventType = EventTypes.Info,
                        OrganizationID = device.OrganizationID,
                        Message = $"Device connection for {device?.DeviceName} was denied because it is already connected."
                    });
                    Context.Abort();
                    return;
                }
                
                if (DataService.AddOrUpdateDevice(device, out var updatedDevice))
                {
                    Device = updatedDevice;
                    ServiceConnections.AddOrUpdate(Context.ConnectionId, Device, (id, d) => Device);

                    var onlineOrganizationUsers = BrowserSocketHub.ConnectionIdToUserLookup
                                                    .Where(x => x.Value.OrganizationID == Device.OrganizationID);

                    var authorizedUsers = DataService.GetUsersWithAccessToDevice(onlineOrganizationUsers.Select(x => x.Value.Id), Device);
                    var connectionIds = onlineOrganizationUsers
                                            .Where(onlineUser => authorizedUsers.Any(authorizedUser => authorizedUser.Id == onlineUser.Value.Id))
                                            .Select(x => x.Key)
                                            .ToList();

                    await BrowserHub.Clients.Clients(connectionIds).SendAsync("DeviceCameOnline", Device);
                }
                else
                {
                    // Organization wasn't found.
                    await Clients.Caller.SendAsync("UninstallClient");
                }
            }
            catch (Exception ex)
            {
                DataService.WriteEvent(ex);
                throw;
            }
        }

        public async Task DeviceHeartbeat(Device device)
        {
            DataService.AddOrUpdateDevice(device, out var updatedDevice);
            Device = updatedDevice;
            var onlineOrganizationUsers = BrowserSocketHub.ConnectionIdToUserLookup
                                            .Where(x => x.Value.OrganizationID == Device.OrganizationID);

            var authorizedUsers = DataService.GetUsersWithAccessToDevice(onlineOrganizationUsers.Select(x=>x.Value.Id), Device);
            var connectionIds = onlineOrganizationUsers
                                    .Where(onlineUser => authorizedUsers.Any(authorizedUser => authorizedUser.Id == onlineUser.Value.Id))
                                    .Select(x => x.Key)
                                    .ToList();

            await BrowserHub.Clients.Clients(connectionIds).SendAsync("DeviceHeartbeat", Device);
        }

        public async Task DisplayConsoleMessage(string message, string requesterID)
        {
			await BrowserHub.Clients.Client(requesterID).SendAsync("DisplayConsoleMessage", message);
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

                var onlineOrganizationUsers = BrowserSocketHub.ConnectionIdToUserLookup
                                                   .Where(x => x.Value.OrganizationID == Device.OrganizationID);

                var authorizedUsers = DataService.GetUsersWithAccessToDevice(onlineOrganizationUsers.Select(x => x.Value.Id), Device);
                var connectionIds = onlineOrganizationUsers
                                        .Where(onlineUser => authorizedUsers.Any(authorizedUser => authorizedUser.Id == onlineUser.Value.Id))
                                        .Select(x => x.Key)
                                        .ToList();

                await BrowserHub.Clients.Clients(connectionIds).SendAsync("DeviceWentOffline", Device);

                ServiceConnections.Remove(Context.ConnectionId, out _);
            }
            
            await base.OnDisconnectedAsync(exception);
        }
        public async Task PSCoreResult(PSCoreCommandResult result)
        {
            result.DeviceID = Device.ID;
            var commandContext = DataService.GetCommandContext(result.CommandContextID);
            commandContext.PSCoreResults.Add(result);
            DataService.AddOrUpdateCommandContext(commandContext);
            await BrowserHub.Clients.Client(commandContext.SenderConnectionID).SendAsync("PSCoreResult", result);
        }
		public async void PSCoreResultViaAjax(string commandID)
		{
			var commandContext = DataService.GetCommandContext(commandID);
			await BrowserHub.Clients.Client(commandContext.SenderConnectionID).SendAsync("PSCoreResultViaAjax", commandID, Device.ID);
		}

        public async Task SendConnectionFailedToViewers(List<string> viewerIDs)
        {
            await RCBrowserHub.Clients.Clients(viewerIDs).SendAsync("ConnectionFailed");
        }

        public async Task SendServerVerificationToken()
        {
            await Clients.Caller.SendAsync("ServerVerificationToken", Device.ServerVerificationToken);
        }
        public void SetServerVerificationToken(string verificationToken)
        {
            Device.ServerVerificationToken = verificationToken;
            DataService.SetServerVerificationToken(Device.ID, verificationToken);
        }

        public async Task TransferCompleted(string transferID, string requesterID)
        {
            await BrowserHub.Clients.Client(requesterID).SendAsync("TransferCompleted", transferID);
        }
        public async Task WinPSResultViaAjax(string commandID)
        {
            var commandContext = DataService.GetCommandContext(commandID);
            await BrowserHub.Clients.Client(commandContext.SenderConnectionID).SendAsync("WinPSResultViaAjax", commandID, Device.ID);
        }
    }
}
