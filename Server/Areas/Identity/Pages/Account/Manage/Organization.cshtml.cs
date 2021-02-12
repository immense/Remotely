using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Remotely.Server.Services;
using Remotely.Shared.Utilities;
using Remotely.Shared.Models;
using Remotely.Shared.ViewModels.Organization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Remotely_Tests")]

namespace Remotely.Server.Areas.Identity.Pages.Account.Manage
{
    public class OrganizationModel : PageModel
    {
        private readonly IDataService _dataService;
        private readonly IEmailSenderEx _emailSender;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<RemotelyUser> _userManager;

        public OrganizationModel(
            IDataService dataService,
            UserManager<RemotelyUser> userManager,
            IEmailSenderEx emailSender,
            IHttpClientFactory httpClientFactory)
        {
            _dataService = dataService;
            _userManager = userManager;
            _emailSender = emailSender;
            _httpClientFactory = httpClientFactory;
        }


        public RemotelyUser CurrentUser { get; set; }
        public List<DeviceGroup> DeviceGroups { get; } = new List<DeviceGroup>();

        public List<SelectListItem> DeviceGroupSelectItems { get; } = new List<SelectListItem>();

        [BindProperty]
        [Display(Name = "GitHub Username")]
        [StringLength(40)]
        [Required]
        public string GithubUsername { get; set; }

        [BindProperty]
        [Display(Name = "Default Organization")]
        public bool IsDefaultOrganization { get; set; }

        [BindProperty]
        [Display(Name = "Sponsor Unlock Code")]
        [StringLength(24)]
        public string UnlockCode { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();


        [Display(Name = "Invites")]
        public List<Invite> Invites { get; set; }


        [Display(Name = "Organization")]
        public Organization Organization { get; set; }

        [Display(Name = "Organization Name")]
        [StringLength(25)]
        public string OrganizationName { get; set; }
        [TempData]
        public string StatusMessage { get; set; }

        [Display(Name = "Users")]
        public List<OrganizationUser> Users { get; set; }




        public async Task OnGet()
        {
            await PopulateViewModel();
        }

        public async Task<IActionResult> OnPostAddUserAsync()
        {
            var currentUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            return await AddUser(currentUser);
        }

        public async Task<IActionResult> OnPostCreateDeviceGroupAsync()
        {
            CurrentUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (!CurrentUser.IsAdministrator)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                await PopulateViewModel();
                return Page();
            }

            var deviceGroup = new DeviceGroup()
            {
                Name = Input.DeviceGroupName
            };

            var result = _dataService.AddDeviceGroup(CurrentUser.OrganizationID, deviceGroup, out _, out var errorMessage);
            if (!result)
            {
                await PopulateViewModel();
                ModelState.AddModelError("AddDeviceGroup", errorMessage);
                return Page();
            }
            StatusMessage = "Device group created.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDisableAsync()
        {
            CurrentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!CurrentUser.IsAdministrator)
            {
                return Unauthorized();
            }
            await _dataService.DisableSponsorship(CurrentUser.OrganizationID);
            StatusMessage = "Sponsor features disabled successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRegisterSponsorAsync()
        {

            CurrentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!CurrentUser.IsAdministrator)
            {
                return Unauthorized();
            }

            using var client = _httpClientFactory.CreateClient();

            Organization = await _dataService.GetOrganizationByUserName(User.Identity.Name);

            var sponsorInfo = new SponsorInfo()
            {
                GithubUser = GithubUsername,
                UnlockCode = UnlockCode,
                HostName = $"{Request.Scheme}://{Request.Host}",
                OrganizationId = Organization.ID,
                Amount = Organization.SponsorAmount
            };

            var response = await client.PostAsync(AppConstants.SponsorRegistrationUrl, JsonContent.Create(sponsorInfo));
            var responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                StatusMessage = "Successfully registered!  Please keep a backup copy of your unlock code in a safe place.";
                sponsorInfo = JsonSerializer.Deserialize<SponsorInfo>(responseText, JsonSerializerHelper.CaseInsensitiveOptions);
                var isDefaultOrg = IsDefaultOrganization && CurrentUser.IsServerAdmin;
                await _dataService.UpdateOrganizationSponsorInfo(sponsorInfo, isDefaultOrg, true);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                StatusMessage = $"Error: {responseText}";
                await _dataService.UpdateOrganizationSponsorInfo(sponsorInfo, false, false);
            }
            else
            {
                StatusMessage = "There was a problem registering.  Please try again or contact support.";
            }

            return RedirectToPage();
        }

        internal async Task<IActionResult> AddUser(RemotelyUser currentUser)
        {
            if (!currentUser.IsAdministrator)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                if (!_dataService.DoesUserExist(Input.UserEmail))
                {
                    var result = await _dataService.CreateUser(Input.UserEmail, Input.IsAdmin, currentUser.OrganizationID);
                    if (result)
                    {
                        var user = _dataService.GetUserByName(Input.UserEmail);

                        await _userManager.ConfirmEmailAsync(user, await _userManager.GenerateEmailConfirmationTokenAsync(user));

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
                    var newInvite = _dataService.AddInvite(currentUser.OrganizationID, invite);

                    var inviteURL = $"{Request.Scheme}://{Request.Host}/Invite?id={newInvite.ID}";
                    var emailResult = await _emailSender.SendEmailAsync(invite.InvitedUser, "Invitation to Organization in Remotely",
                            $@"<img src='{Request.Scheme}://{Request.Host}/images/Remotely_Logo.png'/>
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
        private async Task PopulateViewModel()
        {
            CurrentUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            Organization = _dataService.GetOrganizationById(CurrentUser.OrganizationID);
            OrganizationName = Organization.OrganizationName;
            GithubUsername = Organization.GithubUser;
            UnlockCode = Organization.UnlockCode;
            IsDefaultOrganization = Organization.IsDefaultOrganization;
            var deviceGroups = _dataService.GetDeviceGroups(User.Identity.Name).OrderBy(x => x.Name);
            DeviceGroups.AddRange(deviceGroups);
            DeviceGroupSelectItems.AddRange(DeviceGroups.Select(x => new SelectListItem(x.Name, x.ID)));

            Users = _dataService.GetAllUsers(User.Identity.Name)
                .Select(x => new OrganizationUser()
                {
                    ID = x.Id,
                    IsAdmin = x.IsAdministrator,
                    UserName = x.UserName
                }).ToList();

            Invites = _dataService.GetAllInviteLinks(User.Identity.Name).Select(x => new Invite()
            {
                ID = x.ID,
                InvitedUser = x.InvitedUser,
                IsAdmin = x.IsAdmin,
                DateSent = x.DateSent
            }).ToList();
        }

        public class InputModel
        {
            [StringLength(200)]
            public string DeviceGroupName { get; set; }

            public bool IsAdmin { get; set; }

            [EmailAddress]
            public string UserEmail { get; set; }
        }
    }

}