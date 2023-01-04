using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Agent.Extensions;
using Remotely.Agent.Interfaces;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Services;
using Remotely.Shared.Utilities;
using Remotely.Shared.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Remotely.Agent.Services
{
    public interface IAgentHubConnection
    {
        bool IsConnected { get; }

        Task Connect();
        Task SendHeartbeat();
    }

    public class AgentHubConnection : IAgentHubConnection, IDisposable
    {
        private readonly IAppLauncher _appLauncher;

        private readonly ChatClientService _chatService;

        private readonly ConfigService _configService;

        private readonly IDeviceInformationService _deviceInfoService;
        private readonly IHttpClientFactory _httpFactory;
        private readonly ILogger<AgentHubConnection> _logger;
        private readonly ILogger _fileLogger;
        private readonly ScriptExecutor _scriptExecutor;

        private readonly Uninstaller _uninstaller;

        private readonly IUpdater _updater;

        private ConnectionInfo _connectionInfo;
        private HubConnection _hubConnection;
        private Timer _heartbeatTimer;
        private bool _isServerVerified;

        public AgentHubConnection(ConfigService configService,
            Uninstaller uninstaller,
            ScriptExecutor scriptExecutor,
            ChatClientService chatService,
            IAppLauncher appLauncher,
            IUpdater updater,
            IDeviceInformationService deviceInfoService,
            IHttpClientFactory httpFactory,
            IEnumerable<ILoggerProvider> loggerProviders,
            ILogger<AgentHubConnection> logger)
        {
            _configService = configService;
            _uninstaller = uninstaller;
            _scriptExecutor = scriptExecutor;
            _appLauncher = appLauncher;
            _chatService = chatService;
            _updater = updater;
            _deviceInfoService = deviceInfoService;
            _httpFactory = httpFactory;
            _logger = logger;
            _fileLogger = loggerProviders
                .OfType<FileLoggerProvider>()
                .FirstOrDefault()
                ?.CreateLogger(nameof(AgentHubConnection));
        }

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public async Task Connect()
        {
            while (true)
            {
                try
                {
                    _logger.LogInformation("Attempting to connect to server.");

                    _connectionInfo = _configService.GetConnectionInfo();

                    if (_hubConnection is not null)
                    {
                        await _hubConnection.DisposeAsync();
                    }

                    _hubConnection = new HubConnectionBuilder()
                        .WithUrl(_connectionInfo.Host + "/hubs/service")
                        .WithAutomaticReconnect(new RetryPolicy())
                        .AddMessagePackProtocol()
                        .Build();

                    RegisterMessageHandlers();

                    _hubConnection.Reconnected += HubConnection_Reconnected;

                    await _hubConnection.StartAsync();

                    _logger.LogInformation("Connected to server.");

                    var device = await _deviceInfoService.CreateDevice(_connectionInfo.DeviceID, _connectionInfo.OrganizationID);

                    var result = await _hubConnection.InvokeAsync<bool>("DeviceCameOnline", device);

                    if (!result)
                    {
                        // Orgnanization ID wasn't found, or this device is already connected.
                        // The above can be caused by temporary issues on the server.  So we'll do
                        // nothing here and wait for it to get resolved.
                        _logger.LogError("There was an issue registering with the server.  The server might be undergoing maintenance, or the supplied organization ID might be incorrect.");
                        await Task.Delay(TimeSpan.FromMinutes(1));
                        continue;
                    }

                    if (!await VerifyServer())
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1));
                        continue;
                    }

                    if (await CheckForServerMigration())
                    {
                        continue;
                    }

                    _heartbeatTimer?.Dispose();
                    _heartbeatTimer = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
                    _heartbeatTimer.Elapsed += HeartbeatTimer_Elapsed;
                    _heartbeatTimer.Start();

                    await _hubConnection.SendAsync("CheckForPendingSriptRuns");

                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while connecting to server.");
                    await Task.Delay(5_000);
                }


            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _heartbeatTimer?.Dispose();
        }

        public async Task SendHeartbeat()
        {
            try
            {
                var currentInfo = await _deviceInfoService.CreateDevice(_connectionInfo.DeviceID, _connectionInfo.OrganizationID);
                await _hubConnection.SendAsync("DeviceHeartbeat", currentInfo);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while sending heartbeat.");
            }
        }

        private async Task<bool> CheckForServerMigration()
        {
            var serverUrl = await _hubConnection.InvokeAsync<string>("GetServerUrl");

            if (Uri.TryCreate(serverUrl, UriKind.Absolute, out var serverUri) &&
                Uri.TryCreate(_connectionInfo.Host, UriKind.Absolute, out var savedUri) &&
                serverUri.Host != savedUri.Host)
            {
                _connectionInfo.Host = serverUrl.Trim().TrimEnd('/');
                _connectionInfo.ServerVerificationToken = null;
                _configService.SaveConnectionInfo(_connectionInfo);
                await _hubConnection.DisposeAsync();
                return true;
            }
            return false;
        }

        private async void HeartbeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await SendHeartbeat();
        }

        private async Task HubConnection_Reconnected(string arg)
        {
            _logger.LogInformation("Reconnected to server.");
            var device = await _deviceInfoService.CreateDevice(_connectionInfo.DeviceID, _connectionInfo.OrganizationID);
            _ = await _hubConnection.InvokeAsync<bool>("DeviceCameOnline", device);
            await _updater.CheckForUpdates();
        }
        private void RegisterMessageHandlers()
        {

            _hubConnection.On("ChangeWindowsSession", async (string viewerConnectionId, string sessionId, string accessKey, string userConnectionId, string requesterName, string orgName, string orgId, int targetSessionID) =>
            {
                try
                {
                    if (!_isServerVerified)
                    {
                        _logger.LogWarning("Session change attempted before server was verified.");
                        return;
                    }

                    await _appLauncher.RestartScreenCaster(new List<string>() { viewerConnectionId }, sessionId, accessKey, userConnectionId, requesterName, orgName, orgId, _hubConnection, targetSessionID);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while handling ChangeWindowsSession.");
                }
            });

            _hubConnection.On("Chat", async (string senderName, string message, string orgName, string orgId, bool disconnected, string senderConnectionID) =>
            {
                try
                {
                    if (!_isServerVerified)
                    {
                        _logger.LogWarning("Chat attempted before server was verified.");
                        return;
                    }

                    await _chatService.SendMessage(senderName, message, orgName, orgId, disconnected, senderConnectionID, _hubConnection);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while handling chat message.");
                }
            });

            _hubConnection.On("CtrlAltDel", () =>
            {
                if (!_isServerVerified)
                {
                    _logger.LogWarning("CtrlAltDel attempted before server was verified.");
                    return;
                }
                User32.SendSAS(false);
            });

            _hubConnection.On("DeleteLogs", () =>
           {
               if (_fileLogger is FileLogger logger)
               {
                   logger.DeleteLogs();
               }
           });


            _hubConnection.On("ExecuteCommand", ((ScriptingShell shell, string command, string authToken, string senderUsername, string senderConnectionID) =>
            {
                try
                {
                    if (!_isServerVerified)
                    {
                        _logger.LogWarning(
                            "Command attempted before server was verified.  Shell: {shell}.  Command: {command}.  Sender: {senderConnectionID}",
                            shell,
                            command,
                            senderConnectionID);
                        return;
                    }

                    _ = _scriptExecutor.RunCommandFromTerminal(shell,
                        command,
                        authToken,
                        senderUsername,
                        senderConnectionID,
                        ScriptInputType.Terminal,
                        TimeSpan.FromSeconds(30),
                        _hubConnection);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while executing command.");
                }
            }));

            _hubConnection.On("ExecuteCommandFromApi", (
                ScriptingShell shell,
                string authToken,
                string requestID,
                string command,
                string senderUsername) =>
            {
                try
                {
                    if (!_isServerVerified)
                    {
                        _logger.LogWarning(
                            "Command attempted before server was verified.  Shell: {shell}.  Command: {command}.  Sender: {senderUsername}",
                            shell,
                            command,
                            senderUsername);
                        return;
                    }

                    _ = _scriptExecutor.RunCommandFromApi(shell, requestID, command, senderUsername, authToken, _hubConnection);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while executing command from API.");
                }
            });


            _hubConnection.On("GetLogs", async (string senderConnectionId) =>
            {
                if (_fileLogger is not FileLogger logger)
                {
                    await _hubConnection.InvokeAsync("SendLogs", "Logger is not of expected type.", senderConnectionId).ConfigureAwait(false);
                    return;
                }

                var logBytes = await logger.ReadAllBytes();

                if (!logBytes.Any())
                {
                    var message = "There are no log entries written.";

                    await _hubConnection.InvokeAsync("SendLogs", message, senderConnectionId).ConfigureAwait(false);
                    return;
                }

                for (var i = 0; i < logBytes.Length; i += 50_000)
                {
                    var chunk = Encoding.UTF8.GetString(logBytes.Skip(i).Take(50_000).ToArray());
                    await _hubConnection.InvokeAsync("SendLogs", chunk, senderConnectionId).ConfigureAwait(false);
                }
            });


            _hubConnection.On("GetPowerShellCompletions", async (string inputText, int currentIndex, CompletionIntent intent, bool? forward, string senderConnectionId) =>
            {
                try
                {
                    var session = PSCore.GetCurrent(senderConnectionId);
                    var completion = session.GetCompletions(inputText, currentIndex, forward);
                    var completionModel = completion.ToPwshCompletion();
                    await _hubConnection.InvokeAsync("ReturnPowerShellCompletions", completionModel, intent, senderConnectionId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while getting PowerShell completions.");
                }
            });


            _hubConnection.On("ReinstallAgent", async () =>
            {
                try
                {
                    await _updater.InstallLatestVersion();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while reinstalling agent.");
                }
            });

            _hubConnection.On("UninstallAgent", () =>
            {
                try
                {
                    _uninstaller.UninstallAgent();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while uninstalling agent.");
                }
            });

            _hubConnection.On("RemoteControl", async (string sessionId, string accessKey, string userConnectionId, string requesterName, string orgName, string orgId) =>
            {
                try
                {
                    if (!_isServerVerified)
                    {
                        _logger.LogWarning("Remote control attempted before server was verified.");
                        return;
                    }
                    await _appLauncher.LaunchRemoteControl(-1, sessionId, accessKey, userConnectionId, requesterName, orgName, orgId, _hubConnection);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while starting remote control.");
                }
            });

            _hubConnection.On("RestartScreenCaster", async (List<string> viewerIDs, string sessionId, string accessKey, string userConnectionId, string requesterName, string orgName, string orgId) =>
            {
                try
                {
                    if (!_isServerVerified)
                    {
                        _logger.LogWarning("Remote control attempted before server was verified.");
                        return;
                    }
                    await _appLauncher.RestartScreenCaster(viewerIDs, sessionId, accessKey, userConnectionId, requesterName, orgName, orgId, _hubConnection);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while restarting screen caster.");
                }
            });


            _hubConnection.On("RunScript", (Guid savedScriptId, int scriptRunId, string initiator, ScriptInputType scriptInputType, string authToken) =>
            {
                try
                {
                    if (!_isServerVerified)
                    {
                        _logger.LogWarning(
                            "Script run attempted before server was verified.  Script ID: {savedScriptId}.  Initiator: {initiator}",
                            savedScriptId,
                            initiator);
                        return;
                    }

                    _ = _scriptExecutor.RunScript(savedScriptId, scriptRunId, initiator, scriptInputType, authToken);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while running script.");
                }
            });

            _hubConnection.On("TransferFileFromBrowserToAgent", async (string transferID, List<string> fileIDs, string requesterID, string authToken) =>
            {
                try
                {
                    if (!_isServerVerified)
                    {
                        _logger.LogWarning("File upload attempted before server was verified.");
                        return;
                    }

                    _logger.LogInformation("File upload started by {requesterID}.", requesterID);

                    var sharedFilePath = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "RemotelySharedFiles")).FullName;

                    foreach (var fileID in fileIDs)
                    {
                        var url = $"{_connectionInfo.Host}/API/FileSharing/{fileID}";
                        using var client = _httpFactory.CreateClient();
                        client.DefaultRequestHeaders.Add("Authorization", authToken);
                        using var response = await client.GetAsync(url);

                        var filename = response.Content.Headers.ContentDisposition.FileName;
                        var legalChars = filename.ToCharArray().Where(x => !Path.GetInvalidFileNameChars().Any(y => x == y));

                        filename = new string(legalChars.ToArray());

                        using var rs = await response.Content.ReadAsStreamAsync();
                        using var fs = new FileStream(Path.Combine(sharedFilePath, filename), FileMode.Create);
                        rs.CopyTo(fs);
                    }
                    await _hubConnection.SendAsync("TransferCompleted", transferID, requesterID);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while transfering file from browser to agent.");
                }
            });

            _hubConnection.On("TriggerHeartbeat", async () =>
            {
                await SendHeartbeat().ConfigureAwait(false);
            });
        }

        private async Task<bool> VerifyServer()
        {
            if (string.IsNullOrWhiteSpace(_connectionInfo.ServerVerificationToken))
            {
                _isServerVerified = true;
                _connectionInfo.ServerVerificationToken = Guid.NewGuid().ToString();
                await _hubConnection.SendAsync("SetServerVerificationToken", _connectionInfo.ServerVerificationToken).ConfigureAwait(false);
                _configService.SaveConnectionInfo(_connectionInfo);
            }
            else
            {
                var verificationToken = await _hubConnection.InvokeAsync<string>("GetServerVerificationToken");

                if (verificationToken == _connectionInfo.ServerVerificationToken)
                {
                    _isServerVerified = true;
                }
                else
                {
                    _logger.LogWarning("Server sent an incorrect verification token.  Token Sent: {verificationToken}.", verificationToken);
                    return false;
                }
            }

            return true;
        }

        private class RetryPolicy : IRetryPolicy
        {
            public TimeSpan? NextRetryDelay(RetryContext retryContext)
            {
                if (retryContext.PreviousRetryCount == 0)
                {
                    return TimeSpan.FromSeconds(3);
                }

                var waitSeconds = Math.Min(30, retryContext.PreviousRetryCount * 5);
                return TimeSpan.FromSeconds(waitSeconds);
            }
        }
    }
}
