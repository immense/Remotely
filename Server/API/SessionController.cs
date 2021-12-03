using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Auth;
using Remotely.Server.Hubs;
using Remotely.Server.Models;

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ApiAuthorizationFilter))]
    public class SessionController : ControllerBase
    {

        // GET: api/<SessionController>
        [HttpGet]
        public IEnumerable<RCSessionInfo> Get()
        {
            return CasterHub.SessionInfoList.Select(a => a.Value).AsEnumerable();
        }
    }
}
