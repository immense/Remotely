using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Auth;
using Remotely.Server.Extensions;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely.Server.API;

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
        if (!Request.Headers.TryGetOrganizationId(out var orgId))
        {
            return Array.Empty<Device>();
        }

        if (User.Identity?.IsAuthenticated == true &&
            !string.IsNullOrWhiteSpace(User.Identity.Name))
        {
            return DataService.GetDevicesForUser(User.Identity.Name);
        }

        // Authorized with API key.  Return all.
        return DataService.GetAllDevices(orgId);
    }

    [ServiceFilter(typeof(ApiAuthorizationFilter))]
    [HttpGet("{id}")]
    public ActionResult<Device> Get(string id)
    {
        if (!Request.Headers.TryGetOrganizationId(out var orgId))
        {
            return Unauthorized();
        }

        var device = DataService.GetDevice(orgId, id);

        if (User.Identity?.IsAuthenticated == true &&
            !string.IsNullOrWhiteSpace(User.Identity.Name) &&
            !DataService.DoesUserHaveAccessToDevice(id, DataService.GetUserByNameWithOrg(User.Identity.Name)))
        {
            return Unauthorized();
        }
        return device;
    }

    [HttpPut]
    [ServiceFilter(typeof(ApiAuthorizationFilter))]
    public async Task<IActionResult> Update(
        [FromBody] DeviceSetupOptions deviceOptions,
        [FromHeader] string organizationId)
    {
        if (string.IsNullOrWhiteSpace(deviceOptions?.DeviceID) ||
            string.IsNullOrWhiteSpace(organizationId))
        {
            return BadRequest("DeviceOptions and OrganizationId are required.");
        }

        if (string.IsNullOrWhiteSpace(User.Identity?.Name))
        {
            return Unauthorized();
        }

        var user = DataService.GetUserByNameWithOrg(User.Identity.Name);
        if (user is null)
        {
            return Unauthorized();
        }

        if (User.Identity?.IsAuthenticated == true &&
            !DataService.DoesUserHaveAccessToDevice(deviceOptions.DeviceID, user))
        {
            return Unauthorized();
        }

        var device = await DataService.UpdateDevice(deviceOptions, organizationId);
        if (device is null)
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
