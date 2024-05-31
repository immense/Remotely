using Microsoft.AspNetCore.Authorization;
using Remotely.Server.Models;
using Remotely.Server.Services;
using System.Security.Principal;

namespace Remotely.Server.Auth;

public class TwoFactorRequiredHandler(
    IHttpContextAccessor _contextAccessor,
    IDataService _dataService) : AuthorizationHandler<TwoFactorRequiredRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TwoFactorRequiredRequirement requirement)
    {
        var settings = await _dataService.GetSettings();
        if (context.User?.Identity is { } identity &&
            IsTwoFactorRequired(identity, settings))
        {
            var userResult = await _dataService.GetUserByName(identity.Name!);

            if (!userResult.IsSuccess ||
                !userResult.Value.TwoFactorEnabled)
            {
                context.Fail();
                return;
            }
        }
        context.Succeed(requirement);
    }

    private bool IsTwoFactorRequired(IIdentity identity, SettingsModel settings)
    {
        // Account management pages are exempt since they're required
        // to set up 2FA.
        var path = _contextAccessor.HttpContext?.Request.Path ?? "";
        if (path.StartsWithSegments("/Account/Manage"))
        {
            return false;
        }

        return 
            settings.Require2FA &&
            identity.IsAuthenticated &&
            identity.Name is not null;
    }
}
