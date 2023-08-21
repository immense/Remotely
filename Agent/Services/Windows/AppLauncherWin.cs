using Immense.RemoteControl.Desktop.Shared.Native.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Remotely.Agent.Interfaces;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Remotely.Agent.Services.Windows;

[SupportedOSPlatform("windows")]
public class AppLauncherWin : IAppLauncher
{
    private readonly ConnectionInfo _connectionInfo;
    private readonly ILogger<AppLauncherWin> _logger;
    private readonly string _rcBinaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Desktop", EnvironmentHelper.DesktopExecutableFileName);

    public AppLauncherWin(IConfigService configService, ILogger<AppLauncherWin> logger)
    {
        _connectionInfo = configService.GetConnectionInfo();
        _logger = logger;
    }

    public async Task<int> LaunchChatService(string pipeName, string userConnectionId, string requesterName, string orgName, string orgId, HubConnection hubConnection)
    {
        try
        {
            if (!File.Exists(_rcBinaryPath))
            {
                await hubConnection.SendAsync("DisplayMessage", "Chat executable not found on target device.", "Executable not found on device.", "bg-danger", userConnectionId);
            }


            // Start Desktop app.
            await hubConnection.SendAsync("DisplayMessage", $"Starting chat service.", "Starting chat service.", "bg-success", userConnectionId);
            if (WindowsIdentity.GetCurrent().IsSystem)
            {
                var result = Win32Interop.CreateInteractiveSystemProcess(
                    _rcBinaryPath +
                        $" --mode Chat" +
                        $" --host \"{_connectionInfo.Host}\"" +
                        $" --pipe-name {pipeName}" +
                        $" --requester-name \"{requesterName}\"" +
                        $" --org-name \"{orgName}\"" +
                        $" --org-id \"{orgId}\"",
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
                        userConnectionId);
                }
                else
                {
                    return procInfo.dwProcessId;
                }
            }
            else
            {
                return Process.Start(_rcBinaryPath,
                    $" --mode Chat" +
                    $" --host \"{_connectionInfo.Host}\"" +
                    $" --requester-name \"{userConnectionId}\"" +
                    $" --org-name \"{orgName}\"" +
                    $" --org-id \"{orgId}\"" +
                    $" --pipe-name {pipeName}").Id;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while launching chat.");
            await hubConnection.SendAsync("DisplayMessage",
                "Chat service failed to start on target device.",
                "Failed to start chat service.",
                "bg-danger",
                userConnectionId);
        }
        return -1;
    }

    public async Task LaunchRemoteControl(int targetSessionId, string sessionId, string accessKey, string userConnectionId, string requesterName, string orgName, string orgId, HubConnection hubConnection)
    {
        try
        {
            if (!File.Exists(_rcBinaryPath))
            {
                await hubConnection.SendAsync("DisplayMessage",
                    "Remote control executable not found on target device.",
                    "Executable not found on device.",
                    "bg-danger",
                    userConnectionId);
                return;
            }


            // Start Desktop app.
            await hubConnection.SendAsync("DisplayMessage",
                "Starting remote control",
                "Starting remote control",
                "bg-success",
                userConnectionId);
            if (WindowsIdentity.GetCurrent().IsSystem)
            {
                var result = Win32Interop.CreateInteractiveSystemProcess(
                    _rcBinaryPath +
                        $" --mode Unattended" +
                        $" --host {_connectionInfo.Host}" +
                        $" --requester-name \"{requesterName}\"" +
                        $" --org-name \"{orgName}\"" +
                        $" --org-id \"{orgId}\"" +
                        $" --session-id \"{sessionId}\"" +
                        $" --access-key \"{accessKey}\"",
                    targetSessionId: targetSessionId,
                    forceConsoleSession: Shlwapi.IsOS(OsType.OS_ANYSERVER) && targetSessionId == -1,
                    desktopName: "default",
                    hiddenWindow: false,
                    out _);
                if (!result)
                {
                    await hubConnection.SendAsync("DisplayMessage",
                        "Remote control failed to start on target device.",
                        "Failed to start remote control.",
                        "bg-danger",
                        userConnectionId);
                }
            }
            else
            {
                Process.Start(_rcBinaryPath,
                        $" --mode Unattended" +
                        $" --host {_connectionInfo.Host}" +
                        $" --requester-name \"{requesterName}\"" +
                        $" --org-name \"{orgName}\"" +
                        $" --org-id \"{orgId}\"" +
                        $" --session-id \"{sessionId}\"" +
                        $" --access-key \"{accessKey}\"");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while launching remote control.");
            await hubConnection.SendAsync("DisplayMessage",
                "Remote control failed to start on target device.",
                "Failed to start remote control.",
                "bg-danger",
                userConnectionId);
        }
    }
    public async Task RestartScreenCaster(string[] viewerIds, string sessionId, string accessKey, string userConnectionId, string requesterName, string orgName, string orgId, HubConnection hubConnection, int targetSessionID = -1)
    {
        try
        {
            // Start Desktop app.                 
            _logger.LogInformation("Restarting screen caster.");
            if (WindowsIdentity.GetCurrent().IsSystem)
            {
                // Give a little time for session changing, etc.
                await Task.Delay(1000);

                var result = Win32Interop.CreateInteractiveSystemProcess(_rcBinaryPath +
                        $" --mode Unattended" +
                        $" --relaunch true" +
                        $" --host {_connectionInfo.Host}" +
                        $" --requester-name \"{requesterName}\"" +
                        $" --org-name \"{orgName}\"" +
                        $" --org-id \"{orgId}\"" +
                        $" --session-id \"{sessionId}\"" +
                        $" --access-key \"{accessKey}\"" +
                        $" --viewers {string.Join(",", viewerIds)}",

                    targetSessionId: targetSessionID,
                    forceConsoleSession: Shlwapi.IsOS(OsType.OS_ANYSERVER) && targetSessionID == -1,
                    desktopName: "default",
                    hiddenWindow: false,
                    out _);

                if (!result)
                {
                    _logger.LogWarning("Failed to relaunch screen caster.");
                    await hubConnection.SendAsync("SendConnectionFailedToViewers", viewerIds);
                    await hubConnection.SendAsync("DisplayMessage",
                        "Remote control failed to start on target device.",
                        "Failed to start remote control.",
                        "bg-danger",
                        userConnectionId);
                }
            }
            else
            {
                Process.Start(_rcBinaryPath,
                    $" --mode Unattended" +
                    $" --relaunch true" +
                    $" --host {_connectionInfo.Host}" +
                    $" --requester-name \"{requesterName}\"" +
                    $" --org-name \"{orgName}\"" +
                    $" --org-id \"{orgId}\"" +
                    $" --session-id \"{sessionId}\"" +
                    $" --access-key \"{accessKey}\"" +
                    $" --viewers {string.Join(",", viewerIds)}");
            }
        }
        catch (Exception ex)
        {
            await hubConnection.SendAsync("SendConnectionFailedToViewers", viewerIds);
            _logger.LogError(ex, "Error while restarting screen caster.");
            throw;
        }
    }
}
