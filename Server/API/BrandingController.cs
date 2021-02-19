using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandingController : ControllerBase
    {
        private readonly IDataService _dataService;

        public BrandingController(IDataService dataService)
        {
            _dataService = dataService;
        }


        [HttpGet("{organizationId}")]
        public async Task<BrandingInfo> Get(string organizationId)
        {
            return await _dataService.GetBrandingInfo(organizationId);
        }

        [HttpGet]
        public async Task<BrandingInfo> GetDefault()
        {
            var defaultOrg = await _dataService.GetDefaultOrganization();
            if (defaultOrg is null)
            {
                return new BrandingInfo();
            }
            return await _dataService.GetBrandingInfo(defaultOrg.ID);
        }
    }
}
