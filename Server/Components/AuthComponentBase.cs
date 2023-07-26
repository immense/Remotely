using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Remotely.Server.Services;
using Remotely.Shared.Entities;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Remotely.Server.Components;

public class AuthComponentBase : ComponentBase
{
    private RemotelyUser? _user;
    private string? _userName;

    protected override async Task OnInitializedAsync()
    {
        IsAuthenticated = await AuthService.IsAuthenticated();
        var userResult = await AuthService.GetUser();
        if (userResult.IsSuccess)
        {
            _user = userResult.Value;
            _userName = userResult.Value.UserName ?? string.Empty;
        }
        await base.OnInitializedAsync();
    }

    public bool IsAuthenticated { get; private set; }

    public bool IsUserSet => _user is not null;

    public RemotelyUser User
    {
        get => _user ?? throw new InvalidOperationException("User has not been resolved yet.");
        private set => _user = value;
    }

    public string UserName
    {
        get => _userName ?? throw new InvalidOperationException("User has not been resolved yet.");
        private set => _userName = value;
    }

    [Inject]
    protected IAuthService AuthService { get; set; } = null!;
}
