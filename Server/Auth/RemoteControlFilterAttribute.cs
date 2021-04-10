using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Remotely.Server.Services;
using Remotely.Shared.Utilities;
using System;

namespace Remotely.Server.Auth
{
    public class RemoteControlFilterAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        private readonly IApplicationConfig _appConfig;

        public RemoteControlFilterAttribute(IApplicationConfig appConfig)
        {
            _appConfig = appConfig;
        }

        private static MemoryCache OtpCache { get; } = new MemoryCache(new MemoryCacheOptions());
        public static string GetOtp(string deviceId)
        {
            var otp = RandomGenerator.GenerateString(16);
            OtpCache.Set(otp, deviceId, TimeSpan.FromMinutes(1));
            return otp;
        }

        public static bool OtpMatchesDevice(string otp, string deviceId)
        {
            if (OtpCache.TryGetValue(otp, out string cachedDevice) &&
                cachedDevice == deviceId)
            {
                return true;
            }
            return false;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!_appConfig.RemoteControlRequiresAuthentication)
            {
                return;
            }

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                return;
            }

            if (context.HttpContext.Request.Query.TryGetValue("otp", out var otp) &&
                OtpCache.TryGetValue(otp.ToString(), out _))
            {
                return;
            }

            context.Result = new RedirectToPageResult("/Account/Login", new { area = "Identity" });
        }
    }
}
