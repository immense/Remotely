using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Auth;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely.Server.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {

        public DevicesController(IDataService dataService)
        {
            DataService = dataService;
        }
        private IDataService DataService { get; set; }


        [HttpGet]
        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        public IEnumerable<Device> Get()
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            if (User.Identity.IsAuthenticated)
            {
                return DataService.GetDevicesForUser(User.Identity.Name);
            }

            return DataService.GetAllDevices(orgID);
        }

        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        [HttpGet("{id}")]
        public Device Get(string id)
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            var device = DataService.GetDevice(orgID, id);

            if (User.Identity.IsAuthenticated &&
                !DataService.DoesUserHaveAccessToDevice(id, DataService.GetUserByNameWithOrg(User.Identity.Name)))
            {
                return null;
            }
            return device;
        }

        [HttpPut]
        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        public async Task<IActionResult> Update(
            [FromBody] DeviceSetupOptions deviceOptions,
            [FromHeader] string organizationId)
        {
            if (deviceOptions == null ||
                string.IsNullOrWhiteSpace(organizationId))
            {
                return BadRequest("DeviceOptions and OrganizationId are required.");
            }

            if (User.Identity.IsAuthenticated &&
                !DataService.DoesUserHaveAccessToDevice(deviceOptions.DeviceID, DataService.GetUserByNameWithOrg(User.Identity.Name)))
            {
                return Unauthorized();
            }

            var device = await DataService.UpdateDevice(deviceOptions, organizationId);
            if (device == null)
            {
                return BadRequest();
            }
            return Created(Request.GetDisplayUrl(), device);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeviceSetupOptions deviceOptions)
        {
            var device = await DataService.CreateDevice(deviceOptions);
            if (device is null)
            {
                return BadRequest("Device already exists.  Use Put with authorization to update the device.");
            }
            return Created(Request.GetDisplayUrl(), device);
        }
    }
}
