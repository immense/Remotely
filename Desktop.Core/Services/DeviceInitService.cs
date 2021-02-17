using Remotely.Desktop.Core.Interfaces;
using Remotely.Shared.Utilities;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Remotely.Desktop.Core.Services
{
    public interface IDeviceInitService
    {
        BrandingInfo BrandingInfo { get; }

        Task GetInitParams();
        void SetBrandingInfo(BrandingInfo branding);
    }

    public class DeviceInitService : IDeviceInitService
    {
        private static BrandingInfo _brandingInfo;

        private readonly Conductor _conductor;
        private readonly IConfigService _configService;
        public DeviceInitService(Conductor conductor, IConfigService configService)
        {
            _conductor = conductor;
            _configService = configService;
        }

        public BrandingInfo BrandingInfo => _brandingInfo;
        public async Task GetInitParams()
        {
            try
            {
                if (_brandingInfo != null)
                {
                    return;
                }

                using var httpClient = new HttpClient();

                var config = _configService.GetConfig();

                var host = _conductor?.Host;
                if (string.IsNullOrWhiteSpace(host))
                {
                    host = config.Host;
                }

                var fileName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName);
                
                if (fileName.Contains("[") && 
                    fileName.Contains("]") && 
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
                                config.Host = host;
                                config.OrganizationId = organizationId;
                                _configService.Save(config);

                                var brandingUrl = $"{config.Host.TrimEnd('/')}/api/branding/{config.OrganizationId}";
                                _brandingInfo = await httpClient.GetFromJsonAsync<BrandingInfo>(brandingUrl).ConfigureAwait(false);
                                return;
                            }
                        }
                    }
                }
                
                if (!string.IsNullOrWhiteSpace(host))
                {
                    config.Host = host;
                    config.OrganizationId = _conductor.OrganizationId;
                    _configService.Save(config);
                    var brandingUrl = $"{host.TrimEnd('/')}/api/branding/{_conductor.OrganizationId}";
                    _brandingInfo = await httpClient.GetFromJsonAsync<BrandingInfo>(brandingUrl).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Failed to resolve init params.", Shared.Enums.EventType.Warning);
            }
        }

        public void SetBrandingInfo(BrandingInfo branding)
        {
            if (branding != null)
            {
                _brandingInfo = branding;
            }
        }
    }
}
