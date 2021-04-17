using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Remotely.Shared.Extensions;
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
        bool IsAuthenticated { get; }
        ClaimsPrincipal Principal { get; }
        RemotelyUser User { get; }
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

        public ClaimsPrincipal Principal => _authProvider.GetAuthenticationStateAsync()
            .ToResult()?
            .User;

        public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

        public RemotelyUser User => Principal is not null ?
            _dataService.GetUserByNameWithOrg(Principal.Identity.Name) :
            null;
    }
}
