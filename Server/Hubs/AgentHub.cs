using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Remotely.Server.Models;
using Remotely.Server.Services;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Hubs
{
    public class AgentHub : Hub
    {
        private readonly IApplicationConfig _appConfig;
        private readonly ICircuitManager _circuitManager;
        private readonly IExpiringTokenService _expiringTokenService;
        private readonly IDataService _dataService;
        private readonly IHubContext<ViewerHub> _viewerHubContext;

        public AgentHub(IDataService dataService,
            IApplicationConfig appConfig,
            IHubContext<ViewerHub> viewerHubContext,
            ICircuitManager circuitManager,
            IExpiringTokenService expiringTokenService)
        {
            _dataService = dataService;
            _viewerHubContext = viewerHubContext;
            _appConfig = appConfig;
            _circuitManager = circuitManager;
            _expiringTokenService = expiringTokenService;
        }

        public static IMemoryCache ApiScriptResults { get; } = new MemoryCache(new MemoryCacheOptions());
        public static ConcurrentDictionary<string, Device> ServiceConnections { get; } = new ConcurrentDictionary<string, Device>();
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

        public static Device GetDevice(string deviceId)
        {
            return ServiceConnections.Values.FirstOrDefault(x => x.ID == deviceId);
        }

        public Task Chat(string message, bool disconnected, string browserConnectionId)
        {
            if (_circuitManager.TryGetConnection(browserConnectionId, out var connection))
            {
                return connection.InvokeCircuitEvent(CircuitEventName.ChatReceived, Device.ID, Device.DeviceName, message, disconnected);
            }
            else
            {
                return Clients.Caller.SendAsync("Chat", string.Empty, string.Empty, string.Empty, true, browserConnectionId);
            }
        }


        public async Task CheckForPendingSriptRuns()
        {
            var authToken = _expiringTokenService.GetToken(Time.Now.AddMinutes(AppConstants.ScriptRunExpirationMinutes));
            var scriptRuns = await _dataService.GetPendingScriptRuns(Device.ID);
            foreach (var run in scriptRuns)
            {
                await Clients.Caller.SendAsync("RunScript",
                    run.SavedScriptId,
                    run.Id,
                    run.Initiator,
                    run.InputType,
                    authToken);
            }
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
                    _dataService.WriteEvent(new EventLog()
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

                if (_dataService.AddOrUpdateDevice(device, out var updatedDevice))
                {
                    Device = updatedDevice;
                    ServiceConnections.AddOrUpdate(Context.ConnectionId, Device, (id, d) => Device);

                    var userIDs = _circuitManager.Connections.Select(x => x.User.Id);

                    var filteredUserIDs = _dataService.FilterUsersByDevicePermission(userIDs, Device.ID);

                    var connections = _circuitManager.Connections
                        .Where(x => x.User.OrganizationID == Device.OrganizationID &&
                            filteredUserIDs.Contains(x.User.Id));

                    foreach (var connection in connections)
                    {
                        connection.InvokeCircuitEvent(CircuitEventName.DeviceUpdate, Device);
                    }
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
                _dataService.WriteEvent(ex, device?.OrganizationID);
            }

            Context.Abort();
            return Task.FromResult(false);
        }

        public async Task DeviceHeartbeat(Device device)
        {
            if (CheckForDeviceBan(device.ID, device.DeviceName))
            {
                return;
            }

            var ip = Context.GetHttpContext()?.Connection?.RemoteIpAddress;
            if (ip != null && ip.IsIPv4MappedToIPv6)
            {
                ip = ip.MapToIPv4();
            }
            device.PublicIP = ip?.ToString();

            if (CheckForDeviceBan(device.PublicIP))
            {
                return;
            }


            _dataService.AddOrUpdateDevice(device, out var updatedDevice);
            Device = updatedDevice;
            ServiceConnections.AddOrUpdate(Context.ConnectionId, Device, (id, d) => Device);

            var userIDs = _circuitManager.Connections.Select(x => x.User.Id);

            var filteredUserIDs = _dataService.FilterUsersByDevicePermission(userIDs, Device.ID);

            var connections = _circuitManager.Connections
                .Where(x => x.User.OrganizationID == Device.OrganizationID &&
                    filteredUserIDs.Contains(x.User.Id));

            foreach (var connection in connections)
            {
                _ = connection.InvokeCircuitEvent(CircuitEventName.DeviceUpdate, Device);
            }


            await CheckForPendingSriptRuns();
        }


        public Task<bool> DisplayMessage(string consoleMessage, string popupMessage, string className, string requesterID)
        {
            return _circuitManager.InvokeOnConnection(requesterID, CircuitEventName.DisplayMessage, consoleMessage, popupMessage, className);
        }

        public Task<bool> DownloadFile(string fileID, string requesterID)
        {
            return _circuitManager.InvokeOnConnection(requesterID, CircuitEventName.DownloadFile, fileID);
        }

        public Task<bool> DownloadFileProgress(int progressPercent, string requesterID)
        {
            return _circuitManager.InvokeOnConnection(requesterID, CircuitEventName.DownloadFileProgress, progressPercent);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                if (Device != null)
                {
                    _dataService.DeviceDisconnected(Device.ID);

                    Device.IsOnline = false;

                    var userIDs = _circuitManager.Connections.Select(x => x.User.Id);

                    var filteredUserIDs = _dataService.FilterUsersByDevicePermission(userIDs, Device.ID);

                    var connections = _circuitManager.Connections
                        .Where(x => x.User.OrganizationID == Device.OrganizationID &&
                            filteredUserIDs.Contains(x.User.Id));

                    foreach (var connection in connections)
                    {
                        connection.InvokeCircuitEvent(CircuitEventName.DeviceWentOffline, Device);
                    }
                }
                return base.OnDisconnectedAsync(exception);
            }
            finally
            {
                ServiceConnections.TryRemove(Context.ConnectionId, out _);
            }
        }

        public Task ReturnPowerShellCompletions(PwshCommandCompletion completion, CompletionIntent intent, string senderConnectionId)
        {
            return _circuitManager.InvokeOnConnection(senderConnectionId, CircuitEventName.PowerShellCompletions, completion, intent);
        }

        public Task ScriptResult(string scriptResultId)
        {
            var result = _dataService.GetScriptResult(scriptResultId);
            return _circuitManager.InvokeOnConnection(result.SenderConnectionID,
                CircuitEventName.ScriptResult,
                result);
        }

        public void ScriptResultViaApi(string commandID, string requestID)
        {
            ApiScriptResults.Set(requestID, commandID, DateTimeOffset.Now.AddHours(1));
        }
        public Task SendConnectionFailedToViewers(List<string> viewerIDs)
        {
            return _viewerHubContext.Clients.Clients(viewerIDs).SendAsync("ConnectionFailed");
        }

        public Task SendLogs(string logChunk, string requesterConnectionId)
        {
            return _circuitManager.InvokeOnConnection(requesterConnectionId, CircuitEventName.RemoteLogsReceived, logChunk);
        }

        public Task SendServerVerificationToken()
        {
            return Clients.Caller.SendAsync("ServerVerificationToken", Device.ServerVerificationToken);
        }

        public void SetServerVerificationToken(string verificationToken)
        {
            Device.ServerVerificationToken = verificationToken;
            _dataService.SetServerVerificationToken(Device.ID, verificationToken);
        }

        public Task TransferCompleted(string transferID, string requesterID)
        {
            return _circuitManager.InvokeOnConnection(requesterID, CircuitEventName.TransferCompleted, transferID);
        }
        private bool CheckForDeviceBan(params string[] deviceIdNameOrIPs)
        {
            foreach (var device in deviceIdNameOrIPs)
            {
                if (string.IsNullOrWhiteSpace(device))
                {
                    continue;
                }

                if (_appConfig.BannedDevices.Any(x => !string.IsNullOrWhiteSpace(x) &&
                    x.Equals(device, StringComparison.OrdinalIgnoreCase)))
                {
                    _dataService.WriteEvent($"Device ID/name/IP ({device}) is banned.  Sending uninstall command.", null);

                    _ = Clients.Caller.SendAsync("UninstallAgent");
                    return true;
                }
            }
           
            return false;
        }
    }
}
