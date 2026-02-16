using Microsoft.Extensions.Logging;
using Remotely.Agent.Interfaces;
using Remotely.Shared.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Agent.Services.Windows;

public class UpdaterWin : IUpdater, IDisposable
{
    private readonly SemaphoreSlim _checkForUpdatesLock = new(1, 1);
    private readonly IConfigService _configService;
    private readonly IUpdateDownloader _updateDownloader;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<UpdaterWin> _logger;
    private readonly SemaphoreSlim _installLatestVersionLock = new(1, 1);
    private readonly System.Timers.Timer _updateTimer;
    private DateTimeOffset _lastUpdateFailure;
    private bool _disposed;


    public UpdaterWin(
        IConfigService configService,
        IUpdateDownloader updateDownloader,
        IHttpClientFactory httpClientFactory,
        ILogger<UpdaterWin> logger)
    {
        _configService = configService;
        _updateDownloader = updateDownloader;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _updateTimer = new System.Timers.Timer(TimeSpan.FromHours(6).TotalMilliseconds)
        {
            AutoReset = false
        };
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        
        _updateTimer?.Stop();
        _updateTimer?.Dispose();
        
        _checkForUpdatesLock?.Dispose();
        _installLatestVersionLock?.Dispose();
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
                _logger.LogInformation("Skipping update check due to previous failure.  Updating will be tried again after 24 hours have passed.");
                return;
            }


            var connectionInfo = _configService.GetConnectionInfo();
            var serverUrl = _configService.GetConnectionInfo().Host;

            var platform = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            var fileUrl = serverUrl + $"/Content/Remotely-Win-{platform}.zip";

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
        catch (WebException ex) when (ex.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.NotModified)
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

            var zipPath = Path.Combine(Path.GetTempPath(), "RemotelyUpdate.zip");

            var installerPath = Path.Combine(Path.GetTempPath(), "Install-Remotely.ps1");
            var platform = Environment.Is64BitOperatingSystem ? "x64" : "x86";

            await _updateDownloader.DownloadFile(
                 $"{serverUrl}/Content/Install-Remotely.ps1",
                 installerPath);

            await _updateDownloader.DownloadFile(
               $"{serverUrl}/api/AgentUpdate/DownloadPackage/win-{platform}",
               zipPath);

            _logger.LogInformation("Launching installer to perform update.");

            var safeInstallerPath = installerPath.Replace("\"", "\\\"");
            var safeZipPath = zipPath.Replace("\"", "\\\"");
            var safeOrgId = connectionInfo.OrganizationID?.Replace("\"", "\\\"") ?? "";
            var safeServerUrl = serverUrl.Replace("\"", "\\\"");
            var platform = Environment.Is64BitOperatingSystem ? "x64" : "x86";

            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy Bypass -File \"{safeInstallerPath}\" -Path \"{safeZipPath}\" -OrganizationId \"{safeOrgId}\" -ServerUrl \"{safeServerUrl}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process.Start(psi)?.WaitForExit();
            
            try
            {
                File.Delete(installerPath);
                File.Delete(zipPath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete update files after installation.");
            }
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
            await _installLatestVersionLock.ReleaseAsync();
        }
    }

    private async Task UpdateTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        await CheckForUpdates();
    }
}
