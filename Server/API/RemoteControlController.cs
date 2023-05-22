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
using Immense.RemoteControl.Shared.Helpers;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemoteControlController : ControllerBase
    {
        private readonly IHubContext<AgentHub> _serviceHub;
        private readonly IDesktopHubSessionCache _desktopSessionCache;
        private readonly IServiceHubSessionCache _serviceSessionCache;
        private readonly IApplicationConfig _appConfig;
        private readonly IOtpProvider _otpProvider;
        private readonly IHubEventHandler _hubEvents;
        private readonly IDataService _dataService;
        private readonly SignInManager<RemotelyUser> _signInManager;
        private readonly ILogger<RemoteControlController> _logger;

        public RemoteControlController(
            SignInManager<RemotelyUser> signInManager,
            IDataService dataService,
            IDesktopHubSessionCache desktopSessionCache,
            IHubContext<AgentHub> serviceHub,
            IServiceHubSessionCache serviceSessionCache,
            IOtpProvider otpProvider,
            IHubEventHandler hubEvents,
            IApplicationConfig appConfig,
            ILogger<RemoteControlController> logger)
        {
            _dataService = dataService;
            _serviceHub = serviceHub;
            _desktopSessionCache = desktopSessionCache;
            _serviceSessionCache = serviceSessionCache;
            _appConfig = appConfig;
            _otpProvider = otpProvider;
            _hubEvents = hubEvents;
            _signInManager = signInManager;
            _logger = logger;
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
                _logger.LogInformation("API login successful for {rcRequestEmail}.", rcRequest.Email);
                return await InitiateRemoteControl(rcRequest.DeviceID, orgId);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogInformation("API login successful for {rcRequestEmail}.", rcRequest.Email);
                return Unauthorized("Account is locked.");
            }
            else if (result.RequiresTwoFactor)
            {
                _logger.LogInformation("API login successful for {rcRequestEmail}.", rcRequest.Email);
                return Unauthorized("Account requires two-factor authentication.");
            }
            _logger.LogInformation("API login unsuccessful due to bad attempt for {rcRequestEmail}.", rcRequest.Email);
            return BadRequest();
        }

        private async Task<IActionResult> InitiateRemoteControl(string deviceID, string orgID)
        {
            if (!_serviceSessionCache.TryGetByDeviceId(deviceID, out var targetDevice) ||
                !_serviceSessionCache.TryGetConnectionId(deviceID, out var serviceConnectionId))
            {
                return NotFound("The target device couldn't be found.");
            }

            if (targetDevice.OrganizationID != orgID)
            {
                return Unauthorized();
            }

            if (User.Identity.IsAuthenticated &&
               !_dataService.DoesUserHaveAccessToDevice(targetDevice.ID, _dataService.GetUserByNameWithOrg(User.Identity.Name)))
            {
                return Unauthorized();
            }


            var sessionCount = _desktopSessionCache.Sessions
                   .OfType<RemoteControlSessionEx>()
                   .Count(x => x.OrganizationId == orgID);

            if (sessionCount > _appConfig.RemoteControlSessionLimit)
            {
                return BadRequest("There are already the maximum amount of active remote control sessions for your organization.");
            }

            var sessionId = Guid.NewGuid();
            var accessKey = RandomGenerator.GenerateAccessKey();

            var session = new RemoteControlSessionEx()
            {
                UnattendedSessionId = sessionId,
                UserConnectionId = HttpContext.Connection.Id,
                AgentConnectionId = serviceConnectionId,
                DeviceId = deviceID,
                OrganizationId = orgID
            };

            _desktopSessionCache.AddOrUpdate($"{sessionId}", session, (k, v) =>
            {
                if (v is RemoteControlSessionEx ex)
                {
                    ex.AgentConnectionId = HttpContext.Connection.Id;
                    return ex;
                }
                v.Dispose();
                return session;
            });

            var orgName = _dataService.GetOrganizationNameById(orgID);

            await _serviceHub.Clients.Client(serviceConnectionId).SendAsync("RemoteControl", 
                sessionId,
                accessKey,
                HttpContext.Connection.Id,
                string.Empty,
                orgName,
                orgID);

            var waitResult = await session.WaitForSessionReady(TimeSpan.FromSeconds(30));
            if (!waitResult)
            {
                return StatusCode(408, "The remote control process failed to start in time on the remote device.");
            }
           
            var otp = _otpProvider.GetOtp(targetDevice.ID);

            return Ok($"{HttpContext.Request.Scheme}://{Request.Host}/RemoteControl/Viewer?mode=Unattended&sessionId={sessionId}&accessKey={accessKey}&otp={otp}");
        }
    }
}
