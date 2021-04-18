using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Remotely.Server.Auth;
using Remotely.Server.Models;
using Remotely.Server.Services;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.Hubs
{
    public interface ICircuitConnection
    {
        event EventHandler<CircuitEvent> MessageReceived;
        RemotelyUser User { get; }

        Task DownloadFile(string filePath, string deviceID);

        Task ExecuteCommandOnAgent(ScriptingShell shell, string command, string[] deviceIDs);

        Task GetPowerShellCompletions(string inputText, int currentIndex, CompletionIntent intent, bool? forward);

        Task GetRemoteLogs(string deviceId);

        Task InvokeCircuitEvent(CircuitEventName eventName, params object[] args);
        Task ReinstallAgents(string[] deviceIDs);

        Task<bool> RemoteControl(string deviceID);

        Task RemoveDevices(string[] deviceIDs);

        Task RunScript(IEnumerable<string> deviceIds, Guid savedScriptId, int scriptRunId, ScriptInputType scriptInputType, bool runAsHostedService);

        Task SendChat(string message, string deviceId);
        Task<bool> TransferFileFromBrowserToAgent(string deviceId, string transferId, string[] fileIds);

        Task UninstallAgents(string[] deviceIDs);
        Task UpdateTags(string deviceID, string tags);
        Task UploadFiles(List<string> fileIDs, string transferID, string[] deviceIDs);
        Task TriggerHeartbeat(string deviceId);
    }

    public class CircuitConnection : CircuitHandler, ICircuitConnection
    {
        private readonly IHubContext<AgentHub> _agentHubContext;
        private readonly IApplicationConfig _appConfig;
        private readonly IClientAppState _appState;
        private readonly IAuthService _authService;
        private readonly ICircuitManager _circuitManager;
        private readonly IDataService _dataService;
        private readonly ConcurrentQueue<CircuitEvent> _eventQueue = new();
        private readonly IExpiringTokenService _expiringTokenService;
        private readonly ILogger<CircuitConnection> _logger;
        private readonly IToastService _toastService;
        public CircuitConnection(
            IAuthService authService,
            IDataService dataService,
            IClientAppState appState,
            IHubContext<AgentHub> agentHubContext,
            IApplicationConfig appConfig,
            ICircuitManager circuitManager,
            IToastService toastService,
            IExpiringTokenService expiringTokenService,
            ILogger<CircuitConnection> logger)
        {
            _dataService = dataService;
            _agentHubContext = agentHubContext;
            _appState = appState;
            _appConfig = appConfig;
            _authService = authService;
            _circuitManager = circuitManager;
            _toastService = toastService;
            _expiringTokenService = expiringTokenService;
            _logger = logger;
        }


        public event EventHandler<CircuitEvent> MessageReceived;

        public string ConnectionId { get; set; }
        public RemotelyUser User { get; set; }


        public Task DownloadFile(string filePath, string deviceID)
        {
            if (_dataService.DoesUserHaveAccessToDevice(deviceID, User))
            {
                var targetDevice = AgentHub.ServiceConnections.FirstOrDefault(x => x.Value.ID == deviceID);
                _agentHubContext.Clients.Client(targetDevice.Key).SendAsync("DownloadFile", filePath, ConnectionId);
            }
            return Task.CompletedTask;
        }

        public Task ExecuteCommandOnAgent(ScriptingShell shell, string command, string[] deviceIDs)
        {
            deviceIDs = _dataService.FilterDeviceIDsByUserPermission(deviceIDs, User);
            var connections = GetActiveClientConnections(deviceIDs);

            _logger.LogInformation("Command executed by {username}.  Shell: {shell}.  Command: {command}.  Devices: {deviceIds}",
                  User.UserName,
                  shell,
                  command,
                  string.Join(", ", deviceIDs));

            var authTokenForUploadingResults = _expiringTokenService.GetToken(Time.Now.AddMinutes(5));

            foreach (var connection in connections)
            {
                _agentHubContext.Clients.Client(connection.Key).SendAsync("ExecuteCommand",
                    shell,
                    command,
                    authTokenForUploadingResults,
                    User.UserName,
                    ConnectionId);
            }

            return Task.CompletedTask;
        }

        public Task GetPowerShellCompletions(string inputText, int currentIndex, CompletionIntent intent, bool? forward)
        {
            var (canAccess, key) = CanAccessDevice(_appState.DevicesFrameSelectedDevices.FirstOrDefault());
            if (!canAccess)
            {
                return Task.CompletedTask;
            }

            return _agentHubContext.Clients.Client(key).SendAsync("GetPowerShellCompletions", inputText, currentIndex, intent, forward, ConnectionId);
        }

        public Task GetRemoteLogs(string deviceId)
        {
            var (canAccess, key) = CanAccessDevice(deviceId);
            if (!canAccess)
            {
                _toastService.ShowToast("Access denied.", classString: "bg-warning");
                return Task.CompletedTask;
            }

            return _agentHubContext.Clients.Client(key).SendAsync("GetLogs", ConnectionId);
        }

        public Task InvokeCircuitEvent(CircuitEventName eventName, params object[] args)
        {
            _eventQueue.Enqueue(new CircuitEvent(eventName, args));
            return Task.Run(ProcessMessages);
        }

        public override async Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(ConnectionId))
            {
                _circuitManager.TryRemoveConnection(ConnectionId, out _);
            }
            await base.OnCircuitClosedAsync(circuit, cancellationToken);
        }

        public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            if (await _authService.IsAuthenticated())
            {
                User = await _authService.GetUser();
                ConnectionId = Guid.NewGuid().ToString();
                _circuitManager.TryAddConnection(ConnectionId, this);
            }
            await base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }

        public Task ReinstallAgents(string[] deviceIDs)
        {
            deviceIDs = _dataService.FilterDeviceIDsByUserPermission(deviceIDs, User);
            var connections = GetActiveClientConnections(deviceIDs);
            foreach (var connection in connections)
            {
                _agentHubContext.Clients.Client(connection.Key).SendAsync("ReinstallAgent");
            }
            _dataService.RemoveDevices(deviceIDs);
            return Task.CompletedTask;
        }

        public async Task<bool> RemoteControl(string deviceID)
        {
            var targetDevice = AgentHub.ServiceConnections.FirstOrDefault(x => x.Value.ID == deviceID);

            if (targetDevice.Value is null)
            {
                MessageReceived?.Invoke(this, new CircuitEvent(CircuitEventName.DisplayMessage,
                    "The selected device is not online.",
                    "Device is not online.",
                    "bg-warning"));
                return false;
            }

            if (_dataService.DoesUserHaveAccessToDevice(deviceID, User))
            {
                var currentUsers = CasterHub.SessionInfoList.Count(x => x.Value.OrganizationID == User.OrganizationID);
                if (currentUsers >= _appConfig.RemoteControlSessionLimit)
                {
                    MessageReceived?.Invoke(this, new CircuitEvent(CircuitEventName.DisplayMessage,
                        "There are already the maximum amount of active remote control sessions for your organization.",
                        "Max number of concurrent sessions reached.",
                        "bg-warning"));
                    return false;
                }
                await _agentHubContext.Clients.Client(targetDevice.Key).SendAsync("RemoteControl", ConnectionId, targetDevice.Key);
                return true;
            }
            else
            {
                _dataService.WriteEvent($"Remote control attempted by unauthorized user.  Device ID: {deviceID}.  User Name: {User.UserName}.", EventType.Warning, targetDevice.Value.OrganizationID);
                return false;
            }
        }

        public Task RemoveDevices(string[] deviceIDs)
        {
            var filterDevices = _dataService.FilterDeviceIDsByUserPermission(deviceIDs, User);
            _dataService.RemoveDevices(filterDevices);
            return Task.CompletedTask;
        }

        public async Task RunScript(IEnumerable<string> deviceIds, Guid savedScriptId, int scriptRunId, ScriptInputType scriptInputType, bool runAsHostedService)
        {
            string username;
            if (runAsHostedService)
            {
                username = "Remotely Server";
            }
            else
            {
                username = User.UserName;
                deviceIds = _dataService.FilterDeviceIDsByUserPermission(deviceIds.ToArray(), User);
            }
           
            var authToken = _expiringTokenService.GetToken(Time.Now.AddMinutes(AppConstants.ScriptRunExpirationMinutes));

            var connectionIds = AgentHub.ServiceConnections.Where(x => deviceIds.Contains(x.Value.ID)).Select(x=>x.Key);

            if (connectionIds.Any())
            {
                await _agentHubContext.Clients.Clients(connectionIds).SendAsync("RunScript", savedScriptId, scriptRunId, username, scriptInputType, authToken);
            }

        }

        public Task SendChat(string message, string deviceId)
        {
            if (!_dataService.DoesUserHaveAccessToDevice(deviceId, User))
            {
                return Task.CompletedTask;
            }

            var connection = AgentHub.ServiceConnections.FirstOrDefault(x =>
                x.Value.OrganizationID == User.OrganizationID &&
                x.Value.ID == deviceId
            );

            if (connection.Value is null)
            {
                _toastService.ShowToast("Device not found.");
                return Task.CompletedTask;
            }

            var organizationName = _dataService.GetOrganizationNameByUserName(User.UserName);

            return _agentHubContext.Clients.Client(connection.Key).SendAsync("Chat",
                User.UserOptions.DisplayName ?? User.UserName,
                message,
                organizationName,
                false,
                ConnectionId);
        }

        public async Task<bool> TransferFileFromBrowserToAgent(string deviceId, string transferId, string[] fileIds)
        {
            var serviceConnection = AgentHub.ServiceConnections.FirstOrDefault(x => x.Value.ID == deviceId);

            if (serviceConnection.Value is null)
            {
                return false;
            }

            if (!_dataService.DoesUserHaveAccessToDevice(deviceId, User))
            {
                _logger.LogWarning("User {username} does not have access to device ID {deviceId} and attempted file upload.",
                    User.UserName,
                    deviceId);

                return false;
            }

            var authToken = _expiringTokenService.GetToken(Time.Now.AddMinutes(5));

            await _agentHubContext.Clients.Client(serviceConnection.Key).SendAsync(
                "TransferFileFromBrowserToAgent",
                transferId,
                fileIds,
                ConnectionId,
                authToken);

            return true;
        }

        public async Task TriggerHeartbeat(string deviceId)
        {
            var (canAccess, connectionId) = CanAccessDevice(deviceId);

            if (!canAccess)
            {
                return;
            }

            await _agentHubContext.Clients.Client(connectionId).SendAsync("TriggerHeartbeat");
        }

        public Task UninstallAgents(string[] deviceIDs)
        {
            deviceIDs = _dataService.FilterDeviceIDsByUserPermission(deviceIDs, User);
            var connections = GetActiveClientConnections(deviceIDs);
            foreach (var connection in connections)
            {
                _agentHubContext.Clients.Client(connection.Key).SendAsync("UninstallAgent");
            }
            _dataService.RemoveDevices(deviceIDs);
            return Task.CompletedTask;
        }

        public Task UpdateTags(string deviceID, string tags)
        {
            if (_dataService.DoesUserHaveAccessToDevice(deviceID, User))
            {
                if (tags.Length > 200)
                {
                    MessageReceived?.Invoke(this, new CircuitEvent(CircuitEventName.DisplayMessage,
                        $"Tag must be 200 characters or less. Supplied length is {tags.Length}.",
                        "Tag must be under 200 characters.",
                        "bg-warning"));
                }
                _dataService.UpdateTags(deviceID, tags);
                MessageReceived?.Invoke(this, new CircuitEvent(CircuitEventName.DisplayMessage,
                    "Device updated successfully.",
                    "Device updated.",
                    "bg-success"));
            }
            return Task.CompletedTask;
        }

        public Task UploadFiles(List<string> fileIDs, string transferID, string[] deviceIDs)
        {
            _dataService.WriteEvent(new EventLog()
            {
                EventType = EventType.Info,
                Message = $"File transfer started by {User.UserName}.  File transfer IDs: {string.Join(", ", fileIDs)}.",
                TimeStamp = Time.Now,
                OrganizationID = User.OrganizationID
            });
            deviceIDs = _dataService.FilterDeviceIDsByUserPermission(deviceIDs, User);
            var connections = GetActiveClientConnections(deviceIDs);
            foreach (var connection in connections)
            {
                _agentHubContext.Clients.Client(connection.Key).SendAsync("UploadFiles", transferID, fileIDs, ConnectionId);
            }
            return Task.CompletedTask;
        }

        private (bool canAccess, string connectionId) CanAccessDevice(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return (false, null);
            }

            var kvp = AgentHub.ServiceConnections.FirstOrDefault(x => x.Value.ID == deviceId);

            if (kvp.Value is null)
            {
                return (false, null);
            }

            if (!_dataService.DoesUserHaveAccessToDevice(kvp.Value.ID, User))
            {
                return (false, null);
            }

            return (true, kvp.Key);
        }

        private IEnumerable<KeyValuePair<string, Device>> GetActiveClientConnections(IEnumerable<string> deviceIDs)
        {
            return AgentHub.ServiceConnections.Where(x =>
                x.Value.OrganizationID == User.OrganizationID &&
                deviceIDs.Contains(x.Value.ID)
            );
        }

        private void ProcessMessages()
        {
            lock (_eventQueue)
            {
                while (_eventQueue.TryDequeue(out var circuitEvent))
                {
                    try
                    {
                        MessageReceived?.Invoke(this, circuitEvent);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while invoking circuit event.");
                    }
                }
            }
        }
    }
}
