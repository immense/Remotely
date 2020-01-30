using Remotely.Shared.Models;
using Remotely.Shared.Services;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Reflection;
using Remotely.Shared.Win32;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Remotely.Agent.Services
{
    public class DeviceSocket
    {
        public DeviceSocket(Updater updater, 
            ConfigService configService, 
            Uninstaller uninstaller, 
            CommandExecutor commandExecutor,
            ScriptRunner scriptRunner,
            AppLauncher appLauncher,
            ChatClientService chatService)
        {
            Updater = updater;
            ConfigService = configService;
            Uninstaller = uninstaller;
            CommandExecutor = commandExecutor;
            ScriptRunner = scriptRunner;
            AppLauncher = appLauncher;
            ChatService = chatService;
        }
        public bool IsConnected => HubConnection?.State == HubConnectionState.Connected;
        private AppLauncher AppLauncher { get; }
        private ChatClientService ChatService { get; }
        private CommandExecutor CommandExecutor { get; }
        private ConfigService ConfigService { get; }
        private ConnectionInfo ConnectionInfo { get; set; }
        private Timer HeartbeatTimer { get; set; }
        private HubConnection HubConnection { get; set; }
        private bool IsServerVerified { get; set; }
        private ScriptRunner ScriptRunner { get; }
        private Uninstaller Uninstaller { get; }
        private Updater Updater { get; }
        public async Task Connect()
        {
            ConnectionInfo = ConfigService.GetConnectionInfo();

            HubConnection = new HubConnectionBuilder()
                .WithUrl(ConnectionInfo.Host + "/DeviceHub")
                .AddMessagePackProtocol()
                .Build();

            RegisterMessageHandlers();

            await HubConnection.StartAsync();

            var device = Device.Create(ConnectionInfo);

            await HubConnection.InvokeAsync("DeviceCameOnline", device);

            if (string.IsNullOrWhiteSpace(ConnectionInfo.ServerVerificationToken))
            {
                IsServerVerified = true;
                ConnectionInfo.ServerVerificationToken = Guid.NewGuid().ToString();
                await HubConnection.InvokeAsync("SetServerVerificationToken", ConnectionInfo.ServerVerificationToken);
                ConfigService.SaveConnectionInfo(ConnectionInfo);
            }
            else
            {
                await HubConnection.InvokeAsync("SendServerVerificationToken");
            }

            if (ConfigService.TryGetDeviceSetupOptions(out DeviceSetupOptions options))
            {
                await HubConnection.InvokeAsync("DeviceSetupOptions", options, ConnectionInfo.DeviceID);
            }

            HeartbeatTimer?.Dispose();
            HeartbeatTimer = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
            HeartbeatTimer.Elapsed += HeartbeatTimer_Elapsed;
            HeartbeatTimer.Start();
        }

        public void SendHeartbeat()
        {
            var currentInfo = Device.Create(ConnectionInfo);
            HubConnection.InvokeAsync("DeviceHeartbeat", currentInfo);
        }

        private void HeartbeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SendHeartbeat();
        }

        private void RegisterMessageHandlers()
        {
            HubConnection.On("Chat", async (string message, string senderConnectionID) => {
                await ChatService.SendMessage(message, senderConnectionID, HubConnection);
            });
            HubConnection.On("ExecuteCommand", (async (string mode, string command, string commandID, string senderConnectionID) =>
            {
                if (!IsServerVerified)
                {
                    Logger.Write($"Command attempted before server was verified.  Mode: {mode}.  Command: {command}.  Sender: {senderConnectionID}");
                    Uninstaller.UninstallAgent();
                    return;
                }

                await CommandExecutor.ExecuteCommand(mode, command, commandID, senderConnectionID, HubConnection);
            }));
            HubConnection.On("TransferFiles", async (string transferID, List<string> fileIDs, string requesterID) =>
            {
                Logger.Write($"File transfer started by {requesterID}.");
                var sharedFilePath = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(),"RemotelySharedFiles")).FullName;
                
                foreach (var fileID in fileIDs)
                {
                    var url = $"{ConnectionInfo.Host}/API/FileSharing/{fileID}";
                    var wr = WebRequest.CreateHttp(url);
                    var response = await wr.GetResponseAsync();
                    var cd = response.Headers["Content-Disposition"];
                    var filename = cd
                                    .Split(";")
                                    .FirstOrDefault(x => x.Trim()
                                    .StartsWith("filename"))
                                    .Split("=")[1];

                    var legalChars = filename.ToCharArray().Where(x => !Path.GetInvalidFileNameChars().Any(y => x == y));

                    filename = new string(legalChars.ToArray());

                    using (var rs = response.GetResponseStream())
                    {
                        using (var fs = new FileStream(Path.Combine(sharedFilePath, filename), FileMode.Create))
                        {
                            rs.CopyTo(fs);
                        }
                    }
                }
                await this.HubConnection.InvokeAsync("TransferCompleted", transferID, requesterID);
            });
            HubConnection.On("DeployScript", async (string mode, string fileID, string commandContextID, string requesterID) => {
                if (!IsServerVerified)
                {
                    Logger.Write($"Script deploy attempted before server was verified.  Mode: {mode}.  File ID: {fileID}.  Sender: {requesterID}");
                    Uninstaller.UninstallAgent();
                    return;
                }

                await ScriptRunner.RunScript(mode, fileID, commandContextID, requesterID, HubConnection);
            });
            HubConnection.On("UninstallClient", () =>
            {
                Uninstaller.UninstallAgent();
            });
          
            HubConnection.On("RemoteControl", async (string requesterID, string serviceID) =>
            {
                if (!IsServerVerified)
                {
                    Logger.Write("Remote control attempted before server was verified.");
                    Uninstaller.UninstallAgent();
                    return;
                }
                await AppLauncher.LaunchRemoteControl(requesterID, serviceID, HubConnection);
            });
            HubConnection.On("RestartScreenCaster", async (List<string> viewerIDs, string serviceID, string requesterID) =>
            {
                if (!IsServerVerified)
                {
                    Logger.Write("Remote control attempted before server was verified.");
                    Uninstaller.UninstallAgent();
                    return;
                }
                await AppLauncher.RestartScreenCaster(viewerIDs, serviceID, requesterID, HubConnection);
            });
            HubConnection.On("CtrlAltDel", () =>
            {
                User32.SendSAS(false);
            });
          
            HubConnection.On("ServerVerificationToken", (string verificationToken) =>
            {
                if (verificationToken == ConnectionInfo.ServerVerificationToken)
                {
                    IsServerVerified = true;
                    if (!Program.IsDebug)
                    {
                        _ = Task.Run(Updater.CheckForUpdates);
                    }
                }
                else
                {
                    Logger.Write($"Server sent an incorrect verification token.  Token Sent: {verificationToken}.");
                    Uninstaller.UninstallAgent();
                    return;
                }
            });           
        }
    }
}
