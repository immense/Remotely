using Immense.RemoteControl.Server.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Remotely.Shared.Utilities;
using System;
using System.Threading.Tasks;

namespace Remotely.Server.Services.RcImplementations;

public class ViewerAuthorizer : IViewerAuthorizer
{
    private readonly IApplicationConfig _appConfig;
    private readonly IOtpProvider _otpProvider;

    public ViewerAuthorizer(IApplicationConfig appConfig, IOtpProvider otpProvider)
    {
        _appConfig = appConfig;
        _otpProvider = otpProvider;
    }

    public string UnauthorizedRedirectUrl { get; } = "/Identity/Account/Login";

    public Task<bool> IsAuthorized(AuthorizationFilterContext context)
    {
        if (!_appConfig.RemoteControlRequiresAuthentication)
        {
            return Task.FromResult(true);
        }

        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            return Task.FromResult(true);
        }

        if (context.HttpContext.Request.Query.TryGetValue("otp", out var otp) &&
            _otpProvider.Exists($"{otp}"))
        {
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}
