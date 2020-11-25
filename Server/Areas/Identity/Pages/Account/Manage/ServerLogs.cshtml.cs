using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Remotely.Server.Services;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;

namespace Remotely.Server.Areas.Identity.Pages.Account.Manage
{
    public class ServerLogsModel : PageModel
    {
        public ServerLogsModel(IDataService dataService)
        {
            DataService = dataService;
        }

        public IEnumerable<EventLog> EventLogs { get; private set; }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public bool IsAdmin { get; private set; }
        private IDataService DataService { get; }

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
                var from = Input.FromDate ?? DateTimeOffset.MinValue;
                var to = Input.ToDate ?? DateTimeOffset.MaxValue;
                EventLogs = DataService.GetEventLogs(User.Identity.Name,
                    from,
                    to,
                    Input.Type,
                    Input.Message);
            }
            else
            {
                EventLogs = DataService.GetEventLogs(User.Identity.Name,
                    DateTimeOffset.Now.AddDays(-10),
                    DateTimeOffset.Now,
                    Input.Type,
                    Input.Message);
            }

        }

        public void OnPost()
        {
            PopulateViewModel();
        }

        public class InputModel
        {
            public DateTimeOffset? FromDate { get; set; } = DateTimeOffset.Now.AddDays(-10);
            public string Message { get; set; }
            public DateTimeOffset? ToDate { get; set; } = DateTimeOffset.Now;
            public EventType? Type { get; set; }
        }
    }
}
