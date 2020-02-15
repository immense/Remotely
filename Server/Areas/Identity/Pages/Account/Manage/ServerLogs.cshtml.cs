using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Remotely.Server.Services;
using Remotely.Shared.Models;

namespace Remotely.Server.Areas.Identity.Pages.Account.Manage
{
    public class ServerLogsModel : PageModel
    {
        public ServerLogsModel(DataService dataService)
        {
            DataService = dataService;
        }

        public IEnumerable<EventLog> EventLogs { get; private set; }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public bool IsAdmin { get; private set; }
        private DataService DataService { get; }

        public void OnGet()
        {
            PopulateViewModel();
        }

        private void PopulateViewModel()
        {
            var currentUser = DataService.GetUserByName(User.Identity.Name);
            IsAdmin = currentUser.IsAdministrator;

            if (Input.FromDate.HasValue || Input.ToDate.HasValue)
            {
                var from = Input.FromDate ?? DateTime.MinValue;
                var to = Input.ToDate ?? DateTime.MaxValue;
                EventLogs = DataService.GetEventLogs(User.Identity.Name, from, to);
            }
            else
            {
                EventLogs = DataService.GetEventLogs(User.Identity.Name, DateTime.Now.AddDays(-10), DateTime.Now);
            }

        }

        public void OnPost()
        {
            PopulateViewModel();
        }

        public class InputModel
        {
            public DateTime? FromDate { get; set; } = DateTime.Now.AddDays(-10);
            public DateTime? ToDate { get; set; } = DateTime.Now;
        }
    }
}
