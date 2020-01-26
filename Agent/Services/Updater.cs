using Remotely.Shared.Services;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace Remotely.Agent.Services
{
    public class Updater
    {
        public Updater(ConfigService configService)
        {
            ConfigService = configService;
        }

        private ConfigService ConfigService { get; }

        public async Task CheckForUpdates()
        {
            try
            {
                var wc = new WebClient();
                var connectionInfo = ConfigService.GetConnectionInfo();
                var response = new HttpClient().GetAsync(connectionInfo.Host + $"/API/AgentUpdate/CurrentVersion").Result;
                var latestVersion = response.Content.ReadAsStringAsync().Result;
                var thisVersion = FileVersionInfo.GetVersionInfo("Remotely_Agent.dll").FileVersion.ToString().Trim();
                if (thisVersion != latestVersion)
                {
                    Logger.Write($"Service Updater: Update found.  Current Version: {thisVersion}.  Latest Version: {latestVersion}.");

                    var updateWindow = int.Parse(wc.DownloadString(connectionInfo.Host + $"/API/AgentUpdate/UpdateWindow"));
                    var waitTime = new Random().Next(1, updateWindow);
                    Logger.Write($"Waiting {waitTime} seconds before updating.");
                    await Task.Delay(TimeSpan.FromSeconds(waitTime));

                    Logger.Write($"Service Updater: Downloading installer.");
                    if (OSUtils.IsWindows)
                    {
                        var filePath = Path.Combine(Path.GetTempPath(), "RemotelyUpdate.ps1");

                        wc.DownloadFile(
                            ConfigService.GetConnectionInfo().Host + $"/API/ClientDownloads/{connectionInfo.OrganizationID}/Win10",
                            filePath);

                        Process.Start("powershell.exe", $"-f \"{filePath}\"");
                    }
                    else if (OSUtils.IsLinux)
                    {
                        var filePath = Path.Combine(Path.GetTempPath(), "RemotelyUpdate.sh");

                        wc.DownloadFile(
                            ConfigService.GetConnectionInfo().Host + $"/API/ClientDownloads/{connectionInfo.OrganizationID}/Linux-x64",
                            filePath);

                        Process.Start("sudo", $"chmod +x {filePath}").WaitForExit();

                        Process.Start("sudo", $"{filePath} & disown");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
    }
}
