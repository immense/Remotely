using Microsoft.AspNetCore.SignalR.Client;
using Remotely.Shared.Models;
using Remotely.Shared.Services;
using Remotely.Shared.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

        public async Task<int> LaunchChatService(string requesterID, HubConnection hubConnection)
        {
            try
            {
                var rcBinaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScreenCast", OSUtils.ScreenCastExecutableFileName);
                if (!File.Exists(rcBinaryPath))
                {
                    await hubConnection.InvokeAsync("DisplayMessage", "Chat executable not found on target device.", "Executable not found on device.", requesterID);
                }


                // Start ScreenCast.
                await hubConnection.InvokeAsync("DisplayMessage", $"Starting chat service...", "Starting chat service.", requesterID);
                if (OSUtils.IsWindows)
                {

                    if (Program.IsDebug)
                    {
                        return Process.Start("conhost.exe", $"{rcBinaryPath} -mode Chat -requester {requesterID}").Id;
                    }
                    else
                    {
                        var result = Win32Interop.OpenInteractiveProcess($"{rcBinaryPath} -mode Chat -requester {requesterID}", "default", false, out var procInfo);
                        if (!result)
                        {
                            await hubConnection.InvokeAsync("DisplayMessage", "Remote control failed to start on target device.", "Failed to start remote control.", requesterID);
                        }
                        else
                        {
                            return procInfo.dwProcessId;
                        }
                    }
                }
                else if (OSUtils.IsLinux)
                {
                    var args = $"xterm -e {rcBinaryPath} -mode Chat -requester {requesterID} & disown";
                    return StartLinuxScreenCaster(args);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                await hubConnection.InvokeAsync("DisplayMessage", "Remote control failed to start on target device.", "Failed to start remote control.", requesterID);
            }
            return -1;
        }

        public async Task LaunchRemoteControl(string requesterID, string serviceID, HubConnection hubConnection)
        {
            try
            {
                var rcBinaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScreenCast", OSUtils.ScreenCastExecutableFileName);
                if (!File.Exists(rcBinaryPath))
                {
                    await hubConnection.InvokeAsync("DisplayMessage", "Remote control executable not found on target device.", "Executable not found on device.", requesterID);
                    return;
                }


                // Start ScreenCast.
                await hubConnection.InvokeAsync("DisplayMessage", $"Starting remote control...", "Starting remote control.", requesterID);
                if (OSUtils.IsWindows)
                {

                    if (Program.IsDebug)
                    {
                        Process.Start(rcBinaryPath, $"-mode Unattended -requester {requesterID} -serviceid {serviceID} -deviceid {ConnectionInfo.DeviceID} -host {ConnectionInfo.Host}");
                    }
                    else
                    {
                        var result = Win32Interop.OpenInteractiveProcess(rcBinaryPath + $" -mode Unattended -requester {requesterID} -serviceid {serviceID} -deviceid {ConnectionInfo.DeviceID} -host {ConnectionInfo.Host}", "default", true, out _);
                        if (!result)
                        {
                            await hubConnection.InvokeAsync("DisplayMessage", "Remote control failed to start on target device.", "Failed to start remote control.", requesterID);
                        }
                    }
                }
                else if (OSUtils.IsLinux)
                {
                    var args = $"{rcBinaryPath} -mode Unattended -requester {requesterID} -serviceid {serviceID} -deviceid {ConnectionInfo.DeviceID} -host {ConnectionInfo.Host} & disown";
                    StartLinuxScreenCaster(args);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                await hubConnection.InvokeAsync("DisplayMessage", "Remote control failed to start on target device.", "Failed to start remote control.", requesterID);
            }
        }
        public async Task RestartScreenCaster(List<string> viewerIDs, string serviceID, string requesterID, HubConnection hubConnection)
        {
            try
            {
                var rcBinaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScreenCast", OSUtils.ScreenCastExecutableFileName);
                // Start ScreenCast.                 
                if (OSUtils.IsWindows)
                {
                    Logger.Write("Restarting screen caster.");
                    if (Program.IsDebug)
                    {
                        Process.Start(rcBinaryPath, $"-mode Unattended -requester {requesterID} -serviceid {serviceID} -deviceid {ConnectionInfo.DeviceID} -host {ConnectionInfo.Host} -relaunch true -viewers {String.Join(",", viewerIDs)}");
                    }
                    else
                    {

                        // Give a little time for session changing, etc.
                        await Task.Delay(1000);

                        var result = Win32Interop.OpenInteractiveProcess(rcBinaryPath + $" -mode Unattended -requester {requesterID} -serviceid {serviceID} -deviceid {ConnectionInfo.DeviceID} -host {ConnectionInfo.Host} -relaunch true -viewers {String.Join(",", viewerIDs)}", "default", true, out _);

                        if (!result)
                        {
                            await Task.Delay(1000);
                            // Try one more time.
                            result = Win32Interop.OpenInteractiveProcess(rcBinaryPath + $" -mode Unattended -requester {requesterID} -serviceid {serviceID} -deviceid {ConnectionInfo.DeviceID} -host {ConnectionInfo.Host} -relaunch true -viewers {String.Join(",", viewerIDs)}", "default", true, out _);

                            if (!result)
                            {
                                Logger.Write("Failed to relaunch screen caster.");
                                await hubConnection.InvokeAsync("SendConnectionFailedToViewers", viewerIDs);
                                await hubConnection.InvokeAsync("DisplayMessage", "Remote control failed to start on target device.", "Failed to start remote control.", requesterID);
                            }
                        }
                    }
                }
                else if (OSUtils.IsLinux)
                {
                    var args = $"{rcBinaryPath} -mode Unattended -requester {requesterID} -serviceid {serviceID} -deviceid {ConnectionInfo.DeviceID} -host {ConnectionInfo.Host} -relaunch true -viewers {string.Join(",", viewerIDs)} & disown";
                    StartLinuxScreenCaster(args);
                }
            }
            catch (Exception ex)
            {
                await hubConnection.InvokeAsync("SendConnectionFailedToViewers", viewerIDs);
                Logger.Write(ex);
                throw;
            }
        }

        private int StartLinuxScreenCaster(string args)
        {
            var xauthority = OSUtils.StartProcessWithResults("find", $"/ -name Xauthority").Split('\n', StringSplitOptions.RemoveEmptyEntries).First();
            var display = ":0";
            var whoString = OSUtils.StartProcessWithResults("who", "")?.Trim();
            var username = string.Empty;

            if (!string.IsNullOrWhiteSpace(whoString))
            {
                var whoLine = whoString.Split('\n', StringSplitOptions.RemoveEmptyEntries).First();
                var whoSplit = whoLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                username = whoSplit[0];
                display = whoSplit.Last().Trim('(').Trim(')');
                args = $"-u {username} {args}";
            }

            var psi = new ProcessStartInfo()
            {
                FileName = "sudo",
                Arguments = args
            };

            psi.Environment.Add("DISPLAY", display);
            psi.Environment.Add("XAUTHORITY", xauthority);
            Logger.Write($"Attempting to launch screen caster with username {username}, xauthority {xauthority}, and display {display}.");
            return Process.Start(psi).Id;
        }
    }
}
