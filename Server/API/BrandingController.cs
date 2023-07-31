using Immense.RemoteControl.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Remotely.Server.Services;
using Remotely.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.API;

[Route("api/[controller]")]
[ApiController]
public class BrandingController : ControllerBase
{
    private readonly IDataService _dataService;
    private readonly ILogger<BrandingController> _logger;

    public BrandingController(
        IDataService dataService,
        ILogger<BrandingController> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }


    [HttpGet("{organizationId}")]
    public async Task<ActionResult<BrandingInfo>> Get(string organizationId)
    {
        var result = await _dataService.GetBrandingInfo(organizationId);
        _logger.LogResult(result);
        if (!result.IsSuccess)
        {
            return NotFound();
        }
        return result.Value;
    }

    [HttpGet]
    public async Task<BrandingInfo> GetDefault()
    {
        var orgResult = await _dataService.GetDefaultOrganization();
        _logger.LogResult(orgResult);

        if (!orgResult.IsSuccess)
        {
            return new();
        }

        var brandingResult = await _dataService.GetBrandingInfo(orgResult.Value.ID);
        _logger.LogResult(brandingResult);

        if (!orgResult.IsSuccess || 
            brandingResult.Value is null)
        {
            return new();
        }

        return brandingResult.Value;
    }
}
