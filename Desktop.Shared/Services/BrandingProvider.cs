using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Immense.RemoteControl.Desktop.Shared.Services;
using Immense.RemoteControl.Shared;
using Immense.RemoteControl.Shared.Models;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Entities;
using Remotely.Shared.Services;
using System.Diagnostics;
using System.Net.Http.Json;

namespace Desktop.Shared.Services;

public class BrandingProvider : IBrandingProvider
{
    private readonly IAppState _appState;
    private readonly IEmbeddedServerDataSearcher _embeddedDataSearcher;
    private readonly ILogger<BrandingProvider> _logger;
    private readonly IOrganizationIdProvider _orgIdProvider;
    private BrandingInfoBase? _brandingInfo;


    public BrandingProvider(
        IAppState appState,
        IOrganizationIdProvider orgIdProvider,
        IEmbeddedServerDataSearcher embeddedServerDataSearcher,
        ILogger<BrandingProvider> logger)
    {
        _appState = appState;
        _orgIdProvider = orgIdProvider;
        _embeddedDataSearcher = embeddedServerDataSearcher;
        _logger = logger;
    }

    public BrandingInfoBase CurrentBranding => _brandingInfo ??
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

        if (_brandingInfo.Icon?.Any() != true)
        {
            using var mrs = typeof(BrandingProvider).Assembly.GetManifestResourceStream("Desktop.Shared.Assets.Remotely_Icon.png");
            using var ms = new MemoryStream();
            mrs!.CopyTo(ms);

            _brandingInfo.Icon = ms.ToArray();
        }
    }

    public void SetBrandingInfo(BrandingInfoBase brandingInfo)
    {
        _brandingInfo = brandingInfo;
    }

    private async Task<Result<BrandingInfo>> TryGetBrandingInfo()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_orgIdProvider.OrganizationId) ||
                string.IsNullOrWhiteSpace(_appState.Host))
            {
                var filePath = Process.GetCurrentProcess()?.MainModule?.FileName;

                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return Result.Fail<BrandingInfo>("Failed to retrieve executing file name.");
                }

                var result = await _embeddedDataSearcher.TryGetEmbeddedData(filePath);

                if (!result.IsSuccess)
                {
                    return result.HadException ?
                        Result.Fail<BrandingInfo>(result.Exception) :
                        Result.Fail<BrandingInfo>(result.Reason);
                }

                if (!string.IsNullOrWhiteSpace(result.Value.OrganizationId))
                {
                    _orgIdProvider.OrganizationId = result.Value.OrganizationId;
                }

                if (result.Value.ServerUrl is not null)
                {
                    _appState.Host = result.Value.ServerUrl.AbsoluteUri;
                }
            }

            if (string.IsNullOrWhiteSpace(_appState.Host))
            {
                return Result.Fail<BrandingInfo>("ServerUrl is empty.");
            }

            using var httpClient = new HttpClient();

            var brandingUrl = $"{_appState.Host.TrimEnd('/')}/api/branding/{_orgIdProvider.OrganizationId}";
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
