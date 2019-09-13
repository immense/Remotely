using Remotely.Shared.Models;
using Remotely.Shared.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
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

namespace Remotely.Agent.Services
{
    public static class DeviceSocket
    {
        public static Timer HeartbeatTimer { get; private set; }
        public static bool IsServerVerified { get; set; }
        private static ConnectionInfo ConnectionInfo { get; set; }

        private static HubConnection HubConnection { get; set; }

        public static async Task Connect()
        {
            ConnectionInfo = Utilities.GetConnectionInfo();

            HubConnection = new HubConnectionBuilder()
                .WithUrl(ConnectionInfo.Host + "/DeviceHub")
                .Build();

            RegisterMessageHandlers(HubConnection);

            await HubConnection.StartAsync();

            var device = Device.Create(ConnectionInfo);

            await HubConnection.InvokeAsync("DeviceCameOnline", device);

            if (string.IsNullOrWhiteSpace(ConnectionInfo.ServerVerificationToken))
            {
                IsServerVerified = true;
                ConnectionInfo.ServerVerificationToken = Guid.NewGuid().ToString();
                await HubConnection.InvokeAsync("SetServerVerificationToken", ConnectionInfo.ServerVerificationToken);
                Utilities.SaveConnectionInfo(ConnectionInfo);
            }
            else
            {
                await HubConnection.InvokeAsync("SendServerVerificationToken");
            }

            if (HeartbeatTimer != null)
            {
                HeartbeatTimer.Stop();
            }
            HeartbeatTimer = new Timer(300000);
            HeartbeatTimer.Elapsed += HeartbeatTimer_Elapsed;
            HeartbeatTimer.Start();
        }

        public static bool IsConnected => HubConnection?.State == HubConnectionState.Connected;

        public static void SendHeartbeat()
        {
            var currentInfo = Device.Create(ConnectionInfo);
            HubConnection.InvokeAsync("DeviceHeartbeat", currentInfo);
        }

        private static async Task ExecuteCommand(string mode, string command, string commandID, string senderConnectionID)
        {
            if (!IsServerVerified)
            {
                Logger.Write($"Command attempted before server was verified.  Mode: {mode}.  Command: {command}.  Sender: {senderConnectionID}");
                Uninstaller.UninstallAgent();
                return;
            }
            try
            {
                switch (mode.ToLower())
                {
                    case "pscore":
                        {
                            var psCoreResult = PSCore.GetCurrent(senderConnectionID).WriteInput(command, commandID);
                            var serializedResult = JsonConvert.SerializeObject(psCoreResult);
                            if (Encoding.UTF8.GetBytes(serializedResult).Length > 400000)
                            {
                                SendResultsViaAjax("PSCore", psCoreResult);
                                await HubConnection.InvokeAsync("PSCoreResultViaAjax", commandID);
                            }
                            else
                            {
                                await HubConnection.InvokeAsync("PSCoreResult", psCoreResult);
                            }
                            break;
                        }

                    case "winps":
                        if (OSUtils.IsWindows)
                        {
                            var result = WindowsPS.GetCurrent(senderConnectionID).WriteInput(command, commandID);
                            var serializedResult = JsonConvert.SerializeObject(result);
                            if (Encoding.UTF8.GetBytes(serializedResult).Length > 400000)
                            {
                                SendResultsViaAjax("WinPS", result);
                                await HubConnection.InvokeAsync("WinPSResultViaAjax", commandID);
                            }
                            else
                            {
                                await HubConnection.InvokeAsync("CommandResult", result);
                            }
                        }
                        break;
                    case "cmd":
                        if (OSUtils.IsWindows)
                        {
                            var result = CMD.GetCurrent(senderConnectionID).WriteInput(command, commandID);
                            var serializedResult = JsonConvert.SerializeObject(result);
                            if (Encoding.UTF8.GetBytes(serializedResult).Length > 400000)
                            {
                                SendResultsViaAjax("CMD", result);
                                await HubConnection.InvokeAsync("CMDResultViaAjax", commandID);
                            }
                            else
                            {
                                await HubConnection.InvokeAsync("CommandResult", result);
                            }
                        }
                        break;
                    case "bash":
                        if (OSUtils.IsLinux)
                        {
                            var result = Bash.GetCurrent(senderConnectionID).WriteInput(command, commandID);
                            var serializedResult = JsonConvert.SerializeObject(result);
                            if (Encoding.UTF8.GetBytes(serializedResult).Length > 400000)
                            {
                                SendResultsViaAjax("Bash", result);
                            }
                            else
                            {
                                await HubConnection.InvokeAsync("CommandResult", result);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                await HubConnection.InvokeAsync("DisplayConsoleMessage", $"There was an error executing the command.  It has been logged on the client device.", senderConnectionID);
            }
        }

        private static void HeartbeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SendHeartbeat();
        }

        private static void RegisterMessageHandlers(HubConnection hubConnection)
        {
            hubConnection.On("ExecuteCommand", (async (string mode, string command, string commandID, string senderConnectionID) =>
            {
                await ExecuteCommand(mode, command, commandID, senderConnectionID);
            }));
            hubConnection.On("TransferFiles", async (string transferID, List<string> fileIDs, string requesterID) =>
            {
                Logger.Write($"File transfer started by {requesterID}.");
                var connectionInfo = Utilities.GetConnectionInfo();
                var sharedFilePath = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(),"RemotelySharedFiles")).FullName;
                
                foreach (var fileID in fileIDs)
                {
                    var url = $"{connectionInfo.Host}/API/FileSharing/{fileID}";
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
                await HubConnection.InvokeAsync("TransferCompleted", transferID, requesterID);
            });
            hubConnection.On("DeployScript", async (string mode, string fileID, string commandContextID, string requesterID) => {
                var connectionInfo = Utilities.GetConnectionInfo();
                var sharedFilePath = Directory.CreateDirectory(Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        "Remotely",
                        "SharedFiles"
                    )).FullName;
                var webClient = new WebClient();

                var url = $"{connectionInfo.Host}/API/FileSharing/{fileID}";
                var wr = WebRequest.CreateHttp(url);
                var response = await wr.GetResponseAsync();
                var cd = response.Headers["Content-Disposition"];
                var filename = cd.Split(";").FirstOrDefault(x => x.Trim().StartsWith("filename")).Split("=")[1];
                using (var rs = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(rs))
                    {
                        var result = await sr.ReadToEndAsync();
                        await ExecuteCommand(mode, result, commandContextID, requesterID);
                    }
                }
            });
            hubConnection.On("UninstallClient", () =>
            {
                Uninstaller.UninstallAgent();
            });
          
            hubConnection.On("RemoteControl", async (string requesterID, string serviceID) =>
            {
                if (!IsServerVerified)
                {
                    Logger.Write("Remote control attempted before server was verified.");
                    Uninstaller.UninstallAgent();
                    return;
                }
                try
                {
                    var rcBinaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScreenCast", OSUtils.ScreenCastExecutableFileName);
                    if (!File.Exists(rcBinaryPath))
                    {
                        await hubConnection.InvokeAsync("DisplayConsoleMessage", "Remote control executable not found on target device.", requesterID);
                        return;
                    }


                    // Start ScreenCast.
                    await hubConnection.InvokeAsync("DisplayConsoleMessage", $"Starting remote control...", requesterID);
                    if (OSUtils.IsWindows)
                    {

                        if (Program.IsDebug)
                        {
                            Process.Start(rcBinaryPath, $"-mode Unattended -requester {requesterID} -serviceid {serviceID} -deviceid {ConnectionInfo.DeviceID} -host {Utilities.GetConnectionInfo().Host} -desktop default");
                        }
                        else
                        {
                            var result = Win32Interop.OpenInteractiveProcess(rcBinaryPath + $" -mode Unattended -requester {requesterID} -serviceid {serviceID} -deviceid {ConnectionInfo.DeviceID} -host {Utilities.GetConnectionInfo().Host} -desktop default", "default", true, out _);
                            if (!result)
                            {
                                await hubConnection.InvokeAsync("DisplayConsoleMessage", "Remote control failed to start on target device.", requesterID);
                            }
                        }
                    }
                    else if (OSUtils.IsLinux)
                    {
                        var users = OSUtils.StartProcessWithResults("users", "");
                        var username = users?.Split()?.FirstOrDefault()?.Trim();
                        var homeDir = OSUtils.StartProcessWithResults("sudo", $"-u {username} env | grep HOME")?.Split('=')?.Last();
                        var psi = new ProcessStartInfo()
                        {
                            FileName = "sudo",
                            Arguments = $"-u {username} {rcBinaryPath} -mode Unattended -requester {requesterID} -serviceid {serviceID} -deviceid {ConnectionInfo.DeviceID} -host {Utilities.GetConnectionInfo().Host} -desktop default & disown"
                        };
                        psi.Environment.Add("DISPLAY", ":0");
                        psi.Environment.Add("XAUTHORITY", $"{homeDir}/.Xauthority");
                        var casterProc = Process.Start(psi);
                        casterProc.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                    await hubConnection.InvokeAsync("DisplayConsoleMessage", "Remote control failed to start on target device.", requesterID);
                    throw;
                }
            });
            hubConnection.On("RestartScreenCaster", async (List<string> viewerIDs, string serviceID, string requesterID) =>
            {
                if (!IsServerVerified)
                {
                    Logger.Write("Remote control attempted before server was verified.");
                    Uninstaller.UninstallAgent();
                    return;
                }
                try
                {
                    var rcBinaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScreenCast", OSUtils.ScreenCastExecutableFileName);
                    // Start ScreenCast.                 
                    if (OSUtils.IsWindows)
                    {
                        Logger.Write("Restarting screen caster.");
                        if (Program.IsDebug)
                        {
                            var proc = Process.Start(rcBinaryPath, $"-mode Unattended -requester {requesterID} -serviceid {serviceID} -deviceid {ConnectionInfo.DeviceID} -host {Utilities.GetConnectionInfo().Host} -relaunch true -desktop default -viewers {String.Join(",", viewerIDs)}");
                            var stopwatch = Stopwatch.StartNew();
                            while (stopwatch.Elapsed.TotalSeconds < 10)
                            {
                                await Task.Delay(250);
                                if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(rcBinaryPath))?.Where(x => x.Id == proc.Id)?.Count() > 0 != true)
                                {
                                    Logger.Write("Restarting screen caster after failed relaunch.");
                                }
                            }
                        }
                        else
                        {
                            var result = Win32Interop.OpenInteractiveProcess(rcBinaryPath + $" -mode Unattended -requester {requesterID} -serviceid {serviceID} -deviceid {ConnectionInfo.DeviceID} -host {Utilities.GetConnectionInfo().Host} -relaunch true -desktop default -viewers {String.Join(",", viewerIDs)}", "default", true, out var procInfo);
                            
                            if (result)
                            {
                                // This relaunch might have been prompted by a user logging out, which would close
                                // the screencaster process.  In that scenario, the relaunched process can get closed again
                                // while the Windows sign-out process is still occurring.  We'll wait a bit to make sure the
                                // relaunched process is still running.  If not, launch again.
                                var stopwatch = Stopwatch.StartNew();
                                while (stopwatch.Elapsed.TotalSeconds < 10)
                                {
                                    await Task.Delay(250);
                                    if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(rcBinaryPath))?.Where(x=>x.Id == procInfo.dwProcessId)?.Count() > 0 != true)
                                    {
                                        Logger.Write("Restarting screen caster after failed relaunch.");
                                        result = Win32Interop.OpenInteractiveProcess(rcBinaryPath + $" -mode Unattended -requester {requesterID} -serviceid {serviceID} -deviceid {ConnectionInfo.DeviceID} -host {Utilities.GetConnectionInfo().Host} -relaunch true -desktop default -viewers {String.Join(",", viewerIDs)}", "default", true, out procInfo);
                                    }
                                }
                            }
                 
                            if (!result)
                            {
                                Logger.Write("Failed to relaunch screen caster.");
                                await hubConnection.InvokeAsync("SendConnectionFailedToViewers", viewerIDs);
                                await hubConnection.InvokeAsync("DisplayConsoleMessage", "Remote control failed to start on target device.", requesterID);
                            }
                        }
                    }
                    else if (OSUtils.IsLinux)
                    {
                        var users = OSUtils.StartProcessWithResults("users", "");
                        var username = users?.Split()?.FirstOrDefault()?.Trim();
                        var homeDir = OSUtils.StartProcessWithResults("sudo", $"-u {username} env | grep HOME")?.Split('=')?.Last();
                        var psi = new ProcessStartInfo()
                        {
                            FileName = "sudo",
                            Arguments = $"-u {username} {rcBinaryPath} -mode Unattended -requester {requesterID} -serviceid {serviceID} -deviceid {ConnectionInfo.DeviceID} -host {Utilities.GetConnectionInfo().Host} -relaunch true -desktop default -viewers {String.Join(",", viewerIDs)} & disown"
                        };
                        psi.Environment.Add("DISPLAY", ":0");
                        psi.Environment.Add("XAUTHORITY", $"{homeDir}/.Xauthority");
                        var casterProc = Process.Start(psi);
                        casterProc.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    await hubConnection.InvokeAsync("SendConnectionFailedToViewers", viewerIDs);
                    Logger.Write(ex);
                    throw;
                }
            });
            hubConnection.On("CtrlAltDel", () =>
            {
                User32.SendSAS(false);
            });
          
            hubConnection.On("ServerVerificationToken", (string verificationToken) =>
            {
                if (verificationToken == Utilities.GetConnectionInfo().ServerVerificationToken)
                {
                    IsServerVerified = true;
                    if (!Program.IsDebug)
                    {
                        Updater.CheckForCoreUpdates();
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

        private static void SendResultsViaAjax(string resultType, object result)
        {
            var targetURL = Utilities.GetConnectionInfo().Host + $"/API/Commands/{resultType}";
            var webRequest = WebRequest.CreateHttp(targetURL);
            webRequest.Method = "POST";

            using (var sw = new StreamWriter(webRequest.GetRequestStream()))
            {
                sw.Write(JsonConvert.SerializeObject(result));
            }
            webRequest.GetResponse();
        }
    }
}
