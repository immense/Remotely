using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Localization
{
    public class AcceptLanguageCultureProvider : RequestCultureProvider
    {
        public AcceptLanguageCultureProvider()
        {
            SupperCultureInfos = new[] { new CultureInfo("en-US"),new CultureInfo("zh-CN") };

        }

        public static CultureInfo[] SupperCultureInfos { get; private set; }
        private void FillAllCulture(List<string> cultures, CultureInfo cultureInfo)
        {
            if (!string.IsNullOrEmpty(cultureInfo.Name))
            {
                cultures.Add(cultureInfo.Name);
            }
            if (cultureInfo.Parent != null && !string.IsNullOrEmpty(cultureInfo.Parent.Name))
            {
                FillAllCulture(cultures, cultureInfo.Parent);
            }
        }

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var cultureKey = httpContext.Request.Headers["Accept-Language"];
            try
            {
                var firstculture = cultureKey.ToString().Split(',')[0];
                var cultures = new List<string>();
                FillAllCulture(cultures, new CultureInfo(firstculture));
                if (!string.IsNullOrEmpty(cultureKey))
                {
                    var culture = SupperCultureInfos.Where(culture => cultures.Contains(culture.Name)).ToList();
                    if (culture.Count > 0)
                    {
                        return Task.FromResult(new ProviderCultureResult(culture.Select(d => new StringSegment(d.Name)).ToList()));
                    }
                }
            }
            catch
            {
            }
            return Task.FromResult(new ProviderCultureResult(SupperCultureInfos[0].Name));
        }

    }
}
