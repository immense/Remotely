using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Remotely.Server.Services;
using Remotely.Shared.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.Components;

[Authorize]
public class AuthComponentBase : ComponentBase
{
    private readonly ManualResetEventSlim _initSignal = new();
    private RemotelyUser? _user;
    private string? _userName;

    public bool IsAuthenticated { get; private set; }

    public bool IsUserSet => _user is not null;

    public RemotelyUser User
    {
        get
        {
            if (_initSignal.Wait(TimeSpan.FromSeconds(5)) && _user is not null)
            {
                return _user;
            }
            // This should never happen, since AuthBasedComponent is only
            // used on components that require authentication.  This was easier
            // than making this explicitly nullable and refactoring everywhere.
            Logger.LogError("Failed to resolve user.");
            throw new InvalidOperationException("Failed to resolve user.");
        }
        private set => _user = value;
    }

    public string UserName
    {
        get
        {
            if (_initSignal.Wait(TimeSpan.FromSeconds(5)) && _userName is not null)
            {
                return _userName;
            }
            Logger.LogError("Failed to resolve user.");
            throw new InvalidOperationException("Failed to resolve user.");
        }
        private set => _userName = value;
    }

    [Inject]
    protected IAuthService AuthService { get; set; } = null!;

    [Inject]
    private ILogger<AuthComponentBase> Logger { get; init; } = null!;


    protected override async Task OnInitializedAsync()
    {
        IsAuthenticated = await AuthService.IsAuthenticated();
        var userResult = await AuthService.GetUser();
        if (userResult.IsSuccess)
        {
            _user = userResult.Value;
            _userName = userResult.Value.UserName ?? string.Empty;
        }
        _initSignal.Set();
        await base.OnInitializedAsync();
    }
}
