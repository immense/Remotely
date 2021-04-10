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
        public LoginController(SignInManager<RemotelyUser> signInManager,
            IDataService dataService,
            IApplicationConfig appConfig,
            IHubContext<CasterHub> casterHubContext,
            IHubContext<ViewerHub> viewerHubContext)
        {
            SignInManager = signInManager;
            DataService = dataService;
            AppConfig = appConfig;
            CasterHubContext = casterHubContext;
            ViewerHubContext = viewerHubContext;
        }

        private SignInManager<RemotelyUser> SignInManager { get; }
        private IDataService DataService { get; }
        public IApplicationConfig AppConfig { get; }
        private IHubContext<CasterHub> CasterHubContext { get; }
        private IHubContext<ViewerHub> ViewerHubContext { get; }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ApiLogin login)
        {
            if (!AppConfig.AllowApiLogin)
            {
                return NotFound();
            }

            var orgId = DataService.GetUserByNameWithOrg(login.Email)?.OrganizationID;

            var result = await SignInManager.PasswordSignInAsync(login.Email, login.Password, false, true);
            if (result.Succeeded)
            {
                DataService.WriteEvent($"API login successful for {login.Email}.", orgId);
                return Ok();
            }
            else if (result.IsLockedOut)
            {
                DataService.WriteEvent($"API login unsuccessful due to lockout for {login.Email}.", orgId);
                return Unauthorized("Account is locked.");
            }
            else if (result.RequiresTwoFactor)
            {
                DataService.WriteEvent($"API login unsuccessful due to 2FA for {login.Email}.", orgId);
                return Unauthorized("Account requires two-factor authentication.");
            }
            DataService.WriteEvent($"API login unsuccessful due to bad attempt for {login.Email}.", orgId);
            return BadRequest();
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            string orgId = null;

            if (HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                orgId = DataService.GetUserByNameWithOrg(HttpContext.User.Identity.Name)?.OrganizationID;
                var activeSessions = CasterHub.SessionInfoList.Where(x => x.Value.RequesterUserName == HttpContext.User.Identity.Name);
                foreach (var session in activeSessions.ToList())
                {
                    await CasterHubContext.Clients.Client(session.Value.CasterSocketID).SendAsync("Disconnect", "User logged out.");
                    await ViewerHubContext.Clients.Client(session.Value.RequesterSocketID).SendAsync("ConnectionFailed");
                }
            }
            await SignInManager.SignOutAsync();
            DataService.WriteEvent($"API logout successful for {HttpContext?.User?.Identity?.Name}.", orgId);
            return Ok();
        }
    }
}
