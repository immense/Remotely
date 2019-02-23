using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remotely_Library.Models;
using Remotely_Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Remotely_Server.Areas.Identity.Pages.Account.Manage
{
    public class OptionsModel : PageModel
    {
        public OptionsModel(DataService dataService)
        {
            this.DataService = dataService;
        }
        private DataService DataService { get; set; }

        [TempData]
        public string Message { get; set; }

        public RemotelyUserOptions Options { get; set; }

        public void OnGet()
        {
            Options = DataService.GetUserOptions(User.Identity.Name);
        }

        public IActionResult OnPost(Remotely_Library.Models.RemotelyUserOptions options)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            DataService.UpdateUserOptions(User.Identity.Name, options);
            Message = "Saved successfully.";
            return RedirectToPage();
        }
    }
}