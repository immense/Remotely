using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Remotely.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelayController : ControllerBase
    {
        private readonly IDataService _dataService;

        public RelayController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet("{relayCode}")]
        public async Task<string> Get(string relayCode)
        {
            var organization = await _dataService.GetOrganizationByRelayCode(relayCode);
            return organization?.ID;
        }
    }
}
