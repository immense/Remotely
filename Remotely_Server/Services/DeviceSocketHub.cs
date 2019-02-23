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
        
        private DataService DataService { get; }
        private IHubContext<BrowserSocketHub> BrowserHub { get; }
        
        public static ConcurrentDictionary<string, Machine> ServiceConnections { get; set; } = new ConcurrentDictionary<string, Machine>();

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Machine != null)
            {
                DataService.MachineDisconnected(Machine.ID);
                await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, Machine.OrganizationID);
                Machine.IsOnline = false;
                await BrowserHub.Clients.Group(Machine.OrganizationID).SendAsync("MachineWentOffline", Machine);
                while (!ServiceConnections.TryRemove(Context.ConnectionId, out var machine))
                {
                    await Task.Delay(1000);
                }
            }
            
            await base.OnDisconnectedAsync(exception);
        }
        public async Task MachineCameOnline(Machine machine)
        {
            if (ServiceConnections.Any(x=>x.Value.ID == machine.ID))
            {
                DataService.WriteEvent(new EventLog()
                {
                    EventType = EventTypes.Info,
                    OrganizationID = Machine.OrganizationID,
                    Message = $"Machine connection for {machine.MachineName} was denied because it is already connected."
                });
                Context.Abort();
                return;
            }
            machine.IsOnline = true;
            machine.LastOnline = DateTime.Now;
            Machine = machine;
            if (DataService.AddOrUpdateMachine(machine))
            {
                var failCount = 0;
                while (!ServiceConnections.TryAdd(Context.ConnectionId, machine))
                {
                    if (failCount > 3)
                    {
                        Context.Abort();
                        return;
                    }
                    failCount++;
                    await Task.Delay(1000);
                }
                await this.Groups.AddToGroupAsync(this.Context.ConnectionId, machine.OrganizationID);
                await BrowserHub.Clients.Group(Machine.OrganizationID).SendAsync("MachineCameOnline", Machine);
            }
            else
            {
                // Organization wasn't found.
                await Clients.Caller.SendAsync("UninstallClient");
            }
        }
        
        public async Task MachineHeartbeat(Machine machine)
        {
            machine.IsOnline = true;
            machine.LastOnline = DateTime.Now;
            Machine = machine;
            DataService.AddOrUpdateMachine(machine);
            await BrowserHub.Clients.Group(Machine.OrganizationID).SendAsync("MachineHeartbeat", Machine);
        }
        public async Task PSCoreResult(PSCoreCommandResult result)
        {
            result.MachineID = Machine.ID;
            var commandContext = DataService.GetCommandContext(result.CommandContextID);
            commandContext.PSCoreResults.Add(result);
            DataService.AddOrUpdateCommandContext(commandContext);
            await BrowserHub.Clients.Client(commandContext.SenderConnectionID).SendAsync("PSCoreResult", result);
        }
        public async Task CommandResult(GenericCommandResult result)
        {
            result.MachineID = Machine.ID;
            var commandContext = DataService.GetCommandContext(result.CommandContextID);
            commandContext.CommandResults.Add(result);
            DataService.AddOrUpdateCommandContext(commandContext);
            await BrowserHub.Clients.Client(commandContext.SenderConnectionID).SendAsync("CommandResult", result);
        }
        public async Task DisplayConsoleMessage(string message, string requesterID)
        {
            await BrowserHub.Clients.Client(requesterID).SendAsync("DisplayConsoleMessage", message);
        }
       
        public async Task SendServerVerificationToken()
        {
            await Clients.Caller.SendAsync("ServerVerificationToken", Machine.ServerVerificationToken);
        }
        public void SetServerVerificationToken(string verificationToken)
        {
            Machine.ServerVerificationToken = verificationToken;
            DataService.SetServerVerificationToken(Machine.ID, verificationToken);
        }

        public async void TransferCompleted(string transferID, string requesterID)
        {
            await BrowserHub.Clients.Client(requesterID).SendAsync("TransferCompleted", transferID);
        }
        public async void PSCoreResultViaAjax(string commandID)
        {
            var commandContext = DataService.GetCommandContext(commandID);
            await BrowserHub.Clients.Client(commandContext.SenderConnectionID).SendAsync("PSCoreResultViaAjax", commandID, Machine.ID);
        }
        public async void CMDResultViaAjax(string commandID)
        {
            var commandContext = DataService.GetCommandContext(commandID);
            await BrowserHub.Clients.Client(commandContext.SenderConnectionID).SendAsync("CMDResultViaAjax", commandID, Machine.ID);
        }
        public async void WinPSResultViaAjax(string commandID)
        {
            var commandContext = DataService.GetCommandContext(commandID);
            await BrowserHub.Clients.Client(commandContext.SenderConnectionID).SendAsync("WinPSResultViaAjax", commandID, Machine.ID);
        }
        public async void BashResultViaAjax(string commandID)
        {
            var commandContext = DataService.GetCommandContext(commandID);
            await BrowserHub.Clients.Client(commandContext.SenderConnectionID).SendAsync("BashResultViaAjax", commandID, Machine.ID);
        }
        private Machine Machine
        {
            get
            {
                return this.Context.Items["Machine"] as Machine;
            }
            set
            {
                this.Context.Items["Machine"] = value;
            }
        }

      
    }
}
