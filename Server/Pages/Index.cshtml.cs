using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remotely.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Remotely.Shared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Remotely.Server.Pages
{
    public class IndexModel : PageModel
    {
        private DataService DataService { get; }
        public IndexModel(DataService dataService)
        {
            DataService = dataService;
        }

        public string DefaultPrompt { get; set; }
        public List<SelectListItem> DeviceGroups { get; set; } = new List<SelectListItem>();

        public void OnGet()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
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
        }
    }
}
