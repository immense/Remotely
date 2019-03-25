using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely_Server.API
{
    [Authorize]
    [Route("api/[controller]")]
    public class RemoteControlController : Controller
    {
        [HttpPost]
        [EnableCors()]
        public void Post([FromBody]string value)
        {

        }
    }
}
