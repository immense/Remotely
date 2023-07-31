using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Remotely.Server.Services;

public interface IUpgradeService
{
    Version GetCurrentVersion();
    Task<bool> IsNewVersionAvailable();
}

public class UpgradeService : IUpgradeService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<UpgradeService> _logger;
    private Version? _currentVersion;

    public UpgradeService(IHttpClientFactory httpClientFactory, ILogger<UpgradeService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public Version GetCurrentVersion()
    {
        try
        {
            if (_currentVersion is not null)
            {
                return _currentVersion;
            }

            var asmVersion = typeof(UpgradeService).Assembly.GetName().Version;
            if (asmVersion is not null)
            {
                _currentVersion = asmVersion;
                return _currentVersion;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting current version.");
        }

        _currentVersion = new(1, 0);
        return _currentVersion;
    }
    public async Task<bool> IsNewVersionAvailable()
    {

        try
        {
            using var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://github.com/immense/Remotely/releases/latest");
            var versionTag = $"{response.RequestMessage?.RequestUri}".Split("/").LastOrDefault();
            if (string.IsNullOrWhiteSpace(versionTag))
            {
                return false;
            }
            var versionString = versionTag[1..];
            var remoteVersion = Version.Parse(versionString);

            var filePath = Directory.GetFiles(Directory.GetCurrentDirectory(), "Remotely_Server.dll", SearchOption.AllDirectories).First();
            var fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(filePath).FileVersion;
            if (string.IsNullOrWhiteSpace(fileVersion))
            {
                return false;
            }
            var localVersion = Version.Parse(fileVersion);

            if (remoteVersion > localVersion)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve latest version info.");
        }

        return false;
    }
}
