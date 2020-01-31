using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Remotely.Shared.Models;
using Remotely.Server.Data;
using Remotely.Server.Models;
using Remotely.Server.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemoteControlController : ControllerBase
    {
        public RemoteControlController(DataService dataService, IHubContext<DeviceSocketHub> deviceHub, ApplicationConfig appConfig, SignInManager<RemotelyUser> signInManager)
        {
            DataService = dataService;
            DeviceHub = deviceHub;
            AppConfig = appConfig;
            SignInManager = signInManager;
        }

        public DataService DataService { get; }
        public IHubContext<DeviceSocketHub> DeviceHub { get; }
        public ApplicationConfig AppConfig { get; }
        public SignInManager<RemotelyUser> SignInManager { get; }

        [HttpGet("{deviceID}")]
        [Authorize]
        public async Task<IActionResult> Get(string deviceID)
        {
            return await InitiateRemoteControl(deviceID, HttpContext.User.Identity.Name);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]RemoteControlRequest rcRequest)
        {
            if (!AppConfig.AllowApiLogin)
            {
                return NotFound();
            }

            var orgId = DataService.GetUserByName(rcRequest.Email)?.OrganizationID;

            var result = await SignInManager.PasswordSignInAsync(rcRequest.Email, rcRequest.Password, false, true);
            if (result.Succeeded)
            {
                DataService.WriteEvent($"API login successful for {rcRequest.Email}.", orgId);
                return await InitiateRemoteControl(rcRequest.DeviceID, rcRequest.Email);
            }
            else if (result.IsLockedOut)
            {
                DataService.WriteEvent($"API login unsuccessful due to lockout for {rcRequest.Email}.", orgId);
                return Unauthorized("Account is locked.");
            }
            else if (result.RequiresTwoFactor)
            {
                DataService.WriteEvent($"API login unsuccessful due to 2FA for {rcRequest.Email}.", orgId);
                return Unauthorized("Account requires two-factor authentication.");
            }
            DataService.WriteEvent($"API login unsuccessful due to bad attempt for {rcRequest.Email}.", orgId);
            return BadRequest();
        }

        private async Task<IActionResult> InitiateRemoteControl(string deviceID, string userName)
        {
            var remotelyUser = DataService.GetUserByName(userName);
            var targetDevice = DeviceSocketHub.ServiceConnections.FirstOrDefault(x => 
                                    x.Value.OrganizationID == remotelyUser.OrganizationID &&
                                    x.Value.ID.ToLower() == deviceID.ToLower());

            if (targetDevice.Value != null)
            {
                if (DataService.DoesUserHaveAccessToDevice(targetDevice.Value.ID, remotelyUser))
                {
                    var currentUsers = RCDeviceSocketHub.SessionInfoList.Count(x => x.Value.OrganizationID == remotelyUser.OrganizationID);
                    if (currentUsers >= AppConfig.RemoteControlSessionLimit)
                    {
                        return BadRequest("There are already the maximum amount of active remote control sessions for your organization.");
                    }

                    var existingSessions = RCDeviceSocketHub.SessionInfoList.Where(x => x.Value.DeviceID == targetDevice.Value.ID);

                    await DeviceHub.Clients.Client(targetDevice.Key).SendAsync("RemoteControl", Request.HttpContext.Connection.Id, targetDevice.Key);

                    var stopWatch = Stopwatch.StartNew();

                    while (!RCDeviceSocketHub.SessionInfoList.Values.Any(x=>x.DeviceID == targetDevice.Value.ID && !existingSessions.Any(y=>y.Key != x.RCSocketID)) && stopWatch.Elapsed.TotalSeconds < 5)
                    {
                        await Task.Delay(10);
                    }

                    if (!RCDeviceSocketHub.SessionInfoList.Values.Any(x => x.DeviceID == targetDevice.Value.ID && !existingSessions.Any(y => y.Key != x.RCSocketID)))
                    {
                        return StatusCode(408, "The remote control process failed to start in time on the remote device.");
                    }
                    else
                    {
                        var rcSession = RCDeviceSocketHub.SessionInfoList.Values.FirstOrDefault(x=>x.DeviceID == targetDevice.Value.ID && !existingSessions.Any(y=>y.Key != x.RCSocketID));
                        return Ok($"{HttpContext.Request.Scheme}://{Request.Host}/RemoteControl?clientID={rcSession.RCSocketID}&serviceID={targetDevice.Key}&fromApi=true");
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return BadRequest("The target device couldn't be found.");
            }
        }
    }
}
