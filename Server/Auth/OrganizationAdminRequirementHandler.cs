using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Remotely.Shared.Models;
using System.Threading.Tasks;

namespace Remotely.Server.Auth;

public class OrganizationAdminRequirementHandler : AuthorizationHandler<OrganizationAdminRequirement>
{
    private readonly UserManager<RemotelyUser> _userManager;

    public OrganizationAdminRequirementHandler(UserManager<RemotelyUser> userManager)
    {
        _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OrganizationAdminRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Fail();
            return;
        }

        var user = await _userManager.GetUserAsync(context.User);
        if (user?.IsAdministrator != true)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}
