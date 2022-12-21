using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Immense.RemoteControl.Desktop.Shared.Services;
using Immense.RemoteControl.Shared.Models;
using Remotely.Shared;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Desktop.Shared.Services
{
    public class BrandingProvider : IBrandingProvider
    {
        private readonly IAppState _appState;
        private readonly IOrganizationIdProvider _orgIdProvider;
        private BrandingInfoBase _brandingInfo = new()
        {
            Product = "Remote Control"
        };

        public BrandingProvider(IAppState appState, IOrganizationIdProvider orgIdProvider)
        {
            _appState = appState;
            _orgIdProvider = orgIdProvider;

            using var mrs = typeof(BrandingProvider).Assembly.GetManifestResourceStream("Desktop.Shared.Assets.Remotely_Icon.png");
            using var ms = new MemoryStream();
            mrs!.CopyTo(ms);

            _brandingInfo.Icon = ms.ToArray();
        }

        public Task<BrandingInfoBase> GetBrandingInfo()
        {
            return Task.FromResult(_brandingInfo);
        }

        public void SetBrandingInfo(BrandingInfoBase brandingInfo)
        {
            _brandingInfo = brandingInfo;
        }

        public async Task TrySetFromApi()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_appState.Host))
                {
                    return;
                }

                var host = _appState.Host;

                using var httpClient = new HttpClient();

                var fileName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess()?.MainModule?.FileName);

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return;
                }

                if (fileName.Contains('[') &&
                    fileName.Contains(']') &&
                    !string.IsNullOrWhiteSpace(host))
                {
                    var codeLength = AppConstants.RelayCodeLength + 2;

                    for (var i = 0; i < fileName.Length; i++)
                    {
                        var codeSection = string.Join("", fileName.Skip(i).Take(codeLength));
                        if (codeSection.StartsWith("[") && codeSection.EndsWith("]"))
                        {
                            var relayCode = codeSection[1..5];

                            using var response = await httpClient.GetAsync($"{host.TrimEnd('/')}/api/Relay/{relayCode}").ConfigureAwait(false);
                            if (response.IsSuccessStatusCode)
                            {
                                var organizationId = await response.Content.ReadAsStringAsync();
                                _orgIdProvider.OrganizationId = organizationId;

                                var brandingUrl = $"{host.TrimEnd('/')}/api/branding/{organizationId}";
                                var result = await httpClient.GetFromJsonAsync<BrandingInfo>(brandingUrl).ConfigureAwait(false);
                                if (result is not null)
                                {
                                    _brandingInfo = result;
                                }
                                return;
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(_orgIdProvider.OrganizationId))
                {
                    var brandingUrl = $"{host.TrimEnd('/')}/api/branding/{_orgIdProvider.OrganizationId}";
                    var result = await httpClient.GetFromJsonAsync<BrandingInfo>(brandingUrl).ConfigureAwait(false);
                    if (result is not null)
                    {
                        _brandingInfo = result;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Failed to resolve init params.", EventType.Warning);
            }
        }

    }
}
