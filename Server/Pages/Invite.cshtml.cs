using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remotely.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Remotely.Server.Pages
{
    [Authorize]
    public class InviteModel : PageModel
    {
        public InviteModel(DataService dataService)
        {
            this.DataService = dataService;
        }
        private DataService DataService { get; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public void OnGet(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Success = false;
                Message = "No invititation ID is specified.";
                return;
            }
            var result = DataService.JoinViaInvitation(User.Identity.Name, id);
            if (result == false)
            {
                Success = false;
                Message = "The invitation ID wasn't found or is for another account.";
                return;
            }
            
            Success = true;
            Message = "You've successfully joined the organization.";
        }

    }
}