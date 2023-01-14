using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Auth;
using Remotely.Server.Services;
using System.Text;
using System.Text.Json;
using System;

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerLogsController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions() { WriteIndented = true };

        public ServerLogsController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        [HttpGet("Download")]
        public ActionResult Download()
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgId);

            var logs = _dataService.GetAllEventLogs(User.Identity?.Name, orgId);
            var fileBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(logs, _jsonOptions));
            return File(fileBytes, "application/octet-stream", "ServerLogs.json");
        }
    }
}
