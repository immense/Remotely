using Remotely.Agent.Interfaces;
using Remotely.Shared.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Agent.Services
{
    public class UpdaterWin : IUpdater
    {
        private readonly SemaphoreSlim _checkForUpdatesLock = new(1, 1);
        private readonly ConfigService _configService;
        private readonly IWebClientEx _webClientEx;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SemaphoreSlim _installLatestVersionLock = new(1, 1);
        private readonly System.Timers.Timer _updateTimer = new(TimeSpan.FromHours(6).TotalMilliseconds);
        private DateTimeOffset _lastUpdateFailure;


        public UpdaterWin(ConfigService configService, IWebClientEx webClientEx, IHttpClientFactory httpClientFactory)
        {
            _configService = configService;
            _webClientEx = webClientEx;
            _httpClientFactory = httpClientFactory;
            _webClientEx.SetRequestTimeout((int)_updateTimer.Interval);
        }

        public async Task BeginChecking()
        {
            if (EnvironmentHelper.IsDebug)
            {
                return;
            }

            await CheckForUpdates();
            _updateTimer.Elapsed += UpdateTimer_Elapsed;
            _updateTimer.Start();
        }

        public async Task CheckForUpdates()
        {
            try
            {
                await _checkForUpdatesLock.WaitAsync();

                if (EnvironmentHelper.IsDebug)
                {
                    return;
                }

                if (_lastUpdateFailure.AddDays(1) > DateTimeOffset.Now)
                {
                    Logger.Write("Skipping update check due to previous failure.  Updating will be tried again after 24 hours have passed.");
                    return;
                }


                var connectionInfo = _configService.GetConnectionInfo();
                var serverUrl = _configService.GetConnectionInfo().Host;

                var platform = Environment.Is64BitOperatingSystem ? "x64" : "x86";
                var fileUrl = serverUrl + $"/Content/Remotely-Win10-{platform}.zip";

                using var httpClient = _httpClientFactory.CreateClient();
                using var request = new HttpRequestMessage(HttpMethod.Head, fileUrl);

                if (File.Exists("etag.txt"))
                {
                    var lastEtag = await File.ReadAllTextAsync("etag.txt");
                    if (!string.IsNullOrEmpty(lastEtag))
                    {
                        request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(lastEtag.Trim()));
                    }
                }

                using var response = await httpClient.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    Logger.Write("Service Updater: Version is current.");
                    return;
                }


                Logger.Write("Service Updater: Update found.");

                await InstallLatestVersion();

            }
            catch (WebException ex) when ((ex.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotModified)
            {
                Logger.Write("Service Updater: Version is current.");
                return;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                _checkForUpdatesLock.Release();
            }
        }

        public void Dispose()
        {
            _webClientEx?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task InstallLatestVersion()
        {
            try
            {
                await _installLatestVersionLock.WaitAsync();

                var connectionInfo = _configService.GetConnectionInfo();
                var serverUrl = connectionInfo.Host;

                Logger.Write("Service Updater: Downloading install package.");

                var downloadId = Guid.NewGuid().ToString();
                var zipPath = Path.Combine(Path.GetTempPath(), "RemotelyUpdate.zip");

                var installerPath = Path.Combine(Path.GetTempPath(), "Remotely_Installer.exe");
                var platform = Environment.Is64BitOperatingSystem ? "x64" : "x86";

                await _webClientEx.DownloadFileTaskAsync(
                     serverUrl + $"/Content/Remotely_Installer.exe",
                     installerPath);

                await _webClientEx.DownloadFileTaskAsync(
                   serverUrl + $"/api/AgentUpdate/DownloadPackage/win-{platform}/{downloadId}",
                   zipPath);

                using var httpClient = _httpClientFactory.CreateClient();
                using var response = httpClient.GetAsync($"{serverUrl}/api/AgentUpdate/ClearDownload/{downloadId}");

                foreach (var proc in Process.GetProcessesByName("Remotely_Installer"))
                {
                    proc.Kill();
                }

                Logger.Write("Launching installer to perform update.");

                Process.Start(installerPath, $"-install -quiet -path {zipPath} -serverurl {serverUrl} -organizationid {connectionInfo.OrganizationID}");
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.Timeout)
            {
                Logger.Write("Timed out while waiting to download update.", Shared.Enums.EventType.Warning);
                _lastUpdateFailure = DateTimeOffset.Now;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                _lastUpdateFailure = DateTimeOffset.Now;
            }
            finally
            {
                _installLatestVersionLock.Release();
            }
        }

        private async void UpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            await CheckForUpdates();
        }
    }
}
