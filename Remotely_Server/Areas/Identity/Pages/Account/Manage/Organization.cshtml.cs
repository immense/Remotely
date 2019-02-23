using Remotely_Library.Models;
using Remotely_Library.ViewModels;
using Remotely_Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Remotely_Server.Areas.Identity.Pages.Account.Manage
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

        [Display(Name = "Permission Groups")]
        public List<Permission> PermissionList { get; set; }

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
                    Permissions = x?.PermissionGroups?.Select(y => new Permission()
                    {
                        ID = y.ID,
                        Name = y.Name
                    })?.ToList(),
                    UserName = x.UserName
                }).ToList();
            foreach (var user in Users)
            {
                user.Permissions = DataService.GetUserPermissions(User.Identity.Name, user.ID).Select(x => new Permission
                {
                    ID = x.ID,
                    Name = x.Name
                }).ToList();
            }
            var allPermissions = DataService.GetAllPermissions(User.Identity.Name).Select(x => new Permission()
            {
                ID = x.ID,
                Name = x.Name
            }).ToList();
            PermissionList = allPermissions;

            Invites = DataService.GetAllInviteLinks(User.Identity.Name).Select(x => new Invite()
            {
                ID = x.ID,
                InvitedUser = x.InvitedUser,
                IsAdmin = x.IsAdmin,
                DateSent = x.DateSent
            }).ToList();
        }
    }

}