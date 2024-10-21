using Microsoft.Extensions.Logging;
using Remotely.Shared.Entities;
using Remotely.Shared.Primitives;
using Remotely.Shared.Services;
using System.Diagnostics;
using System.Net.Http.Json;

namespace Remotely.Desktop.Shared.Services;

public interface IBrandingProvider
{
    BrandingInfo CurrentBranding { get; }
    Task Initialize();
    void SetBrandingInfo(BrandingInfo brandingInfo);
}

public class BrandingProvider : IBrandingProvider
{
    private readonly IAppState _appState;
    private readonly IEmbeddedServerDataProvider _embeddedDataSearcher;
    private readonly ILogger<BrandingProvider> _logger;
    private BrandingInfo? _brandingInfo;


    public BrandingProvider(
        IAppState appState,
        IEmbeddedServerDataProvider embeddedServerDataSearcher,
        ILogger<BrandingProvider> logger)
    {
        _appState = appState;
        _embeddedDataSearcher = embeddedServerDataSearcher;
        _logger = logger;
    }

    public BrandingInfo CurrentBranding => _brandingInfo ??
        throw new InvalidOperationException("Branding info has not been set or initialized.");

    public async Task Initialize()
    {
        if (_brandingInfo is not null)
        {
            return;
        }

        var result = await TryGetBrandingInfo();

        if (result.IsSuccess)
        {
            _brandingInfo = result.Value;
        }
        else
        {
            _logger.LogWarning(result.Exception, "Failed to extract embedded service data.");
            _brandingInfo = new()
            {
                Product = "Remote Control"
            };
        }

        if (_brandingInfo.Icon is not { Length: > 0 })
        {
            using var mrs = typeof(BrandingProvider).Assembly.GetManifestResourceStream("Remotely.Desktop.Shared.Assets.Remotely_Icon.png");
            using var ms = new MemoryStream();
            mrs!.CopyTo(ms);

            _brandingInfo.Icon = ms.ToArray();
        }
    }

    public void SetBrandingInfo(BrandingInfo brandingInfo)
    {
        _brandingInfo = brandingInfo;
    }

    private async Task<Result<BrandingInfo>> TryGetBrandingInfo()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_appState.OrganizationId) ||
                string.IsNullOrWhiteSpace(_appState.Host))
            {
                var filePath = Process.GetCurrentProcess()?.MainModule?.FileName;

                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return Result.Fail<BrandingInfo>("Failed to retrieve executing file name.");
                }

                var result = _embeddedDataSearcher.TryGetEmbeddedData(filePath);

                if (result.IsSuccess)
                {
                    if (!string.IsNullOrWhiteSpace(result.Value.OrganizationId))
                    {
                        _appState.OrganizationId = result.Value.OrganizationId;
                    }

                    if (result.Value.ServerUrl is not null)
                    {
                        _appState.Host = result.Value.ServerUrl.AbsoluteUri;
                    }
                }

                if (string.IsNullOrWhiteSpace(_appState.Host))
                {
                    return result.HadException ?
                        Result.Fail<BrandingInfo>(result.Exception) :
                        Result.Fail<BrandingInfo>(result.Reason);
                }
            }

            if (string.IsNullOrWhiteSpace(_appState.Host))
            {
                return Result.Fail<BrandingInfo>("ServerUrl is empty.");
            }

            using var httpClient = new HttpClient();

            var brandingUrl = $"{_appState.Host.TrimEnd('/')}/api/branding/{_appState.OrganizationId}";
            var httpResult = await httpClient.GetFromJsonAsync<BrandingInfo>(brandingUrl).ConfigureAwait(false);
            if (httpResult is null)
            {
                return Result.Fail<BrandingInfo>("Branding API HTTP result is null.");
            }

            return Result.Ok(httpResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get branding info.");
            return Result.Fail<BrandingInfo>(ex);
        }
    }
}
