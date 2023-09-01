using Immense.RemoteControl.Shared;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Remotely.Agent.Interfaces;
using Remotely.Shared.Extensions;
using Remotely.Shared.Models;
using Remotely.Shared.Services;
using Remotely.Shared.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Remotely.Agent.Services.Linux;


public class AppLauncherLinux : IAppLauncher
{
    private readonly ConnectionInfo _connectionInfo;
    private readonly ILogger<AppLauncherLinux> _logger;
    private readonly IProcessInvoker _processInvoker;

    private readonly string _rcBinaryPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, 
        "Desktop", 
        EnvironmentHelper.DesktopExecutableFileName);

    public AppLauncherLinux(
        IConfigService configService,
        IProcessInvoker processInvoker,
        ILogger<AppLauncherLinux> logger)
    {
        _processInvoker = processInvoker;
        _connectionInfo = configService.GetConnectionInfo();
        _logger = logger;
    }


    public async Task<int> LaunchChatService(string pipeName, string userConnectionId, string requesterName, string orgName, string orgId, HubConnection hubConnection)
    {
        try
        {
            if (!File.Exists(_rcBinaryPath))
            {
                await hubConnection.SendAsync("DisplayMessage",
                    "Chat executable not found on target device.",
                    "Executable not found on device.",
                    "bg-danger",
                    userConnectionId);
            }


            // Start Desktop app.
            await hubConnection.SendAsync("DisplayMessage", $"Starting chat service.", "Starting chat service.", "bg-success", userConnectionId);
            var args =
                _rcBinaryPath +
                $" --mode Chat" +
                $" --host \"{_connectionInfo.Host}\"" +
                $" --pipe-name {pipeName}" +
                $" --requester-name \"{requesterName}\"" +
                $" --org-name \"{orgName}\"" +
                $" --org-id \"{orgId}\"";
            return StartLinuxDesktopApp(args);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while starting chat.");
            await hubConnection.SendAsync("DisplayMessage", "Chat service failed to start on target device.", "Failed to start chat service.", "bg-danger", userConnectionId);
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
            await hubConnection.SendAsync("DisplayMessage", "Starting remote control.", "Starting remote control.", "bg-success", userConnectionId);
            var args =
                _rcBinaryPath +
                $" --mode Unattended" +
                $" --host {_connectionInfo.Host}" +
                $" --requester-name \"{requesterName}\"" +
                $" --org-name \"{orgName}\"" +
                $" --org-id \"{orgId}\"" +
                $" --session-id \"{sessionId}\"" +
                $" --access-key \"{accessKey}\"";
            StartLinuxDesktopApp(args);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while launching remote control.");
            await hubConnection.SendAsync("DisplayMessage", "Remote control failed to start on target device.", "Failed to start remote control.", "bg-danger", userConnectionId);
        }
    }

    public async Task RestartScreenCaster(string[] viewerIds, string sessionId, string accessKey, string userConnectionId, string requesterName, string orgName, string orgId, HubConnection hubConnection, int targetSessionID = -1)
    {
        try
        {
            var args =
                _rcBinaryPath +
                $" --mode Unattended" +
                $" --host {_connectionInfo.Host}" +
                $" --requester-name \"{requesterName}\"" +
                $" --org-name \"{orgName}\"" +
                $" --org-id \"{orgId}\"" +
                $" --session-id \"{sessionId}\"" +
                $" --access-key \"{accessKey}\"";
            StartLinuxDesktopApp(args);
        }
        catch (Exception ex)
        {
            await hubConnection.SendAsync("SendConnectionFailedToViewers", viewerIds);
            _logger.LogError(ex, "Error while restarting screen caster.");
            throw;
        }
    }

    private int StartLinuxDesktopApp(string args)
    {
        var xdisplay = ":0";
        var xauthority = string.Empty;
        
        var xResult = TryGetXAuth("Xorg");

        if (!xResult.IsSuccess)
        {
            // If running Wayland, this still ends up still being unusable.
            // This X server will only provide a black screen with any apps
            // launched within the display it's using, but the display won't
            // show anything being rendered by the Wayland compositor.  It's
            // better than simply crashing, though, so I'll leave it here
            // until Wayland support is added.
            xResult = TryGetXAuth("Xwayland");
        }

        if (xResult.IsSuccess)
        {
            xdisplay = xResult.Value.XDisplay;
            xauthority = xResult.Value.XAuthority;
        }
        else
        {
            _logger.LogError("Failed to get X server auth.");
        }
        
        var whoString = _processInvoker.InvokeProcessOutput("who", "")?.Trim();
        var username = "";

        if (!string.IsNullOrWhiteSpace(whoString))
        {
            try
            {
                var whoLines = whoString.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                var whoLine = whoLines.FirstOrDefault(x =>
                    Regex.IsMatch(
                        x.Split(" ", StringSplitOptions.RemoveEmptyEntries).Last(),
                        @"\(:[\d]*\)"));

                if (!string.IsNullOrWhiteSpace(whoLine))
                {
                    var whoSplit = whoLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    username = whoSplit[0];
                    xdisplay = whoSplit.Last().TrimStart('(').TrimEnd(')');
                    xauthority = $"/home/{username}/.Xauthority";
                    args = $"-u {username} {args}";

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting current X11 user.");
            }
        }

        var psi = new ProcessStartInfo()
        {
            FileName = "sudo",
            Arguments = args
        };

        psi.Environment.Add("DISPLAY", xdisplay);
        psi.Environment.Add("XAUTHORITY", xauthority);
        _logger.LogInformation(
            "Attempting to launch screen caster with username {username}, xauthority {xauthority}, display {display}, and args {args}.",
            username,
            xauthority,
            xdisplay,
            args);
        return Process.Start(psi)?.Id ?? throw new InvalidOperationException("Failed to launch desktop app.");
    }

    private Result<XAuthInfo> TryGetXAuth(string xServerProcess)
    {
        try
        {
            var xdisplay = ":0";
            var xauthority = string.Empty;

            var xprocess = _processInvoker
                .InvokeProcessOutput("ps", $"-C {xServerProcess} -f")
                ?.Split(Environment.NewLine)
                ?.FirstOrDefault(x => x.Contains(" -auth "));

            if (string.IsNullOrWhiteSpace(xprocess))
            {
                _logger.LogInformation("{xServerProcess} process not found.", xServerProcess);
                return Result.Fail<XAuthInfo>($"{xServerProcess} process not found.");
            }

            _logger.LogInformation("Resolved X server process: {xprocess}", xprocess);

            var xprocSplit = xprocess
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            var xProcIndex = xprocSplit.IndexWhere(x => x.EndsWith(xServerProcess));
            if (xProcIndex > -1 && xprocSplit[xProcIndex + 1].StartsWith(":"))
            {
                xdisplay = xprocSplit[xProcIndex + 1];
            }

            var authIndex = xprocSplit.IndexOf("-auth");
            if (authIndex > -1)
            {
                xauthority = xprocSplit[authIndex + 1];
            }
            return Result.Ok(new XAuthInfo(xdisplay, xauthority));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while geting X auth for {xServerProcess}.", xServerProcess);
            return Result.Fail<XAuthInfo>($"Error while geting X auth for {xServerProcess}.");
        }
    }
    private record XAuthInfo(string XDisplay, string XAuthority);
}
