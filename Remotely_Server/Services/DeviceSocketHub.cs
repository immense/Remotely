using Remotely_Library.Models;
using Remotely_Server.Data;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Remotely_Server.Services
{
    public class DeviceSocketHub : Hub
    {
        public DeviceSocketHub(DataService dataService, IHubContext<BrowserSocketHub> browserHub)
        {
            DataService = dataService;
            BrowserHub = browserHub;
        }

		public static ConcurrentDictionary<string, Device> ServiceConnections { get; set; } = new ConcurrentDictionary<string, Device>();
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

		public async Task DisplayConsoleMessage(string message, string requesterID)
		{
			await BrowserHub.Clients.Client(requesterID).SendAsync("DisplayConsoleMessage", message);
		}

		public async Task DeviceCameOnline(Device device)
		{
			if (ServiceConnections.Any(x => x.Value.ID == device.ID))
			{
				DataService.WriteEvent(new EventLog()
				{
					EventType = EventTypes.Info,
					OrganizationID = Device.OrganizationID,
					Message = $"Device connection for {device.DeviceName} was denied because it is already connected."
				});
				Context.Abort();
				return;
			}
			device.IsOnline = true;
			device.LastOnline = DateTime.Now;
			Device = device;
			if (DataService.AddOrUpdateDevice(device))
			{
				var failCount = 0;
				while (!ServiceConnections.TryAdd(Context.ConnectionId, device))
				{
					if (failCount > 3)
					{
						Context.Abort();
						return;
					}
					failCount++;
					await Task.Delay(1000);
				}
				await this.Groups.AddToGroupAsync(this.Context.ConnectionId, device.OrganizationID);
				await BrowserHub.Clients.Group(Device.OrganizationID).SendAsync("DeviceCameOnline", Device);
			}
			else
			{
				// Organization wasn't found.
				await Clients.Caller.SendAsync("UninstallClient");
			}
		}

		public async Task DeviceHeartbeat(Device device)
		{
			device.IsOnline = true;
			device.LastOnline = DateTime.Now;
			Device = device;
			DataService.AddOrUpdateDevice(device);
			await BrowserHub.Clients.Group(Device.OrganizationID).SendAsync("DeviceHeartbeat", Device);
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
                await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, Device.OrganizationID);
                Device.IsOnline = false;
                await BrowserHub.Clients.Group(Device.OrganizationID).SendAsync("DeviceWentOffline", Device);
                while (!ServiceConnections.TryRemove(Context.ConnectionId, out var device))
                {
                    await Task.Delay(1000);
                }
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

		public async Task SendServerVerificationToken()
		{
            await Clients.Caller.SendAsync("ServerVerificationToken", Device.ServerVerificationToken);
        }
        public void SetServerVerificationToken(string verificationToken)
        {
            Device.ServerVerificationToken = verificationToken;
            DataService.SetServerVerificationToken(Device.ID, verificationToken);
        }

        public async void TransferCompleted(string transferID, string requesterID)
        {
            await BrowserHub.Clients.Client(requesterID).SendAsync("TransferCompleted", transferID);
        }
        public async void WinPSResultViaAjax(string commandID)
        {
            var commandContext = DataService.GetCommandContext(commandID);
            await BrowserHub.Clients.Client(commandContext.SenderConnectionID).SendAsync("WinPSResultViaAjax", commandID, Device.ID);
        }
    }
}
