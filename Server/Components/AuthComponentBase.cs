using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Remotely.Server.Services;
using Remotely.Shared.Extensions;
using Remotely.Shared.Models;

namespace Remotely.Server.Components
{
    public class AuthComponentBase : ComponentBase
    {
        public bool IsAuthenticated => AuthService?.IsAuthenticated ?? false;

        public RemotelyUser User => AuthService?.User;

        public string Username => AuthService?.Principal?.Identity?.Name;

        [Inject]
        protected IAuthService AuthService { get; set; }
    }
}
