using Immense.RemoteControl.Server.Hubs;
using Immense.RemoteControl.Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Remotely.Server.Hubs;
using Remotely.Server.Models;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Obsolete("This controller is here only for legacy purposes.  For new integrations, use API tokens.")]
    public class LoginController : ControllerBase
    {
        private readonly IApplicationConfig _appConfig;
        private readonly IDataService _dataService;
        private readonly IHubContext<DesktopHub> _desktopHub;
        private readonly IDesktopHubSessionCache _desktopSessionCache;
        private readonly SignInManager<RemotelyUser> _signInManager;
        private readonly IHubContext<ViewerHub> _viewerHub;

        public LoginController(
            SignInManager<RemotelyUser> signInManager,
            IDataService dataService,
            IApplicationConfig appConfig,
            IHubContext<DesktopHub> casterHubContext,
            IDesktopHubSessionCache desktopSessionCache,
            IHubContext<ViewerHub> viewerHubContext)
        {
            _signInManager = signInManager;
            _dataService = dataService;
            _appConfig = appConfig;
            _desktopHub = casterHubContext;
            _desktopSessionCache = desktopSessionCache;
            _viewerHub = viewerHubContext;
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            string orgId = null;

            if (HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                orgId = _dataService.GetUserByNameWithOrg(HttpContext.User.Identity.Name)?.OrganizationID;
                var activeSessions = _desktopSessionCache.Sessions.Where(x => x.Value.RequesterUserName == HttpContext.User.Identity.Name);
                foreach (var session in activeSessions.ToList())
                {
                    await _desktopHub.Clients.Client(session.Value.DesktopConnectionId).SendAsync("Disconnect", "User logged out.");
                    await _viewerHub.Clients.Clients(session.Value.ViewerList).SendAsync("ConnectionFailed");
                }
            }
            await _signInManager.SignOutAsync();
            _dataService.WriteEvent($"API logout successful for {HttpContext?.User?.Identity?.Name}.", orgId);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ApiLogin login)
        {
            if (!_appConfig.AllowApiLogin)
            {
                return NotFound();
            }

            var orgId = _dataService.GetUserByNameWithOrg(login.Email)?.OrganizationID;

            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, true);
            if (result.Succeeded)
            {
                _dataService.WriteEvent($"API login successful for {login.Email}.", orgId);
                return Ok();
            }
            else if (result.IsLockedOut)
            {
                _dataService.WriteEvent($"API login unsuccessful due to lockout for {login.Email}.", orgId);
                return Unauthorized("Account is locked.");
            }
            else if (result.RequiresTwoFactor)
            {
                _dataService.WriteEvent($"API login unsuccessful due to 2FA for {login.Email}.", orgId);
                return Unauthorized("Account requires two-factor authentication.");
            }
            _dataService.WriteEvent($"API login unsuccessful due to bad attempt for {login.Email}.", orgId);
            return BadRequest();
        }
    }
}
