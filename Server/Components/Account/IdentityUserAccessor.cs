using Remotely.Server.Data;
using Microsoft.AspNetCore.Identity;
using Remotely.Shared.Entities;

namespace Remotely.Server.Components.Account;

internal sealed class IdentityUserAccessor(UserManager<RemotelyUser> userManager, IdentityRedirectManager redirectManager)
{
    public async Task<RemotelyUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }

        return user;
    }
}
