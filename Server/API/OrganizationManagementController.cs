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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [Authorize]
    public class OrganizationManagementController : Controller
    {
        public OrganizationManagementController(DataService dataService, UserManager<RemotelyUser> userManager, EmailSender emailSender)
        {
            this.DataService = dataService;
            this.UserManager = userManager;
            this.EmailSender = emailSender;
        }

        private DataService DataService { get; }
        private EmailSender EmailSender { get; }
        private UserManager<RemotelyUser> UserManager { get; }
        [HttpPost("AddUserPermission/{userID}")]
        public IActionResult AddUserPermission(string userID, [FromBody]string permissionID)
        {
            if (!DataService.GetUserByName(User.Identity.Name).IsAdministrator)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = DataService.AddPermissionToUser(User.Identity.Name, userID, permissionID.Trim(), out var errorMessage);
            if (!result)
            {
                return BadRequest(errorMessage);
            }
            return Ok(permissionID);
        }

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
        [HttpDelete("Permission")]
        public IActionResult Permission([FromBody]string permissionID)
        {
            if (!DataService.GetUserByName(User.Identity.Name).IsAdministrator)
            {
                return Unauthorized();
            }

            DataService.DeletePermission(User.Identity.Name, permissionID.Trim());
            return Ok("ok");
        }

        [HttpPost("Permission")]
        public IActionResult Permission([FromBody]Permission permission)
        {
            if (!DataService.GetUserByName(User.Identity.Name).IsAdministrator)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = DataService.AddPermission(User.Identity.Name, permission, out var permissionID, out var errorMessage);
            if (!result)
            {
                return BadRequest(errorMessage);
            }
            return Ok(permissionID);
        }
        [HttpDelete("RemovePermissionFromUser/{userID}/{permissionID}")]
        public IActionResult RemovePermissionFromUser(string userID, string permissionID)
        {
            if (!DataService.GetUserByName(User.Identity.Name).IsAdministrator)
            {
                return Unauthorized();
            }

            DataService.RemovePermissionFromUser(User.Identity.Name, userID, permissionID.Trim());
            return Ok("ok");
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
                    var resetCode = UrlEncoder.Default.Encode(await UserManager.GeneratePasswordResetTokenAsync(user));
                    var resetUrl = $"{Request.Scheme}://{Request.Host}/Identity/Account/ResetPassword?code={resetCode}";
                    invite.ResetUrl = resetUrl;
                    newUserMessage = $@"<br><br>Since you don't have an account yet, one has been created for you.
                                    You will need to set a password first before attempting to join the organization.<br><br>
                                    Set your password by <a href='{resetUrl}'>clicking here</a>.  Your username/email
                                    is <strong>{invite.InvitedUser}</strong>.";
                }
                else
                {
                    return BadRequest("There was an issue creating the new account.");
                }
            }

            var newInvite = DataService.AddInvite(User.Identity.Name, invite, Request.Scheme + "://" + Request.Host);

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
