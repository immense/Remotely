using Remotely.Shared.Models;
using Remotely.Shared.ViewModels;
using Remotely.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
                    Permissions = x?.UserPermissionLinks?.Select(y => new Permission()
                    {
                        ID = y.PermissionGroupID,
                        Name = y.PermissionGroup.Name
                    })?.ToList(),
                    UserName = x.UserName
                }).ToList();

            foreach (var user in Users)
            {
                var permissions = DataService.GetUserPermissions(User.Identity.Name, user.ID);

                if (permissions.Any())
                {
                    user.Permissions = permissions?.Select(x => new Permission
                    {
                        ID = x.PermissionGroupID,
                        Name = x.PermissionGroup?.Name
                    }).ToList();
                }
            }

            PermissionList = DataService.GetAllPermissions(User.Identity.Name).Select(x => new Permission()
            {
                ID = x.ID,
                Name = x.Name
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