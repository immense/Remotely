#nullable enable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Remotely.Shared.Entities;
using System.Threading.Tasks;

namespace Remotely.Server.Auth;

public class ServerAdminRequirementHandler : AuthorizationHandler<ServerAdminRequirement>
{
    private readonly UserManager<RemotelyUser> _userManager;

    public ServerAdminRequirementHandler(UserManager<RemotelyUser> userManager)
    {
        _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ServerAdminRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Fail();
            return;
        }

        var user = await _userManager.GetUserAsync(context.User);
        if (user?.IsServerAdmin != true)
        {
            context.Fail();
            return;
        }
        
        context.Succeed(requirement);
    }
}
