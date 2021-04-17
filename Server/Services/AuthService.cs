using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Remotely.Server.Services
{
    public interface IAuthService
    {
        Task<bool> IsAuthenticated();
        Task<RemotelyUser> GetUser();
    }

    public class AuthService : IAuthService
    {
        private readonly AuthenticationStateProvider _authProvider;
        private readonly IDataService _dataService;

        public AuthService(
            AuthenticationStateProvider authProvider,
            IDataService dataService)
        {
            _authProvider = authProvider;
            _dataService = dataService;
        }

        public async Task<bool> IsAuthenticated()
        {
            var principal = await _authProvider.GetAuthenticationStateAsync();
            return principal?.User?.Identity?.IsAuthenticated ?? false;
        }

        public async Task<RemotelyUser> GetUser()
        {
            var principal = await _authProvider.GetAuthenticationStateAsync();

            if (principal?.User?.Identity?.IsAuthenticated == true)
            {
                return await _dataService.GetUserAsync(principal.User.Identity.Name);
            }

            return null;
        }
    }
}
