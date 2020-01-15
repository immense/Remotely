using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Services;
using Remotely.Shared.Models;

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServerLogsController : ControllerBase
    {

        public ServerLogsController(DataService dataService)
        {
            DataService = dataService;
        }
        public DataService DataService { get; set; }

        // GET: api/ServerLogs
        [HttpGet("Download")]
        public ActionResult Download()
        {
            var logs = DataService.GetAllEventLogs(HttpContext.User.Identity.Name);
            var fileBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(logs));
            return File(fileBytes, "application/octet-stream", "ServerLogs.json");
        }
    }
}
