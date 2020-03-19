using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Auth;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ApiAuthorizationFilter))]
    public class AlertsController : ControllerBase
    {
        [HttpPost("Create")]
        public async Task<IActionResult> Create(AlertOptions alertOptions)
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            if (alertOptions.ShouldAlert)
            {
                var alert = new Alert()
                {
                    CreatedOn = DateTimeOffset.Now,
                    DeviceID = alertOptions.AlertDeviceID,
                    Message = alertOptions.AlertMessage
                };
                // TODO: Add alert.
            }

            // TODO: Email.

            // TODO: API request.

            return Ok();
        }
    }
}
