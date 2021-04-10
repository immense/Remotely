using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Remotely.Server.Attributes;
using Remotely.Server.Hubs;
using Remotely.Server.Models;
using Remotely.Server.Services;
using Remotely.Shared.Utilities;
using Remotely.Shared.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Remotely.Server.Auth;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemoteControlController : ControllerBase
    {
        public RemoteControlController(IDataService dataService,
            IHubContext<AgentHub> agentHub,
            IApplicationConfig appConfig,
            SignInManager<RemotelyUser> signInManager)
        {
            DataService = dataService;
            AgentHubContext = agentHub;
            AppConfig = appConfig;
            SignInManager = signInManager;
        }

        public IDataService DataService { get; }
        public IHubContext<AgentHub> AgentHubContext { get; }
        public IApplicationConfig AppConfig { get; }
        public SignInManager<RemotelyUser> SignInManager { get; }

        [HttpGet("{deviceID}")]
        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        public async Task<IActionResult> Get(string deviceID)
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);
            return await InitiateRemoteControl(deviceID, orgID);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RemoteControlRequest rcRequest)
        {
            if (!AppConfig.AllowApiLogin)
            {
                return NotFound();
            }

            var orgId = DataService.GetUserByNameWithOrg(rcRequest.Email)?.OrganizationID;

            var result = await SignInManager.PasswordSignInAsync(rcRequest.Email, rcRequest.Password, false, true);
            if (result.Succeeded &&
                DataService.DoesUserHaveAccessToDevice(rcRequest.DeviceID, DataService.GetUserByNameWithOrg(rcRequest.Email)))
            {
                DataService.WriteEvent($"API login successful for {rcRequest.Email}.", orgId);
                return await InitiateRemoteControl(rcRequest.DeviceID, orgId);
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

        private async Task<IActionResult> InitiateRemoteControl(string deviceID, string orgID)
        {
            var targetDevice = AgentHub.ServiceConnections.FirstOrDefault(x =>
                                    x.Value.OrganizationID == orgID &&
                                    x.Value.ID.ToLower() == deviceID.ToLower());

            if (targetDevice.Value != null)
            {
                if (User.Identity.IsAuthenticated &&
                   !DataService.DoesUserHaveAccessToDevice(targetDevice.Value.ID, DataService.GetUserByNameWithOrg(User.Identity.Name)))
                {
                    return Unauthorized();
                }


                var currentUsers = CasterHub.SessionInfoList.Count(x => x.Value.OrganizationID == orgID);
                if (currentUsers >= AppConfig.RemoteControlSessionLimit)
                {
                    return BadRequest("There are already the maximum amount of active remote control sessions for your organization.");
                }

                var existingSessions = CasterHub.SessionInfoList
                    .Where(x => x.Value.DeviceID == targetDevice.Value.ID)
                    .Select(x => x.Key)
                    .ToList();

                await AgentHubContext.Clients.Client(targetDevice.Key).SendAsync("RemoteControl", Request.HttpContext.Connection.Id, targetDevice.Key);

                bool remoteControlStarted()
                {
                    return !CasterHub.SessionInfoList.Values
                        .Where(x => x.DeviceID == targetDevice.Value.ID)
                        .All(x => existingSessions.Contains(x.CasterSocketID));
                };

                if (!await TaskHelper.DelayUntilAsync(remoteControlStarted, TimeSpan.FromSeconds(30)))
                {
                    return StatusCode(408, "The remote control process failed to start in time on the remote device.");
                }
                else
                {
                    var rcSession = CasterHub.SessionInfoList.Values.LastOrDefault(x => x.DeviceID == targetDevice.Value.ID && !existingSessions.Contains(x.CasterSocketID));
                    var otp = RemoteControlFilterAttribute.GetOtp(targetDevice.Value.ID);
                    return Ok($"{HttpContext.Request.Scheme}://{Request.Host}/RemoteControl?casterID={rcSession.CasterSocketID}&serviceID={targetDevice.Key}&fromApi=true&otp={Uri.EscapeDataString(otp)}");
                }
            }
            else
            {
                return BadRequest("The target device couldn't be found.");
            }
        }
    }
}
