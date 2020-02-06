using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Remotely.Server.Services;
using Remotely.Shared.Models;

namespace Remotely.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;
        private readonly SignInManager<RemotelyUser> _signInManager;
        public LogoutModel(SignInManager<RemotelyUser> signInManager,
            ILogger<LogoutModel> logger,
            IHubContext<RCDeviceSocketHub> rcDeviceHub,
            IHubContext<RCBrowserSocketHub> rcBrowserHub)
        {
            _signInManager = signInManager;
            _logger = logger;
            RCDeviceHub = rcDeviceHub;
            RCBrowserHub = rcBrowserHub;
        }

        private IHubContext<RCDeviceSocketHub> RCDeviceHub { get; }
        private IHubContext<RCBrowserSocketHub> RCBrowserHub { get; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var activeSessions = RCDeviceSocketHub.SessionInfoList.Where(x => x.Value.RequesterUserName == HttpContext.User.Identity.Name);
                foreach (var session in activeSessions.ToList())
                {
                    await RCDeviceHub.Clients.Client(session.Value.RCDeviceSocketID).SendAsync("Disconnect", "User logged out.");
                    await RCBrowserHub.Clients.Client(session.Value.RequesterSocketID).SendAsync("ConnectionFailed");
                }
            }
        
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
