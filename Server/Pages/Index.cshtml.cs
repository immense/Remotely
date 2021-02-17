using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Remotely.Server.Services;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Remotely.Server.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IApplicationConfig _appConfig;
        private readonly IDataService _dataService;
        private readonly SignInManager<RemotelyUser> _signInManager;
        private readonly IUpgradeService _upgradeService;
        public IndexModel(IDataService dataService,
            SignInManager<RemotelyUser> signInManager,
            IUpgradeService upgradeService,
            IApplicationConfig appConfig)
        {
            _dataService = dataService;
            _signInManager = signInManager;
            _appConfig = appConfig;
            _upgradeService = upgradeService;
        }

        public List<Alert> Alerts { get; set; } = new List<Alert>();
        public string DefaultPrompt { get; set; }
        public List<SelectListItem> DeviceGroups { get; set; } = new List<SelectListItem>();
        public bool IsNewVersionAvailable { get; set; }
        public string Motd { get; set; }
        public bool RegistrationAvailable { get; set; }

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

                var organizationCount = _dataService.GetOrganizationCount();
                RegistrationAvailable = _appConfig.MaxOrganizationCount < 0 || organizationCount < _appConfig.MaxOrganizationCount;

                var org = _dataService.GetOrganizationById(user.OrganizationID);
                IsNewVersionAvailable = await _upgradeService.IsNewVersionAvailable();

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
