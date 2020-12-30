﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Attributes;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Remotely.Server.API
{
    public class RemotelyInfo
    { 
        public string AuthHeader { get; set; }
        public string OrganizationID { get; set; }
    }
    public class RegUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class TheopenemController : ControllerBase
    {

        public TheopenemController(IDataService dataService,
          UserManager<RemotelyUser> userManager,
          IEmailSenderEx emailSender,
          IApplicationConfig appConfig)
        {
            DataService = dataService;
            UserManager = userManager;
            AppConfig = appConfig;
        }

        private IDataService DataService { get; }

        private IApplicationConfig AppConfig { get; }
        private UserManager<RemotelyUser> UserManager { get; }

        [HttpPost("CreateFirstUser")]
        public async Task<IActionResult> CreateFirstUser([FromBody] RegUser regUser)
        {
            var organizationCount = DataService.GetOrganizationCount();
            if (AppConfig.MaxOrganizationCount > 0 && organizationCount >= AppConfig.MaxOrganizationCount)
            {
                return NotFound();
            }

            var user = new RemotelyUser
            {
                UserName = $"{regUser.Username}@localhost",
                Email = $"{regUser.Username}@localhost",
                IsServerAdmin = organizationCount == 0,
                Organization = new Organization(),
                UserOptions = new RemotelyUserOptions(),
                IsAdministrator = true
            };
            var result = await UserManager.CreateAsync(user, regUser.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            else
            {
                //confirm email, not really needed
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                await UserManager.ConfirmEmailAsync(user, code);

                //create api token

                var secret = PasswordGenerator.GeneratePassword(24);
                var secretHash = new PasswordHasher<RemotelyUser>().HashPassword(null, secret);

                var newToken = await DataService.CreateApiToken(user.UserName, "Theopenem", secretHash);
                var NewTokenKey = Guid.Parse(newToken.Token);
                var NewTokenSecret = secret;
                var toemsInfo = new RemotelyInfo();
                toemsInfo.AuthHeader = $"{NewTokenKey}:{NewTokenSecret}";
                toemsInfo.OrganizationID = user.Organization.ID;
                return Ok(toemsInfo);
            }

        }

        [HttpGet("IsDeviceOnline")]
        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        public bool IsDeviceOnline(string deviceID)
        {
            var device = DataService.GetDevice(deviceID);
            if (device != null)
                return device.IsOnline;
            return false;
        }

        [HttpGet("Status")]
        public bool Status()
        {
            return true;
        }
    }
}