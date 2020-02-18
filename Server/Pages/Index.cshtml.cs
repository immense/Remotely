using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Remotely.Shared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Remotely.Server.Services;

namespace Remotely.Server.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel(DataService dataService, SignInManager<RemotelyUser> signInManager)
        {
            DataService = dataService;
            SignInManager = signInManager;
        }

        public string DefaultPrompt { get; set; }
        public List<SelectListItem> DeviceGroups { get; set; } = new List<SelectListItem>();
        private DataService DataService { get; }
        private SignInManager<RemotelyUser> SignInManager { get; }

        public async Task<IActionResult> OnGet()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                var user = DataService.GetUserByName(User.Identity.Name);
                if (user is null)
                {
                    await SignInManager.SignOutAsync();
                    return RedirectToPage();
                }
                DefaultPrompt = DataService.GetDefaultPrompt(User.Identity.Name);
                var groups = DataService.GetDeviceGroupsForUserName(User.Identity.Name);
                if (groups?.Any() == true)
                {
                    DeviceGroups.AddRange(groups.Select(x => new SelectListItem(x.Name, x.ID)));
                }
            }
            else
            {
                DefaultPrompt = DataService.GetDefaultPrompt();
            }

            return Page();
        }
    }
}
