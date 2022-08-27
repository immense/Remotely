using Immense.RemoteControl.Server.Abstractions;
using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol.Core.Types;
using Remotely.Server.Hubs;
using Remotely.Server.Models;
using Remotely.Shared.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Remotely.Server.Services.RcImplementations
{
    public class HubEventHandler : IHubEventHandler
    {
        private readonly IDataService _dataService;
        private readonly ICircuitManager _circuitManager;
        private readonly IHubContext<AgentHub> _serviceHub;

        public HubEventHandler(
            IDataService dataService,
            ICircuitManager circuitManager,
            IHubContext<AgentHub> serviceHub)
        {
            _dataService = dataService;
            _circuitManager = circuitManager;
            _serviceHub = serviceHub;
        }

        public Task ChangeWindowsSession(string serviceConnectionId, string viewerConnectionId, int targetWindowsSession)
        {
            return _serviceHub.Clients
                    .Client(serviceConnectionId)
                    .SendAsync("ChangeWindowsSession",
                        serviceConnectionId,
                        viewerConnectionId,
                        targetWindowsSession);
        }

        public void LogRemoteControlStarted(string message, string organizationId)
        {
            _dataService.WriteEvent(message, EventType.Info, organizationId);
        }

        public Task NotifyUnattendedSessionReady(string userConnectionId, string desktopConnectionId, string deviceId)
        {
            return _circuitManager.InvokeOnConnection(userConnectionId, CircuitEventName.UnattendedSessionReady, desktopConnectionId, deviceId);
        }

        public Task RestartScreenCaster(string desktopConnectionId, string serviceConnectionId, HashSet<string> viewerList)
        {
            return _serviceHub.Clients.Client(serviceConnectionId).SendAsync("RestartScreenCaster", viewerList, serviceConnectionId, desktopConnectionId);
        }
    }
}
