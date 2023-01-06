﻿using Microsoft.Extensions.Logging;
using Remotely.Agent.Interfaces;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Agent.Services
{
    
    public class UpdaterLinux : IUpdater
    {
        private readonly SemaphoreSlim _checkForUpdatesLock = new(1, 1);
        private readonly ConfigService _configService;
        private readonly IUpdateDownloader _updateDownloader;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UpdaterLinux> _logger;
        private readonly SemaphoreSlim _installLatestVersionLock = new(1, 1);
        private readonly System.Timers.Timer _updateTimer = new(TimeSpan.FromHours(6).TotalMilliseconds);
        private DateTimeOffset _lastUpdateFailure;
        
        public UpdaterLinux(
            ConfigService configService, 
            IUpdateDownloader updateDownloader, 
            IHttpClientFactory httpClientFactory,
            ILogger<UpdaterLinux> logger)
        {
            _configService = configService;
            _updateDownloader = updateDownloader;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
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
            if (!await _checkForUpdatesLock.WaitAsync(0))
            {
                return;
            }

            try
            {
                if (EnvironmentHelper.IsDebug)
                {
                    return;
                }

                if (_lastUpdateFailure.AddDays(1) > DateTimeOffset.Now)
                {
                    _logger.LogInformation("Skipping update check due to previous failure.  Restart the service to try again, or manually install the update.");
                    return;
                }


                var connectionInfo = _configService.GetConnectionInfo();
                var serverUrl = _configService.GetConnectionInfo().Host;

                var fileUrl = serverUrl + $"/Content/Remotely-Linux.zip";

                using var httpClient = _httpClientFactory.CreateClient();
                using var request = new HttpRequestMessage(HttpMethod.Head, fileUrl);

                if (File.Exists("etag.txt"))
                {
                    var lastEtag = await File.ReadAllTextAsync("etag.txt");
                    if (!string.IsNullOrWhiteSpace(lastEtag) &&
                       EntityTagHeaderValue.TryParse(lastEtag.Trim(), out var etag))
                    {
                        request.Headers.IfNoneMatch.Add(etag);
                    }
                }

                using var response = await httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    _logger.LogInformation("Service Updater: Version is current.");
                    return;
                }

                _logger.LogInformation("Service Updater: Update found.");

                await InstallLatestVersion();

            }
            catch (WebException ex) when ((ex.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotModified)
            {
                _logger.LogInformation("Service Updater: Version is current.");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking for updates.");
            }
            finally
            {
                _checkForUpdatesLock.Release();
            }
        }

        public async Task InstallLatestVersion()
        {
            try
            {
                await _installLatestVersionLock.WaitAsync();

                var connectionInfo = _configService.GetConnectionInfo();
                var serverUrl = connectionInfo.Host;

                _logger.LogInformation("Service Updater: Downloading install package.");

                var downloadId = Guid.NewGuid().ToString();
                var zipPath = Path.Combine(Path.GetTempPath(), "RemotelyUpdate.zip");

                var installerPath = Path.Combine(Path.GetTempPath(), "RemotelyUpdate.sh");

                string platform;

                if (RuntimeInformation.OSDescription.Contains("Ubuntu", StringComparison.OrdinalIgnoreCase))
                {
                    platform = "UbuntuInstaller-x64";
                }
                else if (RuntimeInformation.OSDescription.Contains("Manjaro", StringComparison.OrdinalIgnoreCase))
                {
                    platform = "ManjaroInstaller-x64";
                }
                else
                {
                    throw new PlatformNotSupportedException();
                }

                await _updateDownloader.DownloadFile(
                       $"{serverUrl}/API/ClientDownloads/{connectionInfo.OrganizationID}/{platform}",
                       installerPath);

                await _updateDownloader.DownloadFile(
                   $"{serverUrl}/API/AgentUpdate/DownloadPackage/linux/{downloadId}",
                   zipPath);

                using var httpClient = _httpClientFactory.CreateClient();
                using var response = httpClient.GetAsync($"{serverUrl}/api/AgentUpdate/ClearDownload/{downloadId}");

                _logger.LogInformation("Launching installer to perform update.");

                Process.Start("sudo", $"chmod +x {installerPath}").WaitForExit();

                Process.Start("sudo", $"{installerPath} --path {zipPath}");
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.Timeout)
            {
                _logger.LogWarning("Timed out while waiting to download update.");
                _lastUpdateFailure = DateTimeOffset.Now;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while installing latest version.");
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
