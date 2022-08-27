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
using Immense.RemoteControl.Server.Services;
using Remotely.Server.Services.RcImplementations;
using Immense.RemoteControl.Server.Abstractions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemoteControlController : ControllerBase
    {
        private readonly IHubContext<ServiceHub> _serviceHub;
        private readonly IDesktopHubSessionCache _desktopSessionCache;
        private readonly IServiceHubSessionCache _serviceSessionCache;
        private readonly IApplicationConfig _appConfig;
        private readonly IOtpProvider _otpProvider;
        private readonly IDataService _dataService;
        private readonly SignInManager<RemotelyUser> _signInManager;

        public RemoteControlController(
            SignInManager<RemotelyUser> signInManager,
            IDataService dataService,
            IDesktopHubSessionCache desktopSessionCache,
            IHubContext<ServiceHub> serviceHub,
            IServiceHubSessionCache serviceSessionCache,
            IOtpProvider otpProvider,
            IApplicationConfig appConfig)
        {
            _dataService = dataService;
            _serviceHub = serviceHub;
            _desktopSessionCache = desktopSessionCache;
            _serviceSessionCache = serviceSessionCache;
            _appConfig = appConfig;
            _otpProvider = otpProvider;
            _signInManager = signInManager;
        }

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
            if (!_appConfig.AllowApiLogin)
            {
                return NotFound();
            }

            var orgId = _dataService.GetUserByNameWithOrg(rcRequest.Email)?.OrganizationID;

            var result = await _signInManager.PasswordSignInAsync(rcRequest.Email, rcRequest.Password, false, true);
            if (result.Succeeded &&
                _dataService.DoesUserHaveAccessToDevice(rcRequest.DeviceID, _dataService.GetUserByNameWithOrg(rcRequest.Email)))
            {
                _dataService.WriteEvent($"API login successful for {rcRequest.Email}.", orgId);
                return await InitiateRemoteControl(rcRequest.DeviceID, orgId);
            }
            else if (result.IsLockedOut)
            {
                _dataService.WriteEvent($"API login unsuccessful due to lockout for {rcRequest.Email}.", orgId);
                return Unauthorized("Account is locked.");
            }
            else if (result.RequiresTwoFactor)
            {
                _dataService.WriteEvent($"API login unsuccessful due to 2FA for {rcRequest.Email}.", orgId);
                return Unauthorized("Account requires two-factor authentication.");
            }
            _dataService.WriteEvent($"API login unsuccessful due to bad attempt for {rcRequest.Email}.", orgId);
            return BadRequest();
        }

        private async Task<IActionResult> InitiateRemoteControl(string deviceID, string orgID)
        {
            var targetDevice = _serviceSessionCache.Sessions.FirstOrDefault(x =>
                                    x.Value.ToLower() == deviceID.ToLower() &&
                                    _dataService.GetDevice(x.Value)?.OrganizationID == orgID);

            if (targetDevice.Value != null)
            {
                if (User.Identity.IsAuthenticated &&
                   !_dataService.DoesUserHaveAccessToDevice(targetDevice.Value, _dataService.GetUserByNameWithOrg(User.Identity.Name)))
                {
                    return Unauthorized();
                }


                var currentUsers = _desktopSessionCache.Sessions.Count(x => x.Value.OrganizationID == orgID);
                if (currentUsers >= _appConfig.RemoteControlSessionLimit)
                {
                    return BadRequest("There are already the maximum amount of active remote control sessions for your organization.");
                }

                var existingSessions = _desktopSessionCache.Sessions
                    .Where(x => x.Value.DeviceID == targetDevice.Value)
                    .Select(x => x.Key)
                    .ToList();

                await _serviceHub.Clients.Client(targetDevice.Key).SendAsync("RemoteControl", Request.HttpContext.Connection.Id, targetDevice.Key);

                bool remoteControlStarted()
                {
                    return !_desktopSessionCache.Sessions.Values
                        .Where(x => x.DeviceID == targetDevice.Value)
                        .All(x => existingSessions.Contains(x.CasterConnectionId));
                };

                if (!await TaskHelper.DelayUntilAsync(remoteControlStarted, TimeSpan.FromSeconds(30)))
                {
                    return StatusCode(408, "The remote control process failed to start in time on the remote device.");
                }
                else
                {
                    var rcSession = _desktopSessionCache.Sessions.Values.LastOrDefault(x =>
                        x.DeviceID == targetDevice.Value && 
                        !existingSessions.Contains(x.CasterConnectionId));

                    var otp = _otpProvider.GetOtp(targetDevice.Value);
                    return Ok($"{HttpContext.Request.Scheme}://{Request.Host}/RemoteControl?casterID={rcSession.CasterConnectionId}&serviceID={targetDevice.Key}&fromApi=true&otp={Uri.EscapeDataString(otp)}");
                }
            }
            else
            {
                return BadRequest("The target device couldn't be found.");
            }
        }
    }
}
