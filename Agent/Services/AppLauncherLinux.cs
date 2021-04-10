using Microsoft.AspNetCore.SignalR.Client;
using Remotely.Agent.Interfaces;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Agent.Services
{

    public class AppLauncherLinux : IAppLauncher
    {
        private readonly string _rcBinaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Desktop", EnvironmentHelper.DesktopExecutableFileName);

        public AppLauncherLinux(ConfigService configService)
        {
            ConnectionInfo = configService.GetConnectionInfo();
        }

        private ConnectionInfo ConnectionInfo { get; }

        public async Task<int> LaunchChatService(string orgName, string requesterID, HubConnection hubConnection)
        {
            try
            {
                if (!File.Exists(_rcBinaryPath))
                {
                    await hubConnection.SendAsync("DisplayMessage", 
                        "Chat executable not found on target device.", 
                        "Executable not found on device.", 
                        "bg-danger",
                        requesterID);
                }


                // Start Desktop app.
                await hubConnection.SendAsync("DisplayMessage", $"Starting chat service.", "Starting chat service.", "bg-success", requesterID);
                var args = $"{_rcBinaryPath} " +
                    $"-mode Chat " +
                    $"-requester \"{requesterID}\" " +
                    $"-organization \"{orgName}\" " +
                    $"-host \"{ConnectionInfo.Host}\" " +
                    $"-orgid \"{ConnectionInfo.OrganizationID}\"";
                return StartLinuxDesktopApp(args);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                await hubConnection.SendAsync("DisplayMessage", "Chat service failed to start on target device.", "Failed to start chat service.", "bg-danger", requesterID);
            }
            return -1;
        }

        public async Task LaunchRemoteControl(int targetSessionId, string requesterID, string serviceID, HubConnection hubConnection)
        {
            try
            {
                if (!File.Exists(_rcBinaryPath))
                {
                    await hubConnection.SendAsync("DisplayMessage",
                        "Remote control executable not found on target device.", 
                        "Executable not found on device.", 
                        "bg-danger", 
                        requesterID);
                    return;
                }


                // Start Desktop app.
                await hubConnection.SendAsync("DisplayMessage", "Starting remote control.", "Starting remote control.",  "bg-success", requesterID);
                var args = $"{_rcBinaryPath} " +
                    $"-mode Unattended " +
                    $"-requester \"{requesterID}\" " +
                    $"-serviceid \"{serviceID}\" " +
                    $"-deviceid {ConnectionInfo.DeviceID} " +
                    $"-host \"{ConnectionInfo.Host}\" " +
                    $"-orgid \"{ConnectionInfo.OrganizationID}\"";
                StartLinuxDesktopApp(args);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                await hubConnection.SendAsync("DisplayMessage", "Remote control failed to start on target device.", "Failed to start remote control.", "bg-danger", requesterID);
            }
        }
        public async Task RestartScreenCaster(List<string> viewerIDs, string serviceID, string requesterID, HubConnection hubConnection, int targetSessionID = -1)
        {
            try
            {
                // Start Desktop app.                 
                var args = $"{_rcBinaryPath} " +
                    $"-mode Unattended " +
                    $"-requester \"{requesterID}\" " +
                    $"-serviceid \"{serviceID}\" " +
                    $"-deviceid {ConnectionInfo.DeviceID} " +
                    $"-host \"{ConnectionInfo.Host}\" " +
                    $"-orgid \"{ConnectionInfo.OrganizationID}\" " +
                    $"-relaunch true " +
                    $"-viewers {string.Join(",", viewerIDs)}";
                StartLinuxDesktopApp(args);
            }
            catch (Exception ex)
            {
                await hubConnection.SendAsync("SendConnectionFailedToViewers", viewerIDs);
                Logger.Write(ex);
                throw;
            }
        }

        private int StartLinuxDesktopApp(string args)
        {
            var xauthority = GetXorgAuth();

            var display = ":0";
            var whoString = EnvironmentHelper.StartProcessWithResults("who", "")?.Trim();
            var username = "";

            if (!string.IsNullOrWhiteSpace(whoString))
            {
                try
                {
                    var whoLine = whoString
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .First();

                    var whoSplit = whoLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    username = whoSplit[0];
                    display = whoSplit.Last().TrimStart('(').TrimEnd(')');
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

        private string GetXorgAuth()
        {
            try
            {
                var processes = EnvironmentHelper.StartProcessWithResults("ps", "-eaf")?.Split(Environment.NewLine);
                if (processes?.Length > 0)
                {
                    var xorgLine = processes.FirstOrDefault(x => x.Contains("xorg", StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrWhiteSpace(xorgLine))
                    {
                        var xorgSplit = xorgLine?.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                        var authIndex = xorgSplit?.IndexOf("-auth");
                        if (authIndex > -1 && xorgSplit?.Count >= authIndex + 1)
                        {
                            var auth = xorgSplit[(int)authIndex + 1];
                            if (!string.IsNullOrWhiteSpace(auth))
                            {
                                return auth;
                            }
                        }
                    }
                }
            }
            catch { }
            return string.Empty;
        }
    }
}
