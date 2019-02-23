using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remotely_Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Remotely_Server.Pages
{
    public class IndexModel : PageModel
    {
        private DataService DB { get; }
        public IndexModel(DataService db)
        {
            DB = db;
        }

        public string DefaultPrompt { get; set; }
        public void OnGet()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                DefaultPrompt = DB.GetDefaultPrompt(User.Identity.Name);

            }
            else
            {
                DefaultPrompt = DB.GetDefaultPrompt();
            }
        }
    }
}
