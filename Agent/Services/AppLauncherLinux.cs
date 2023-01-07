using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Remotely.Agent.Interfaces;
using Remotely.Shared.Models;
using Remotely.Shared.Services;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Services.Description;

namespace Remotely.Agent.Services
{

    public class AppLauncherLinux : IAppLauncher
    {
        private readonly string _rcBinaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Desktop", EnvironmentHelper.DesktopExecutableFileName);
        private readonly IProcessInvoker _processInvoker;
        private readonly ConnectionInfo _connectionInfo;
        private readonly ILogger<AppLauncherLinux> _logger;

        public AppLauncherLinux(
            ConfigService configService, 
            IProcessInvoker processInvoker,
            ILogger<AppLauncherLinux> logger)
        {
            _processInvoker = processInvoker;
            _connectionInfo = configService.GetConnectionInfo();
            _logger = logger;
        }


        private int StartLinuxDesktopApp(string args)
        {
            var xauthority = GetXorgAuth();

            var display = ":0";
            var whoString = _processInvoker.InvokeProcessOutput("who", "")?.Trim();
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
                    _logger.LogError(ex, "Error while getting current X11 user.");
                }
            }

            var psi = new ProcessStartInfo()
            {
                FileName = "sudo",
                Arguments = args
            };

            psi.Environment.Add("DISPLAY", display);
            psi.Environment.Add("XAUTHORITY", xauthority);
            _logger.LogInformation(
                "Attempting to launch screen caster with username {username}, xauthority {xauthority}, display {display}, and args {args}.",
                username,
                xauthority,
                display,
                args);
            return Process.Start(psi).Id;
        }

        private string GetXorgAuth()
        {
            try
            {
                var processes = _processInvoker.InvokeProcessOutput("ps", "-eaf")?.Split(Environment.NewLine);
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

        public async Task RestartScreenCaster(List<string> viewerIDs, string sessionId, string accessKey, string userConnectionId, string requesterName, string orgName, string orgId, HubConnection hubConnection, int targetSessionID = -1)
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
                await hubConnection.SendAsync("SendConnectionFailedToViewers", viewerIDs);
                _logger.LogError(ex, "Error while restarting screen caster.");
                throw;
            }
        }
    }
}
