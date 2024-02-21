using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Remotely.Server.Services;
using Remotely.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Auth;

public class TwoFactorRequiredHandler : AuthorizationHandler<TwoFactorRequiredRequirement>
{
    private readonly IDataService _dataService;

    public TwoFactorRequiredHandler(IDataService dataService)
    {
        _dataService = dataService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TwoFactorRequiredRequirement requirement)
    {
        var settings = await _dataService.GetSettings();
        if (context.User.Identity?.IsAuthenticated == true &&
            context.User.Identity.Name is not null &&
            settings.Require2FA)
        {
            var userResult = await _dataService.GetUserByName(context.User.Identity.Name);

            if (!userResult.IsSuccess ||
                !userResult.Value.TwoFactorEnabled)
            {
                context.Fail();
                return;
            }
        }
        context.Succeed(requirement);
    }
}
