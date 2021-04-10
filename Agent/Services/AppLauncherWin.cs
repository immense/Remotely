using Microsoft.AspNetCore.SignalR.Client;
using Remotely.Agent.Interfaces;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using Remotely.Shared.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Remotely.Agent.Services
{

    public class AppLauncherWin : IAppLauncher
    {
        private readonly string _rcBinaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Desktop", EnvironmentHelper.DesktopExecutableFileName);

        public AppLauncherWin(ConfigService configService)
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
                    await hubConnection.SendAsync("DisplayMessage", "Chat executable not found on target device.", "Executable not found on device.", "bg-danger", requesterID);
                }


                // Start Desktop app.
                await hubConnection.SendAsync("DisplayMessage", $"Starting chat service.", "Starting chat service.", "bg-success", requesterID);
                if (WindowsIdentity.GetCurrent().IsSystem)
                {
                    var result = Win32Interop.OpenInteractiveProcess($"{_rcBinaryPath} " +
                            $"-mode Chat " +
                            $"-requester \"{requesterID}\" " +
                            $"-organization \"{orgName}\" " +
                            $"-host \"{ConnectionInfo.Host}\" " +
                            $"-orgid \"{ConnectionInfo.OrganizationID}\"",
                        targetSessionId: -1,
                        forceConsoleSession: false,
                        desktopName: "default",
                        hiddenWindow: false,
                        out var procInfo);
                    if (!result)
                    {
                        await hubConnection.SendAsync("DisplayMessage", 
                            "Chat service failed to start on target device.", 
                            "Failed to start chat service.", 
                            "bg-danger",
                            requesterID);
                    }
                    else
                    {
                        return procInfo.dwProcessId;
                    }
                }
                else
                {
                    return Process.Start(_rcBinaryPath, 
                        $"-mode Chat " +
                        $"-requester \"{requesterID}\" " +
                        $"-organization \"{orgName}\" " +
                         $"-host \"{ConnectionInfo.Host}\" " +
                        $"-orgid \"{ConnectionInfo.OrganizationID}\"").Id;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                await hubConnection.SendAsync("DisplayMessage", 
                    "Chat service failed to start on target device.",
                    "Failed to start chat service.",
                    "bg-danger",
                    requesterID);
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
                await hubConnection.SendAsync("DisplayMessage", 
                    "Starting remote control.",
                    "Starting remote control.",
                    "bg-success",
                    requesterID);
                if (WindowsIdentity.GetCurrent().IsSystem)
                {
                    var result = Win32Interop.OpenInteractiveProcess(_rcBinaryPath +
                            $" -mode Unattended" +
                            $" -requester \"{requesterID}\"" +
                            $" -serviceid \"{serviceID}\"" +
                            $" -deviceid {ConnectionInfo.DeviceID}" +
                            $" -host {ConnectionInfo.Host}" +
                            $" -orgid \"{ConnectionInfo.OrganizationID}\"",
                        targetSessionId: targetSessionId,
                        forceConsoleSession: Shlwapi.IsOS(OsType.OS_ANYSERVER) && targetSessionId == -1,
                        desktopName: "default",
                        hiddenWindow: true,
                        out _);
                    if (!result)
                    {
                        await hubConnection.SendAsync("DisplayMessage", 
                            "Remote control failed to start on target device.",
                            "Failed to start remote control.",
                            "bg-danger",
                            requesterID);
                    }
                }
                else
                {
                    // SignalR Connection IDs might start with a hyphen.  We surround them
                    // with quotes so the command line will be parsed correctly.
                    Process.Start(_rcBinaryPath, $"-mode Unattended " +
                        $"-requester \"{requesterID}\" " +
                        $"-serviceid \"{serviceID}\" " +
                        $"-deviceid {ConnectionInfo.DeviceID} " +
                        $"-host {ConnectionInfo.Host} " +
                        $"-orgid \"{ConnectionInfo.OrganizationID}\"");
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                await hubConnection.SendAsync("DisplayMessage", 
                    "Remote control failed to start on target device.", 
                    "Failed to start remote control.",
                    "bg-danger",
                    requesterID);
            }
        }
        public async Task RestartScreenCaster(List<string> viewerIDs, string serviceID, string requesterID, HubConnection hubConnection, int targetSessionID = -1)
        {
            try
            {
                // Start Desktop app.                 
                Logger.Write("Restarting screen caster.");
                if (WindowsIdentity.GetCurrent().IsSystem)
                {
                    // Give a little time for session changing, etc.
                    await Task.Delay(1000);

                    var result = Win32Interop.OpenInteractiveProcess(_rcBinaryPath + 
                            $" -mode Unattended" +
                            $" -requester \"{requesterID}\"" +
                            $" -serviceid \"{serviceID}\"" +
                            $" -deviceid {ConnectionInfo.DeviceID}" +
                            $" -host {ConnectionInfo.Host}" +
                            $" -orgid \"{ConnectionInfo.OrganizationID}\"" +
                            $" -relaunch true" +
                            $" -viewers {String.Join(",", viewerIDs)}",

                        targetSessionId: targetSessionID,
                        forceConsoleSession: Shlwapi.IsOS(OsType.OS_ANYSERVER) && targetSessionID == -1,
                        desktopName: "default",
                        hiddenWindow: true,
                        out _);

                    if (!result)
                    {
                        Logger.Write("Failed to relaunch screen caster.");
                        await hubConnection.SendAsync("SendConnectionFailedToViewers", viewerIDs);
                        await hubConnection.SendAsync("DisplayMessage", 
                            "Remote control failed to start on target device.",
                            "Failed to start remote control.",
                            "bg-danger",
                            requesterID);
                    }
                }
                else
                {
                    // SignalR Connection IDs might start with a hyphen.  We surround them
                    // with quotes so the command line will be parsed correctly.
                    Process.Start(_rcBinaryPath, 
                        $"-mode Unattended " +
                        $"-requester \"{requesterID}\" " +
                        $"-serviceid \"{serviceID}\" " +
                        $"-deviceid {ConnectionInfo.DeviceID} " +
                        $"-host {ConnectionInfo.Host} " +
                        $" -orgid \"{ConnectionInfo.OrganizationID}\"" +
                        $"-relaunch true " +
                        $"-viewers {String.Join(",", viewerIDs)}");
                }
            }
            catch (Exception ex)
            {
                await hubConnection.SendAsync("SendConnectionFailedToViewers", viewerIDs);
                Logger.Write(ex);
                throw;
            }
        }
    }
}
