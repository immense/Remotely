using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Remotely.Server.Hubs;
using Remotely.Shared.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;
        private readonly SignInManager<RemotelyUser> _signInManager;
        public LogoutModel(SignInManager<RemotelyUser> signInManager,
            ILogger<LogoutModel> logger,
            IHubContext<CasterHub> casterHubContext,
            IHubContext<ViewerHub> viewerHubContext)
        {
            _signInManager = signInManager;
            _logger = logger;
            CasterHubContext = casterHubContext;
            ViewerHubContext = viewerHubContext;
        }

        private IHubContext<CasterHub> CasterHubContext { get; }
        private IHubContext<ViewerHub> ViewerHubContext { get; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var activeSessions = CasterHub.SessionInfoList.Where(x => x.Value.RequesterUserName == HttpContext.User.Identity.Name);
                foreach (var session in activeSessions.ToList())
                {
                    await CasterHubContext.Clients.Client(session.Value.CasterSocketID).SendAsync("Disconnect", "User logged out.");
                    await ViewerHubContext.Clients.Client(session.Value.RequesterSocketID).SendAsync("ConnectionFailed");
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
