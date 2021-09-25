using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Remotely.Agent.Extensions;
using Remotely.Agent.Interfaces;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using Remotely.Shared.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Remotely.Agent.Services
{
    public class AgentSocket
    {
        private readonly IAppLauncher _appLauncher;

        private readonly ChatClientService _chatService;

        private readonly ConfigService _configService;

        private readonly IDeviceInformationService _deviceInfoService;

        private readonly ScriptExecutor _scriptExecutor;

        private readonly Uninstaller _uninstaller;

        private readonly IUpdater _updater;

        private ConnectionInfo _connectionInfo;
        private HubConnection _hubConnection;
        private System.Timers.Timer HeartbeatTimer;

        private bool IsServerVerified;

        public AgentSocket(ConfigService configService,
            Uninstaller uninstaller,
            ScriptExecutor scriptExecutor,
            ChatClientService chatService,
            IAppLauncher appLauncher,
            IUpdater updater,
            IDeviceInformationService deviceInfoService)
        {
            _configService = configService;
            _uninstaller = uninstaller;
            _scriptExecutor = scriptExecutor;
            _appLauncher = appLauncher;
            _chatService = chatService;
            _updater = updater;
            _deviceInfoService = deviceInfoService;
        }
        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
        public async Task Connect()
        {
            try
            {
                _connectionInfo = _configService.GetConnectionInfo();

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_connectionInfo.Host + "/AgentHub")
                    .AddMessagePackProtocol()
                    .Build();

                RegisterMessageHandlers();

                await _hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Failed to connect to server.  Internet connection may be unavailable.", EventType.Warning);
                return;
            }

            try
            {
                var device = await _deviceInfoService.CreateDevice(_connectionInfo.DeviceID, _connectionInfo.OrganizationID);

                var result = await _hubConnection.InvokeAsync<bool>("DeviceCameOnline", device);

                if (!result)
                {
                    // Orgnanization ID wasn't found, or this device is already connected.
                    // The above can be caused by temporary issues on the server.  So we'll do
                    // nothing here and wait for it to get resolved.
                    Logger.Write("There was an issue registering with the server.  The server might be undergoing maintenance, or the supplied organization ID might be incorrect.");
                    await Task.Delay(TimeSpan.FromMinutes(1));
                    await _hubConnection.StopAsync();
                    return;
                }

                if (!await VerifyServer())
                {
                    return;
                }

                if (await CheckForServerMigration())
                {
                    return;
                }

                HeartbeatTimer?.Dispose();
                HeartbeatTimer = new System.Timers.Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
                HeartbeatTimer.Elapsed += HeartbeatTimer_Elapsed;
                HeartbeatTimer.Start();

                await _hubConnection.SendAsync("CheckForPendingSriptRuns");
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Error starting websocket connection.", EventType.Error);
            }
        }

        public async Task HandleConnection()
        {
            while (true)
            {
                try
                {
                    if (!IsConnected)
                    {
                        var waitTime = new Random().Next(1000, 30000);
                        Logger.Write($"Websocket closed.  Reconnecting in {waitTime / 1000} seconds...");
                        await Task.Delay(waitTime);
                        await Program.Services.GetRequiredService<AgentSocket>().Connect();
                        await Program.Services.GetRequiredService<IUpdater>().CheckForUpdates();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
                await Task.Delay(1000);
            }
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
                Logger.Write(ex, EventType.Warning);
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

        private void RegisterMessageHandlers()
        {
            // TODO: Remove possibility for circular dependencies in the future
            // by emitting these events so other services can listen for them.

            _hubConnection.On("ChangeWindowsSession", async (string serviceID, string viewerID, int targetSessionID) =>
            {
                try
                {
                    if (!IsServerVerified)
                    {
                        Logger.Write("Session change attempted before server was verified.", EventType.Warning);
                        return;
                    }

                    await _appLauncher.RestartScreenCaster(new List<string>() { viewerID }, serviceID, viewerID, _hubConnection, targetSessionID);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });

            _hubConnection.On("Chat", async (string senderName, string message, string orgName, bool disconnected, string senderConnectionID) =>
            {
                try
                {
                    if (!IsServerVerified)
                    {
                        Logger.Write("Chat attempted before server was verified.", EventType.Warning);
                        return;
                    }

                    await _chatService.SendMessage(senderName, message, orgName, disconnected, senderConnectionID, _hubConnection);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });

            _hubConnection.On("CtrlAltDel", () =>
            {
                if (!IsServerVerified)
                {
                    Logger.Write("CtrlAltDel attempted before server was verified.", EventType.Warning);
                    return;
                }
                User32.SendSAS(false);
            });

            _hubConnection.On("DeleteLogs", () =>
           {
               Logger.DeleteLogs();
           });

            _hubConnection.On("DownloadFile", async (string filePath, string senderConnectionID) =>
            {
                try
                {
                    if (!IsServerVerified)
                    {
                        Logger.Write("File download attempted before server was verified.", EventType.Warning);
                        return;
                    }

                    filePath = filePath.Replace("\"", "");
                    if (!File.Exists(filePath))
                    {
                        await _hubConnection.SendAsync("DisplayMessage",
                            "File not found on remote device.",
                            "File not found.",
                            "bg-danger",
                            senderConnectionID);
                        return;
                    }

                    using var wc = new WebClient();
                    var lastProgressPercent = 0;
                    wc.UploadProgressChanged += async (sender, args) =>
                    {
                        if (args.ProgressPercentage > lastProgressPercent)
                        {
                            lastProgressPercent = args.ProgressPercentage;
                            await _hubConnection.SendAsync("DownloadFileProgress", lastProgressPercent, senderConnectionID);
                        }
                    };

                    try
                    {
                        var response = await wc.UploadFileTaskAsync($"{_connectionInfo.Host}/API/FileSharing/", filePath);
                        var fileIDs = JsonSerializer.Deserialize<string[]>(Encoding.UTF8.GetString(response));
                        await _hubConnection.SendAsync("DownloadFile", fileIDs[0], senderConnectionID);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex);
                        await _hubConnection.SendAsync("DisplayMessage",
                            "Error occurred while uploading file from remote computer.",
                            "Upload error.",
                            "bg-danger",
                            senderConnectionID);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });

            _hubConnection.On("ExecuteCommand", ((ScriptingShell shell, string command, string authToken, string senderUsername, string senderConnectionID) =>
            {
                try
                {
                    if (!IsServerVerified)
                    {
                        Logger.Write($"Command attempted before server was verified.  Shell: {shell}.  Command: {command}.  Sender: {senderConnectionID}", EventType.Warning);
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
                    Logger.Write(ex);
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
                    if (!IsServerVerified)
                    {
                        Logger.Write($"Command attempted before server was verified.  Shell: {shell}.  Command: {command}.  Sender: {senderUsername}", EventType.Warning);
                        return;
                    }

                    _ = _scriptExecutor.RunCommandFromApi(shell, requestID, command, senderUsername, authToken, _hubConnection);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });


            _hubConnection.On("GetLogs", async (string senderConnectionId) =>
            {
                var logBytes = await Logger.ReadAllLogs();

                if (!logBytes.Any())
                {
                    var message = "There are no log entries written.";

                    await _hubConnection.InvokeAsync("SendLogs", message, senderConnectionId);
                    return;
                }

                for (var i = 0; i < logBytes.Length; i += 50_000)
                {
                    var chunk = Encoding.UTF8.GetString(logBytes.Skip(i).Take(50_000).ToArray());
                    await _hubConnection.InvokeAsync("SendLogs", chunk, senderConnectionId);
                }
            });


            _hubConnection.On("GetPowerShellCompletions", async (string inputText, int currentIndex, CompletionIntent intent, bool? forward, string senderConnectionId) =>
            {
                try
                {
                    var session = PSCore.GetCurrent(senderConnectionId);
                    var completion = session.GetCompletions(inputText, currentIndex, forward);
                    var completionModel = completion.ToPwshCompletion();
                    await _hubConnection.InvokeAsync("ReturnPowerShellCompletions", completionModel, intent, senderConnectionId);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
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
                    Logger.Write(ex);
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
                    Logger.Write(ex);
                }
            });

            _hubConnection.On("RemoteControl", async (string requesterID, string serviceID) =>
            {
                try
                {
                    if (!IsServerVerified)
                    {
                        Logger.Write("Remote control attempted before server was verified.", EventType.Warning);
                        return;
                    }
                    await _appLauncher.LaunchRemoteControl(-1, requesterID, serviceID, _hubConnection);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });

            _hubConnection.On("RestartScreenCaster", async (List<string> viewerIDs, string serviceID, string requesterID) =>
            {
                try
                {
                    if (!IsServerVerified)
                    {
                        Logger.Write("Remote control attempted before server was verified.", EventType.Warning);
                        return;
                    }
                    await _appLauncher.RestartScreenCaster(viewerIDs, serviceID, requesterID, _hubConnection);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });


            _hubConnection.On("RunScript", (Guid savedScriptId, int scriptRunId, string initiator, ScriptInputType scriptInputType, string authToken) =>
            {
                try
                {
                    if (!IsServerVerified)
                    {
                        Logger.Write($"Script run attempted before server was verified.  Script ID: {savedScriptId}.  Initiator: {initiator}", EventType.Warning);
                        return;
                    }

                    _ = _scriptExecutor.RunScript(savedScriptId, scriptRunId, initiator, scriptInputType, authToken);

                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });

            _hubConnection.On("TransferFileFromBrowserToAgent", async (string transferID, List<string> fileIDs, string requesterID, string authToken) =>
            {
                try
                {
                    if (!IsServerVerified)
                    {
                        Logger.Write("File upload attempted before server was verified.", EventType.Warning);
                        return;
                    }

                    Logger.Write($"File upload started by {requesterID}.");
                    var sharedFilePath = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "RemotelySharedFiles")).FullName;

                    foreach (var fileID in fileIDs)
                    {
                        var url = $"{_connectionInfo.Host}/API/FileSharing/{fileID}";
                        var wr = WebRequest.CreateHttp(url);
                        wr.Headers[HttpRequestHeader.Authorization] = authToken;
                        using var response = await wr.GetResponseAsync();
                        var cd = response.Headers["Content-Disposition"];
                        var filename = cd
                                        .Split(";")
                                        .FirstOrDefault(x => x.Trim()
                                        .StartsWith("filename"))
                                        .Split("=")[1];

                        var legalChars = filename.ToCharArray().Where(x => !Path.GetInvalidFileNameChars().Any(y => x == y));

                        filename = new string(legalChars.ToArray());

                        using var rs = response.GetResponseStream();
                        using var fs = new FileStream(Path.Combine(sharedFilePath, filename), FileMode.Create);
                        rs.CopyTo(fs);
                    }
                    await _hubConnection.SendAsync("TransferCompleted", transferID, requesterID);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });

            _hubConnection.On("TriggerHeartbeat", async () =>
            {
                await SendHeartbeat();
            });
        }

        private async Task<bool> VerifyServer()
        {
            if (string.IsNullOrWhiteSpace(_connectionInfo.ServerVerificationToken))
            {
                IsServerVerified = true;
                _connectionInfo.ServerVerificationToken = Guid.NewGuid().ToString();
                await _hubConnection.SendAsync("SetServerVerificationToken", _connectionInfo.ServerVerificationToken);
                _configService.SaveConnectionInfo(_connectionInfo);
            }
            else
            {
                var verificationToken = await _hubConnection.InvokeAsync<string>("GetServerVerificationToken");

                if (verificationToken == _connectionInfo.ServerVerificationToken)
                {
                    IsServerVerified = true;
                }
                else
                {
                    Logger.Write($"Server sent an incorrect verification token.  Token Sent: {verificationToken}.", EventType.Warning);
                    return false;
                }
            }

            return true;
        }
    }
}
