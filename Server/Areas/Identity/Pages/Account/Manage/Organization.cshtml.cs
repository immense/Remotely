using Remotely.Shared.Models;
using Remotely.Shared.ViewModels;
using Remotely.Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Remotely.Shared.ViewModels.Organization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Remotely.Server.Areas.Identity.Pages.Account.Manage
{
    public class OrganizationModel : PageModel
    {
        public OrganizationModel(DataService dataService)
        {
            DataService = dataService;
        }
        public List<SelectListItem> DeviceGroups { get; } = new List<SelectListItem>();

        [Display(Name = "Invites")]
        public List<Invite> Invites { get; set; }

        [Display(Name = "Organization Name")]
        [StringLength(25)]
        public string OrganizationName { get; set; }

        [Display(Name = "Users")]
        public List<OrganizationUser> Users { get; set; }

        private DataService DataService { get; }
        public void OnGet()
        {
            OrganizationName = DataService.GetOrganizationName(User.Identity.Name);

            var groups = DataService.GetDeviceGroupsForUserName(User.Identity.Name);
            DeviceGroups.AddRange(groups.Select(x => new SelectListItem(x.Name, x.ID)));

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