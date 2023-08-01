using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Remotely.Server.Services;
using Remotely.Shared.Entities;
using System.Threading.Tasks;

namespace Remotely.Server.Auth;

public class OrganizationAdminRequirementHandler : AuthorizationHandler<OrganizationAdminRequirement>
{
    private readonly IDataService _dataService;

    public OrganizationAdminRequirementHandler(IDataService dataService)
    {
        _dataService = dataService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OrganizationAdminRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true ||
            string.IsNullOrWhiteSpace(context.User.Identity.Name))
        {
            context.Fail();
            return;
        }

        var userResult = await _dataService.GetUserByName(context.User.Identity.Name);

        if (!userResult.IsSuccess ||
            !userResult.Value.IsAdministrator)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}
