using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Remotely.Shared.Utilities;

namespace Remotely.Agent.Services
{
    public class Updater
    {
        public Updater(ConfigService configService)
        {
            ConfigService = configService;
        }

        private ConfigService ConfigService { get; }
        private SemaphoreSlim UpdateLock { get; } = new SemaphoreSlim(1);
        private System.Timers.Timer UpdateTimer { get; } = new System.Timers.Timer(TimeSpan.FromHours(6).TotalMilliseconds);


        public async Task BeginChecking()
        {
            await CheckForUpdates();
            UpdateTimer.Elapsed += UpdateTimer_Elapsed;
            UpdateTimer.Start();
        }

        private async void UpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            await CheckForUpdates();
        }

        public async Task CheckForUpdates()
        {
            try
            {
                await UpdateLock.WaitAsync();

                var hc = new HttpClient();
                var wc = new WebClient();
                var connectionInfo = ConfigService.GetConnectionInfo();

                var response = await hc.GetAsync(connectionInfo.Host + $"/API/AgentUpdate/CurrentVersion");
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
                    if (EnvironmentHelper.IsWindows)
                    {
                        var filePath = Path.Combine(Path.GetTempPath(), "Remotely_Installer.exe");

                        wc.DownloadFile(
                            ConfigService.GetConnectionInfo().Host + $"/Downloads/Remotely_Installer.exe",
                            filePath);

                        foreach (var proc in Process.GetProcessesByName("Remotely_Installer"))
                        {
                            proc.Kill();
                        }

                        Process.Start(filePath, $"-install -quiet -serverurl {connectionInfo.Host} -organizationid {connectionInfo.OrganizationID}");
                    }
                    else if (EnvironmentHelper.IsLinux)
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
            finally
            {
                UpdateLock.Release();
            }
        }
    }
}
