using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Remotely.Shared.Models;
using Remotely.Server.Data;
using Remotely.Server.Models;
using Remotely.Server.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public LoginController(SignInManager<RemotelyUser> signInManager, DataService dataService, ApplicationConfig appConfig)
        {
            SignInManager = signInManager;
            DataService = dataService;
            AppConfig = appConfig;
        }

        private SignInManager<RemotelyUser> SignInManager { get; }
        private DataService DataService { get; }
        public ApplicationConfig AppConfig { get; }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ApiLogin login)
        {
            if (!AppConfig.AllowApiLogin)
            {
                return NotFound();
            }

            var orgId = DataService.GetUserByName(login.Email)?.OrganizationID;

            var result = await SignInManager.PasswordSignInAsync(login.Email, login.Password, false, true);
            if (result.Succeeded)
            {
                DataService.WriteEvent($"API login successful for {login.Email}.", orgId);
                return Ok();
            }
            else if (result.IsLockedOut)
            {
                DataService.WriteEvent($"API login unsuccessful due to lockout for {login.Email}.", orgId);
                return Unauthorized("Account is locked.");
            }
            else if (result.RequiresTwoFactor)
            {
                DataService.WriteEvent($"API login unsuccessful due to 2FA for {login.Email}.", orgId);
                return Unauthorized("Account requires two-factor authentication.");
            }
            DataService.WriteEvent($"API login unsuccessful due to bad attempt for {login.Email}.", orgId);
            return BadRequest();
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            string orgId = null;

            if (HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                orgId = DataService.GetUserByName(HttpContext.User.Identity.Name)?.OrganizationID;
            }
            await SignInManager.SignOutAsync();
            DataService.WriteEvent($"API logout successful for {HttpContext?.User?.Identity?.Name}.", orgId);
            return Ok();
        }
    }
}
