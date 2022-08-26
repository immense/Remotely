using Immense.RemoteControl.Server.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Remotely.Shared.Utilities;
using System;

namespace Remotely.Server.Services.RcImplementations
{
    public class ViewerAuthorizer : IViewerAuthorizer
    {
        private static readonly MemoryCache _otpCache = new(new MemoryCacheOptions());

        private readonly IApplicationConfig _appConfig;

        public ViewerAuthorizer(IApplicationConfig appConfig)
        {
            _appConfig = appConfig;
        }

        public string UnauthorizedRedirectArea => "Identity";
        public string UnauthorizedRedirectPageName => "/Account/Login";

        public static string GetOtp(string deviceId)
        {
            var otp = RandomGenerator.GenerateString(16);
            _otpCache.Set(otp, deviceId, TimeSpan.FromMinutes(1));
            return otp;
        }

        public static bool OtpMatchesDevice(string otp, string deviceId)
        {
            if (_otpCache.TryGetValue(otp, out string cachedDevice) &&
                cachedDevice == deviceId)
            {
                return true;
            }
            return false;
        }

        public bool IsAuthorized(AuthorizationFilterContext context)
        {
            if (!_appConfig.RemoteControlRequiresAuthentication)
            {
                return true;
            }

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                return true;
            }

            if (context.HttpContext.Request.Query.TryGetValue("otp", out var otp) &&
                _otpCache.TryGetValue(otp.ToString(), out _))
            {
                return true;
            }

            return false;
        }
    }
}
