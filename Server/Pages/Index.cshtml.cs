using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remotely.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Remotely.Shared.Models;

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

        public void OnGet()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                DefaultPrompt = DataService.GetDefaultPrompt(User.Identity.Name);

            }
            else
            {
                DefaultPrompt = DataService.GetDefaultPrompt();
            }
        }
    }
}
