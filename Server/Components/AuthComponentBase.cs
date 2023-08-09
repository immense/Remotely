using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Remotely.Server.Services;
using Remotely.Shared.Entities;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.Components;

[Authorize]
public class AuthComponentBase : MessengerSubscriber
{
    [Inject]
    protected IAuthService AuthService { get; set; } = null!;

    protected RemotelyUser? User { get; private set; }

    protected string? UserName => User?.UserName;

    [MemberNotNull(nameof(User), nameof(UserName))]
    protected void EnsureUserSet()
    {
        if (User is null)
        {
            throw new InvalidOperationException("User has not been set.");
        }

        if (UserName is null)
        {
            throw new InvalidOperationException("UserName has not been set.");
        }
    }

    protected override async Task OnInitializedAsync()
    {
        var userResult = await AuthService.GetUser();
        if (userResult.IsSuccess)
        {
            User = userResult.Value;
        }
        await base.OnInitializedAsync();
    }
}
