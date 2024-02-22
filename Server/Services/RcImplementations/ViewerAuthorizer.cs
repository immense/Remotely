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
    private readonly IDataService _dataService;
    private readonly IOtpProvider _otpProvider;

    public ViewerAuthorizer(IDataService dataService, IOtpProvider otpProvider)
    {
        _dataService = dataService;
        _otpProvider = otpProvider;
    }

    public string UnauthorizedRedirectUrl { get; } = "/Account/Login";

    public async Task<bool> IsAuthorized(AuthorizationFilterContext context)
    {
        var settings = await _dataService.GetSettings();
        if (!settings.RemoteControlRequiresAuthentication)
        {
            return true;
        }

        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            return true;
        }

        if (context.HttpContext.Request.Query.TryGetValue("otp", out var otp) &&
            _otpProvider.Exists($"{otp}"))
        {
            return true;
        }

        return false;
    }
}
