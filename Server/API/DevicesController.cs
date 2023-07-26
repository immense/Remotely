using Immense.RemoteControl.Shared.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Remotely.Server.Auth;
using Remotely.Server.Extensions;
using Remotely.Server.Services;
using Remotely.Shared.Entities;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Remotely.Server.API;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDataService _dataService;
    private readonly ILogger<DevicesController> _logger;

    public DevicesController(
        IDataService dataService,
        ILogger<DevicesController> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }


    [HttpGet]
    [ServiceFilter(typeof(ApiAuthorizationFilter))]
    public IEnumerable<Device> Get()
    {
        if (!Request.Headers.TryGetOrganizationId(out var orgId))
        {
            return Array.Empty<Device>();
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            return _dataService.GetDevicesForUser($"{User.Identity.Name}");
        }

        // Authorized with API key.  Return all.
        return _dataService.GetAllDevices(orgId);
    }

    [ServiceFilter(typeof(ApiAuthorizationFilter))]
    [HttpGet("{id}")]
    public async Task<ActionResult<Device>> Get(string id)
    {
        if (!Request.Headers.TryGetOrganizationId(out var orgId))
        {
            return Unauthorized();
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            var userResult = await _dataService.GetUserByName($"{User.Identity.Name}");
            _logger.LogResult(userResult);

            if (!userResult.IsSuccess)
            {
                return Unauthorized();
            }

            if (!_dataService.DoesUserHaveAccessToDevice(id, userResult.Value))
            {
                return Unauthorized();
            }
        }

        var deviceResult = await _dataService.GetDevice(orgId, id);
        _logger.LogResult(deviceResult);

        if (!deviceResult.IsSuccess)
        {
            return NotFound();
        }

        return deviceResult.Value;
    }

    [HttpPut]
    [ServiceFilter(typeof(ApiAuthorizationFilter))]
    public async Task<IActionResult> Update([FromBody] DeviceSetupOptions deviceOptions)
    {
        if (!Request.Headers.TryGetOrganizationId(out var orgId))
        {
            return Unauthorized();
        }
        
        if (string.IsNullOrWhiteSpace(deviceOptions?.DeviceID))
        {
            return BadRequest("DeviceId is required.");
        }


        if (User.Identity?.IsAuthenticated == true)
        {
            var userResult = await _dataService.GetUserByName($"{User.Identity.Name}");
            _logger.LogResult(userResult);

            if (!userResult.IsSuccess)
            {
                return Unauthorized();
            }

            if (!_dataService.DoesUserHaveAccessToDevice(deviceOptions.DeviceID, userResult.Value))
            {
                return Unauthorized();
            }

        }

        var deviceResult = await _dataService.UpdateDevice(deviceOptions, orgId);
        _logger.LogResult(deviceResult);

        if (!deviceResult.IsSuccess)
        {
            return BadRequest();
        }
        return Created(Request.GetDisplayUrl(), deviceResult.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DeviceSetupOptions deviceOptions)
    {
        var result = await _dataService.CreateDevice(deviceOptions);
        _logger.LogResult(result);

        if (!result.IsSuccess)
        {
            return BadRequest("Device already exists.  Use Put with authorization to update the device.");
        }
        return Created(Request.GetDisplayUrl(), result.Value);
    }
}
