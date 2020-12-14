using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Remotely.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<RemotelyUser> _userManager;
        private readonly IEmailSenderEx _emailSender;

        private IDataService DataService { get; }

        public ForgotPasswordModel(UserManager<RemotelyUser> userManager,
            IEmailSenderEx emailSender,
            IDataService dataService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            DataService = dataService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                DataService.WriteEvent($"Sending password reset for user {user.UserName}. Reset URL: {callbackUrl}", user.OrganizationID);

                var emailResult = await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Reset Password",
                    $"<img src='https://remotely.one/media/Remotely_Logo.png'/><br><br>Please reset your Remotely password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                if (!emailResult)
                {
                    ModelState.AddModelError("EmailError", "Error sending email.");
                    return Page();
                }


                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
