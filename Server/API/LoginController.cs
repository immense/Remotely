using Immense.RemoteControl.Server.Hubs;
using Immense.RemoteControl.Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<LoginController> _logger;

        public LoginController(
            SignInManager<RemotelyUser> signInManager,
            IDataService dataService,
            IApplicationConfig appConfig,
            IHubContext<DesktopHub> casterHubContext,
            IDesktopHubSessionCache desktopSessionCache,
            IHubContext<ViewerHub> viewerHubContext,
            ILogger<LoginController> logger)
        {
            _signInManager = signInManager;
            _dataService = dataService;
            _appConfig = appConfig;
            _desktopHub = casterHubContext;
            _desktopSessionCache = desktopSessionCache;
            _viewerHub = viewerHubContext;
            _logger = logger;
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            string orgId = null;

            if (HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                orgId = _dataService.GetUserByNameWithOrg(HttpContext.User.Identity.Name)?.OrganizationID;
                var activeSessions = _desktopSessionCache
                    .Sessions
                    .Where(x => x.RequesterUserName == HttpContext.User.Identity.Name);

                foreach (var session in activeSessions)
                {
                    await _desktopHub.Clients.Client(session.DesktopConnectionId).SendAsync("Disconnect", "User logged out.");
                    await _viewerHub.Clients.Clients(session.ViewerList).SendAsync("ConnectionFailed");
                }
            }
            await _signInManager.SignOutAsync();
            _logger.LogInformation("API logout successful for {userName}.", HttpContext?.User?.Identity?.Name);
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
                _logger.LogInformation("API login successful for {loginEmail}.", login.Email);
                return Ok();
            }
            else if (result.IsLockedOut)
            {
                _logger.LogInformation("API login unsuccessful due to lockout for {loginEmail}.", login.Email);
                return Unauthorized("Account is locked.");
            }
            else if (result.RequiresTwoFactor)
            {
                _logger.LogInformation("API login unsuccessful due to 2FA for {loginEmail}.", login.Email);
                return Unauthorized("Account requires two-factor authentication.");
            }
            _logger.LogInformation("API login unsuccessful due to bad attempt for {loginEmail}.", login.Email);
            return BadRequest();
        }
    }
}
