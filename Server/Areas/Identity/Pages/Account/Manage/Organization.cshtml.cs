using Remotely.Shared.Models;
using Remotely.Shared.ViewModels;
using Remotely.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Remotely.Shared.ViewModels.Organization;

namespace Remotely.Server.Areas.Identity.Pages.Account.Manage
{
    public class OrganizationModel : PageModel
    {
        public OrganizationModel(DataService dataService, UserManager<RemotelyUser> userManager)
        {
            DataService = dataService;
            UserManager = userManager;
        }
        private DataService DataService { get; }
        private UserManager<RemotelyUser> UserManager { get; }

        [Display(Name = "Organization Name")]
        [StringLength(25)]
        public string OrganizationName { get; set; }

        [Display(Name = "Users")]
        public List<OrganizationUser> Users { get; set; }

        [Display(Name = "Invites")]
        public List<Invite> Invites { get; set; }


        public void OnGet()
        {
            OrganizationName = DataService.GetOrganizationName(User.Identity.Name);

            Users = DataService.GetAllUsers(User.Identity.Name)
                .Select(x => new OrganizationUser()
                {
                    ID = x.Id,
                    IsAdmin = x.IsAdministrator,
                    UserName = x.UserName
                }).ToList();

            Invites = DataService.GetAllInviteLinks(User.Identity.Name).Select(x => new Invite()
            {
                ID = x.ID,
                InvitedUser = x.InvitedUser,
                IsAdmin = x.IsAdmin,
                DateSent = x.DateSent,
                ResetUrl = x.ResetUrl
            }).ToList();
        }
    }

}