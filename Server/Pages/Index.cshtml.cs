using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IApplicationConfig _appConfig;

        private readonly IDataService _dataService;

        private readonly SignInManager<RemotelyUser> _signInManager;

        public IndexModel(IDataService dataService,
            SignInManager<RemotelyUser> signInManager,
            IApplicationConfig appConfig)
        {
            _dataService = dataService;
            _signInManager = signInManager;
            _appConfig = appConfig;
        }

        public List<Alert> Alerts { get; set; } = new List<Alert>();
        public string DefaultPrompt { get; set; }
        public string Motd { get; set; }
        public List<SelectListItem> DeviceGroups { get; set; } = new List<SelectListItem>();
        public async Task<IActionResult> OnGet()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                var user = _dataService.GetUserByName(User.Identity.Name);
                if (user is null)
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToPage();
                }

                if (_appConfig.Require2FA && !user.TwoFactorEnabled)
                {
                    return RedirectToPage("TwoFactorRequired");
                }

                DefaultPrompt = _dataService.GetDefaultPrompt(User.Identity.Name);
                var groups = _dataService.GetDeviceGroups(User.Identity.Name);
                if (groups?.Any() == true)
                {
                    DeviceGroups.AddRange(groups.Select(x => new SelectListItem(x.Name, x.ID)));
                }
                var alerts = _dataService.GetAlerts(user.Id);
                if (alerts.Any())
                {
                    Alerts.AddRange(alerts);
                }

                Motd = _appConfig.MessageOfTheDay;
            }
            else
            {
                DefaultPrompt = _dataService.GetDefaultPrompt();
            }

            return Page();
        }
    }
}
