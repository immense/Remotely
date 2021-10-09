using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.Localization
{
    public class LocalizationMiddleware : IMiddleware
    {
        private readonly IOptions<RequestLocalizationOptions> _locOptions;
        public LocalizationMiddleware(IOptions<RequestLocalizationOptions> locOptions)
        {
            _locOptions = locOptions;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var cultureKey = context.Request.Headers["Accept-Language"];
            if (!string.IsNullOrEmpty(cultureKey))
            {
                if (DoesCultureExist(cultureKey))
                {
                    var culture = new System.Globalization.CultureInfo(cultureKey);
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
                else
                {
                    var culture = _locOptions.Value.DefaultRequestCulture.Culture;
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
            }

            await next(context);
        }

        private bool DoesCultureExist(string cultureName)
        {
            return _locOptions.Value.SupportedCultures.Any(culture => string.Equals(culture.Name, cultureName, StringComparison.CurrentCultureIgnoreCase));

        }
    }
}
