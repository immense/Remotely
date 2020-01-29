using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Remotely.Shared.Models;
using Remotely.Shared.ViewModels;
using Remotely.Server.Data;
using Remotely.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Remotely.Shared.ViewModels.Organization;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OrganizationManagementController : ControllerBase
    {
        public OrganizationManagementController(DataService dataService, UserManager<RemotelyUser> userManager, IEmailSender emailSender)
        {
            this.DataService = dataService;
            this.UserManager = userManager;
            this.EmailSender = emailSender;
        }

        private DataService DataService { get; }
        private IEmailSender EmailSender { get; }
        private UserManager<RemotelyUser> UserManager { get; }


        [HttpPost("ChangeIsAdmin/{userID}")]
        public IActionResult ChangeIsAdmin(string userID, [FromBody]bool isAdmin)
        {
            if (!DataService.GetUserByName(User.Identity.Name).IsAdministrator)
            {
                return Unauthorized();
            }

            if (DataService.GetUserByName(User.Identity.Name).Id == userID)
            {
                return BadRequest("You can't remove administrator rights from yourself.");
            }

            DataService.ChangeUserIsAdmin(User.Identity.Name, userID, isAdmin);
            return Ok("ok");
        }

        [HttpDelete("DeleteInvite/{inviteID}")]
        public IActionResult DeleteInvite(string inviteID)
        {
            if (!DataService.GetUserByName(User.Identity.Name).IsAdministrator)
            {
                return Unauthorized();
            }
            DataService.DeleteInvite(User.Identity.Name, inviteID);
            return Ok("ok");
        }

        [HttpPut("Name")]
        public IActionResult Name([FromBody]string organizationName)
        {
            if (!DataService.GetUserByName(User.Identity.Name).IsAdministrator)
            {
                return Unauthorized();
            }
            if (organizationName.Length > 25)
            {
                return BadRequest();
            }
            DataService.UpdateOrganizationName(User.Identity.Name, organizationName.Trim());
            return Ok("ok");
        }

        [HttpDelete("DeviceGroup")]
        public IActionResult DeviceGroup([FromBody]string deviceGroupID)
        {
            if (!DataService.GetUserByName(User.Identity.Name).IsAdministrator)
            {
                return Unauthorized();
            }

            DataService.DeleteDeviceGroup(User.Identity.Name, deviceGroupID.Trim());
            return Ok("ok");
        }

        [HttpPost("DeviceGroup")]
        public IActionResult DeviceGroup([FromBody]DeviceGroup deviceGroup)
        {
            if (!DataService.GetUserByName(User.Identity.Name).IsAdministrator)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = DataService.AddDeviceGroup(User.Identity.Name, deviceGroup, out var deviceGroupID, out var errorMessage);
            if (!result)
            {
                return BadRequest(errorMessage);
            }
            return Ok(deviceGroupID);
        }
        [HttpDelete("RemoveUserFromOrganization/{userID}")]
        public IActionResult RemoveUserFromOrganization(string userID)
        {
            if (!DataService.GetUserByName(User.Identity.Name).IsAdministrator)
            {
                return Unauthorized();
            }

            DataService.RemoveUserFromOrganization(User.Identity.Name, userID);
            return Ok("ok");
        }

        [HttpPost("SendInvite")]
        public async Task<IActionResult> SendInvite([FromBody]Invite invite)
        {
            if (!DataService.GetUserByName(User.Identity.Name).IsAdministrator)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var newUserMessage = "";
            if (!DataService.DoesUserExist(invite.InvitedUser))
            {           
                var user = new RemotelyUser { UserName = invite.InvitedUser, Email = invite.InvitedUser };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    user = await UserManager.FindByEmailAsync(invite.InvitedUser);

                    await UserManager.ConfirmEmailAsync(user, await UserManager.GenerateEmailConfirmationTokenAsync(user));

                    var code = await UserManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ResetPassword",
                        pageHandler: null,
                        values: new { area = "Identity", code },
                        protocol: Request.Scheme);

                    invite.ResetUrl = callbackUrl;

                    newUserMessage = $@"<br><br>Since you don't have an account yet, one has been created for you.
                                    You will need to set a password first before attempting to join the organization.<br><br>
                                    Set your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.  Your username/email
                                    is <strong>{invite.InvitedUser}</strong>.";
                }
                else
                {
                    return BadRequest("There was an issue creating the new account.");
                }
            }

            var newInvite = DataService.AddInvite(User.Identity.Name, invite);

            var inviteURL = $"{Request.Scheme}://{Request.Host}/Invite?id={newInvite.ID}";
            await EmailSender.SendEmailAsync(invite.InvitedUser, "Invitation to Organization in Remotely",
                        $@"<img src='https://remotely.lucency.co/images/Remotely_Logo.png'/>
                            <br><br>
                            Hello!
                            <br><br>
                            You've been invited by {User.Identity.Name} to join an organization in Remotely.
                            {newUserMessage}
                            <br><br>
                            You can join the organization by <a href='{HtmlEncoder.Default.Encode(inviteURL)}'>clicking here</a>.");

            return Ok(newInvite);
        }
    }
}
