using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using Remotely.Shared.ViewModels.Organization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Remotely.Server.Areas.Identity.Pages.Account.Manage
{
    public class OrganizationModel : PageModel
    {
        public OrganizationModel(IDataService dataService,
            UserManager<RemotelyUser> userManager,
            IEmailSenderEx emailSender)
        {
            DataService = dataService;
            UserManager = userManager;
            EmailSender = emailSender;
        }
        public List<SelectListItem> DeviceGroupSelectItems { get; } = new List<SelectListItem>();
        public List<DeviceGroup> DeviceGroups { get; } = new List<DeviceGroup>();

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [Display(Name = "Invites")]
        public List<Invite> Invites { get; set; }

        [Display(Name = "Organization Name")]
        [StringLength(25)]
        public string OrganizationName { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [Display(Name = "Users")]
        public List<OrganizationUser> Users { get; set; }

        private IDataService DataService { get; }

        private IEmailSenderEx EmailSender { get; }

        private UserManager<RemotelyUser> UserManager { get; }

        public void OnGet()
        {
            PopulateViewModel();
        }

        public async Task<IActionResult> OnPostAddUserAsync()
        {
            var currentUser = await UserManager.FindByEmailAsync(User.Identity.Name);
            return await AddUser(currentUser);
        }

        public async Task<IActionResult> OnPostCreateDeviceGroupAsync()
        {
            var currentUser = await UserManager.FindByEmailAsync(User.Identity.Name);
            if (!currentUser.IsAdministrator)
            {
                return RedirectToPage("Index");
            }

            if (ModelState.IsValid)
            {
                var deviceGroup = new DeviceGroup()
                {
                    Name = Input.DeviceGroupName
                };

                var result = DataService.AddDeviceGroup(currentUser.OrganizationID, deviceGroup, out _, out var errorMessage);
                if (!result)
                {
                    PopulateViewModel();
                    ModelState.AddModelError("AddDeviceGroup", errorMessage);
                    return Page();
                }
                StatusMessage = "Device group created.";
                return RedirectToPage();
            }
            PopulateViewModel();
            return Page();
        }

        public async Task<IActionResult> AddUser(RemotelyUser currentUser)
        {
            if (!currentUser.IsAdministrator)
            {
                return RedirectToPage("Index");
            }

            if (ModelState.IsValid)
            {
                if (!DataService.DoesUserExist(Input.UserEmail))
                {
                    var result = await DataService.CreateUser(Input.UserEmail, Input.IsAdmin, currentUser.OrganizationID);
                    if (result)
                    {
                        var user = DataService.GetUserByName(Input.UserEmail);

                        await UserManager.ConfirmEmailAsync(user, await UserManager.GenerateEmailConfirmationTokenAsync(user));

                        StatusMessage = "User account created.";
                        return RedirectToPage();
                    }
                    else
                    {
                        ModelState.AddModelError("CreateUser", "Failed to create user account.");
                        return Page();
                    }
                }
                else
                {

                    var invite = new Invite()
                    {
                        InvitedUser = Input.UserEmail,
                        IsAdmin = Input.IsAdmin
                    };
                    var newInvite = DataService.AddInvite(currentUser.OrganizationID, invite);

                    var inviteURL = $"{Request.Scheme}://{Request.Host}/Invite?id={newInvite.ID}";
                    var emailResult = await EmailSender.SendEmailAsync(invite.InvitedUser, "Invitation to Organization in Remotely",
                            $@"<img src='https://remotely.one/media/Remotely_Logo.png'/>
                            <br><br>
                            Hello!
                            <br><br>
                            You've been invited to join an organization in Remotely.
                            <br><br>
                            You can join the organization by <a href='{HtmlEncoder.Default.Encode(inviteURL)}'>clicking here</a>.",
                            currentUser.OrganizationID);
                    if (emailResult)
                    {
                        StatusMessage = "Invitation sent.";
                    }
                    else
                    {
                        StatusMessage = "Error sending invititation email.";
                    }

                    return RedirectToPage();
                }
            }
            return Page();
        }

        private void PopulateViewModel()
        {
            OrganizationName = DataService.GetOrganizationName(User.Identity.Name);
            var deviceGroups = DataService.GetDeviceGroups(User.Identity.Name).OrderBy(x => x.Name);
            DeviceGroups.AddRange(deviceGroups);
            DeviceGroupSelectItems.AddRange(DeviceGroups.Select(x => new SelectListItem(x.Name, x.ID)));

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
                DateSent = x.DateSent
            }).ToList();
        }

        public class InputModel
        {
            public bool IsAdmin { get; set; }

            [EmailAddress]
            public string UserEmail { get; set; }

            [StringLength(200)]
            public string DeviceGroupName { get; set; }
        }
    }

}