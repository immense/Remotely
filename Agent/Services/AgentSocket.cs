using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Remotely.Agent.Interfaces;
using Remotely.Agent.Utilities;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Services;
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
        public AgentSocket(ConfigService configService,
            Uninstaller uninstaller,
            CommandExecutor commandExecutor,
            ScriptRunner scriptRunner,
            ChatClientService chatService,
            IAppLauncher appLauncher,
            IUpdater updater)
        {
            ConfigService = configService;
            Uninstaller = uninstaller;
            CommandExecutor = commandExecutor;
            ScriptRunner = scriptRunner;
            AppLauncher = appLauncher;
            ChatService = chatService;
            Updater = updater;
        }
        public bool IsConnected => HubConnection?.State == HubConnectionState.Connected;
        private IAppLauncher AppLauncher { get; }
        private ChatClientService ChatService { get; }
        private CommandExecutor CommandExecutor { get; }
        private ConfigService ConfigService { get; }
        private ConnectionInfo ConnectionInfo { get; set; }
        private System.Timers.Timer HeartbeatTimer { get; set; }
        private HubConnection HubConnection { get; set; }
        private bool IsServerVerified { get; set; }
        private ScriptRunner ScriptRunner { get; }
        private Uninstaller Uninstaller { get; }
        private IUpdater Updater { get; }

        public async Task Connect()
        {
            try
            {
                ConnectionInfo = ConfigService.GetConnectionInfo();

                HubConnection = new HubConnectionBuilder()
                    .WithUrl(ConnectionInfo.Host + "/AgentHub")
                    .Build();

                RegisterMessageHandlers();

                await HubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Failed to connect to server.  Internet connection may be unavailable.", EventType.Warning);
                return;
            }

            try
            {
                var device = await DeviceInformation.Create(ConnectionInfo.DeviceID, ConnectionInfo.OrganizationID);

                var result = await HubConnection.InvokeAsync<bool>("DeviceCameOnline", device);

                if (!result)
                {
                    // Orgnanization ID wasn't found, or this device is already connected.
                    // The above can be caused by temporary issues on the server.  So we'll do
                    // nothing here and wait for it to get resolved.
                    Logger.Write("There was an issue registering with the server.  The server might be undergoing maintenance, or the supplied organization ID might be incorrect.");
                    await Task.Delay(TimeSpan.FromMinutes(1));
                    await HubConnection.StopAsync();
                    return;
                }

                if (string.IsNullOrWhiteSpace(ConnectionInfo.ServerVerificationToken))
                {
                    IsServerVerified = true;
                    ConnectionInfo.ServerVerificationToken = Guid.NewGuid().ToString();
                    await HubConnection.SendAsync("SetServerVerificationToken", ConnectionInfo.ServerVerificationToken);
                    ConfigService.SaveConnectionInfo(ConnectionInfo);
                }
                else
                {
                    await HubConnection.SendAsync("SendServerVerificationToken");
                }

                HeartbeatTimer?.Dispose();
                HeartbeatTimer = new System.Timers.Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
                HeartbeatTimer.Elapsed += HeartbeatTimer_Elapsed;
                HeartbeatTimer.Start();

                if (EnvironmentHelper.IsWindows &&
                    !RegistryHelper.CheckNetFrameworkVersion())
                {
                    await SendDeviceAlert("The .NET Framework version on this device is outdated, and " +
                        "Remotely will no longer receive updates.  Update the installed .NET Framework version " +
                        "to fix this.");
                }
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

        public async Task SendDeviceAlert(string alertMessage)
        {
            try
            {
                await HubConnection.SendAsync("AddDeviceAlert", alertMessage);
            }
            catch (Exception ex)
            {
                Logger.Write(ex, EventType.Warning);
            }
        }
        public async Task SendHeartbeat()
        {
            try
            {
                var currentInfo = await DeviceInformation.Create(ConnectionInfo.DeviceID, ConnectionInfo.OrganizationID);
                await HubConnection.SendAsync("DeviceHeartbeat", currentInfo);
            }
            catch (Exception ex)
            {
                Logger.Write(ex, EventType.Warning);
            }
        }

        private async void HeartbeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await SendHeartbeat();
        }

        private void RegisterMessageHandlers()
        {
            // TODO: Remove possibility for circular dependencies in the future
            // by emitting these events so other services can listen for them.

            HubConnection.On("Chat", async (string senderName, string message, string orgName, bool disconnected, string senderConnectionID) =>
            {
                try
                {
                    if (!IsServerVerified)
                    {
                        Logger.Write("Chat attempted before server was verified.", EventType.Warning);
                        return;
                    }

                    await ChatService.SendMessage(senderName, message, orgName, disconnected, senderConnectionID, HubConnection);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });
            HubConnection.On("DownloadFile", async (string filePath, string senderConnectionID) =>
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
                        await HubConnection.SendAsync("DisplayMessage", "File not found on remote device.", "File not found.", senderConnectionID);
                        return;
                    }

                    using var wc = new WebClient();
                    var lastProgressPercent = 0;
                    wc.UploadProgressChanged += async (sender, args) =>
                    {
                        if (args.ProgressPercentage > lastProgressPercent)
                        {
                            lastProgressPercent = args.ProgressPercentage;
                            await HubConnection.SendAsync("DownloadFileProgress", lastProgressPercent, senderConnectionID);
                        }
                    };

                    try
                    {
                        var response = await wc.UploadFileTaskAsync($"{ConnectionInfo.Host}/API/FileSharing/", filePath);
                        var fileIDs = JsonSerializer.Deserialize<string[]>(Encoding.UTF8.GetString(response));
                        await HubConnection.SendAsync("DownloadFile", fileIDs[0], senderConnectionID);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex);
                        await HubConnection.SendAsync("DisplayMessage", "Error occurred while uploading file from remote computer.", "Upload error.", senderConnectionID);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });
            HubConnection.On("ChangeWindowsSession", async (string serviceID, string viewerID, int targetSessionID) =>
            {
                try
                {
                    if (!IsServerVerified)
                    {
                        Logger.Write("Session change attempted before server was verified.", EventType.Warning);
                        return;
                    }

                    await AppLauncher.RestartScreenCaster(new List<string>() { viewerID }, serviceID, viewerID, HubConnection, targetSessionID);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });
            HubConnection.On("ExecuteCommand", (async (string mode, string command, string commandID, string senderConnectionID) =>
            {
                try
                {
                    if (!IsServerVerified)
                    {
                        Logger.Write($"Command attempted before server was verified.  Mode: {mode}.  Command: {command}.  Sender: {senderConnectionID}", EventType.Warning);
                        return;
                    }

                    await CommandExecutor.ExecuteCommand(mode, command, commandID, senderConnectionID, HubConnection);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }));
            HubConnection.On("ExecuteCommandFromApi", (async (string mode, string requestID, string command, string commandID, string senderUserName) =>
            {
                try
                {
                    if (!IsServerVerified)
                    {
                        Logger.Write($"Command attempted before server was verified.  Mode: {mode}.  Command: {command}.  Sender: {senderUserName}", EventType.Warning);
                        return;
                    }

                    await CommandExecutor.ExecuteCommandFromApi(mode, requestID, command, commandID, senderUserName, HubConnection);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }));
            HubConnection.On("UploadFiles", async (string transferID, List<string> fileIDs, string requesterID) =>
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
                        var url = $"{ConnectionInfo.Host}/API/FileSharing/{fileID}";
                        var wr = WebRequest.CreateHttp(url);
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
                    await HubConnection.SendAsync("TransferCompleted", transferID, requesterID);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });
            HubConnection.On("DeployScript", async (string mode, string fileID, string commandResultID, string requesterID) =>
            {
                try
                {
                    if (!IsServerVerified)
                    {
                        Logger.Write($"Script deploy attempted before server was verified.  Mode: {mode}.  File ID: {fileID}.  Sender: {requesterID}", EventType.Warning);
                        return;
                    }

                    await ScriptRunner.RunScript(mode, fileID, commandResultID, requesterID, HubConnection);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });

            HubConnection.On("ReinstallAgent", async () =>
            {
                try
                {
                    await Updater.InstallLatestVersion();
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });

            HubConnection.On("UninstallAgent", () =>
            {
                try
                {
                    Uninstaller.UninstallAgent();
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });

            HubConnection.On("RemoteControl", async (string requesterID, string serviceID) =>
            {
                try
                {
                    if (!IsServerVerified)
                    {
                        Logger.Write("Remote control attempted before server was verified.", EventType.Warning);
                        return;
                    }
                    await AppLauncher.LaunchRemoteControl(-1, requesterID, serviceID, HubConnection);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });
            HubConnection.On("RestartScreenCaster", async (List<string> viewerIDs, string serviceID, string requesterID) =>
            {
                try
                {
                    if (!IsServerVerified)
                    {
                        Logger.Write("Remote control attempted before server was verified.", EventType.Warning);
                        return;
                    }
                    await AppLauncher.RestartScreenCaster(viewerIDs, serviceID, requesterID, HubConnection);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });
            HubConnection.On("CtrlAltDel", () =>
            {
                if (!IsServerVerified)
                {
                    Logger.Write("CtrlAltDel attempted before server was verified.", EventType.Warning);
                    return;
                }
                User32.SendSAS(false);
            });

            HubConnection.On("TriggerHeartbeat", async () =>
            {
                await SendHeartbeat();
            });

            HubConnection.On("ServerVerificationToken", (string verificationToken) =>
            {
                if (verificationToken == ConnectionInfo.ServerVerificationToken)
                {
                    IsServerVerified = true;
                }
                else
                {
                    Logger.Write($"Server sent an incorrect verification token.  Token Sent: {verificationToken}.", EventType.Warning);
                    return;
                }
            });
        }

    }
}
