using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System.Threading.Tasks;

namespace Remotely.Server.Components
{
    public class AuthComponentBase : ComponentBase
    {
        protected override async Task OnInitializedAsync()
        {
            IsAuthenticated = await AuthService.IsAuthenticated();
            User = await AuthService.GetUser();
            Username = User?.UserName;
            await base.OnInitializedAsync();
        }

        public bool IsAuthenticated { get; private set; }

        public RemotelyUser User { get; private set; }

        public string Username { get; private set; }

        [Inject]
        protected IAuthService AuthService { get; set; }
    }
}
