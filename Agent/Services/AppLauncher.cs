using Microsoft.AspNetCore.SignalR.Client;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using Remotely.Shared.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Agent.Services
{
    public class AppLauncher
    {
        public AppLauncher(ConfigService configService)
        {
            ConnectionInfo = configService.GetConnectionInfo();
        }

        private ConnectionInfo ConnectionInfo { get; }

        public async Task<int> LaunchChatService(string orgName, string requesterID, HubConnection hubConnection)
        {
            try
            {
                var rcBinaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScreenCast", EnvironmentHelper.ScreenCastExecutableFileName);
                if (!File.Exists(rcBinaryPath))
                {
                    await hubConnection.SendAsync("DisplayMessage", "Chat executable not found on target device.", "Executable not found on device.", requesterID);
                }


                // Start ScreenCast.
                await hubConnection.SendAsync("DisplayMessage", $"Starting chat service...", "Starting chat service.", requesterID);
                if (EnvironmentHelper.IsWindows)
                {

                    if (EnvironmentHelper.IsDebug)
                    {
                        return Process.Start(rcBinaryPath, $"-mode Chat -requester \"{requesterID}\" -organization \"{orgName}\"").Id;
                    }
                    else
                    {
                        var result = Win32Interop.OpenInteractiveProcess($"{rcBinaryPath} -mode Chat -requester \"{requesterID}\" -organization \"{orgName}\"", 
                            targetSessionId: -1,
                            forceConsoleSession: false,
                            desktopName: "default", 
                            hiddenWindow: false, 
                            out var procInfo);
                        if (!result)
                        {
                            await hubConnection.SendAsync("DisplayMessage", "Remote control failed to start on target device.", "Failed to start remote control.", requesterID);
                        }
                        else
                        {
                            return procInfo.dwProcessId;
                        }
                    }
                }
                else if (EnvironmentHelper.IsLinux)
                {
                    var args = $"xterm -e {rcBinaryPath} -mode Chat -requester \"{requesterID}\" -organization \"{orgName}\" & disown";
                    return StartLinuxScreenCaster(args);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                await hubConnection.SendAsync("DisplayMessage", "Remote control failed to start on target device.", "Failed to start remote control.", requesterID);
            }
            return -1;
        }

        public async Task LaunchRemoteControl(int targetSessionId, string requesterID, string serviceID, HubConnection hubConnection)
        {
            try
            {
                var rcBinaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScreenCast", EnvironmentHelper.ScreenCastExecutableFileName);
                if (!File.Exists(rcBinaryPath))
                {
                    await hubConnection.SendAsync("DisplayMessage", "Remote control executable not found on target device.", "Executable not found on device.", requesterID);
                    return;
                }


                // Start ScreenCast.
                await hubConnection.SendAsync("DisplayMessage", $"Starting remote control...", "Starting remote control.", requesterID);
                if (EnvironmentHelper.IsWindows)
                {

                    if (EnvironmentHelper.IsDebug)
                    {
                        Process.Start(rcBinaryPath, $"-mode Unattended -requester \"{requesterID}\" -serviceid \"{serviceID}\" -deviceid {ConnectionInfo.DeviceID} -host {ConnectionInfo.Host}");
                    }
                    else
                    {
                        var result = Win32Interop.OpenInteractiveProcess(rcBinaryPath + $" -mode Unattended -requester \"{requesterID}\" -serviceid \"{serviceID}\" -deviceid {ConnectionInfo.DeviceID} -host {ConnectionInfo.Host}",
                            targetSessionId: targetSessionId,
                            forceConsoleSession: false,
                            desktopName: "default",
                            hiddenWindow: true,
                            out _);
                        if (!result)
                        {
                            await hubConnection.SendAsync("DisplayMessage", "Remote control failed to start on target device.", "Failed to start remote control.", requesterID);
                        }
                    }
                }
                else if (EnvironmentHelper.IsLinux)
                {
                    var args = $"{rcBinaryPath} -mode Unattended -requester \"{requesterID}\" -serviceid \"{serviceID}\" -deviceid {ConnectionInfo.DeviceID} -host {ConnectionInfo.Host} & disown";
                    StartLinuxScreenCaster(args);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                await hubConnection.SendAsync("DisplayMessage", "Remote control failed to start on target device.", "Failed to start remote control.", requesterID);
            }
        }
        public async Task RestartScreenCaster(List<string> viewerIDs, string serviceID, string requesterID, HubConnection hubConnection, int targetSessionID = -1)
        {
            try
            {
                var rcBinaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScreenCast", EnvironmentHelper.ScreenCastExecutableFileName);
                // Start ScreenCast.                 
                if (EnvironmentHelper.IsWindows)
                {
                    Logger.Write("Restarting screen caster.");
                    if (EnvironmentHelper.IsDebug)
                    {
                        Process.Start(rcBinaryPath, $"-mode Unattended -requester \"{requesterID}\" -serviceid \"{serviceID}\" -deviceid {ConnectionInfo.DeviceID} -host {ConnectionInfo.Host} -relaunch true -viewers {String.Join(",", viewerIDs)}");
                    }
                    else
                    {

                        // Give a little time for session changing, etc.
                        await Task.Delay(1000);

                        var result = Win32Interop.OpenInteractiveProcess(rcBinaryPath + $" -mode Unattended -requester \"{requesterID}\" -serviceid \"{serviceID}\" -deviceid {ConnectionInfo.DeviceID} -host {ConnectionInfo.Host} -relaunch true -viewers {String.Join(",", viewerIDs)}",
                            targetSessionId: targetSessionID,
                            forceConsoleSession: Shlwapi.IsOS(OsType.OS_ANYSERVER) && targetSessionID == -1 ? true : false,
                            desktopName: "default",
                            hiddenWindow: true,
                            out _);

                        if (!result)
                        {
                            Logger.Write("Failed to relaunch screen caster.");
                            await hubConnection.SendAsync("SendConnectionFailedToViewers", viewerIDs);
                            await hubConnection.SendAsync("DisplayMessage", "Remote control failed to start on target device.", "Failed to start remote control.", requesterID);
                        }
                    }
                }
                else if (EnvironmentHelper.IsLinux)
                {
                    var args = $"{rcBinaryPath} -mode Unattended -requester \"{requesterID}\" -serviceid \"{serviceID}\" -deviceid {ConnectionInfo.DeviceID} -host {ConnectionInfo.Host} -relaunch true -viewers {string.Join(",", viewerIDs)} & disown";
                    StartLinuxScreenCaster(args);
                }
            }
            catch (Exception ex)
            {
                await hubConnection.SendAsync("SendConnectionFailedToViewers", viewerIDs);
                Logger.Write(ex);
                throw;
            }
        }

        private int StartLinuxScreenCaster(string args)
        {
            var xauthority = string.Empty;

            var processes = EnvironmentHelper.StartProcessWithResults("ps", "-eaf").Split(Environment.NewLine);
            var xorgLine = processes.FirstOrDefault(x => x.Contains("xorg"));
            var xorgSplit = xorgLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            var auth = xorgSplit[xorgSplit.IndexOf("-auth") + 1];
            if (!string.IsNullOrWhiteSpace(auth))
            {
                xauthority = auth;
            }

            var display = ":0";
            var whoString = EnvironmentHelper.StartProcessWithResults("w", "-h")?.Trim();
            var username = string.Empty;

            if (!string.IsNullOrWhiteSpace(whoString))
            {
                try
                {
                    var whoLine = whoString
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .First();

                    var whoSplit = whoLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    username = whoSplit[0];
                    display = whoSplit[2];
                    xauthority = $"/home/{username}/.Xauthority";
                    args = $"-u {username} {args}";
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }

            var psi = new ProcessStartInfo()
            {
                FileName = "sudo",
                Arguments = args
            };

            psi.Environment.Add("DISPLAY", display);
            psi.Environment.Add("XAUTHORITY", xauthority);
            Logger.Write($"Attempting to launch screen caster with username {username}, xauthority {xauthority}, display {display}, and args {args}.");
            return Process.Start(psi).Id;
        }
    }
}
