using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Remotely.Server.Services;

namespace Remotely.Server.Filters;

internal class ViewerAuthorizationFilter(
    IDataService _dataService,
    IOtpProvider _otpProvider):  IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (await IsAuthorized(context))
        {
            return;
        }

        context.Result = new RedirectResult("/Account/Login");
    }

    private async Task<bool> IsAuthorized(AuthorizationFilterContext context)
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
