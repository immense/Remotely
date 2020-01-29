using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentUpdateController : ControllerBase
    {
        public AgentUpdateController(IWebHostEnvironment hostingEnv, DataService dataService)
        {
            this.HostingEnv = hostingEnv;
            DataService = dataService;
        }

        private IWebHostEnvironment HostingEnv { get; }
        private DataService DataService { get; }

        // GET: api/<controller>
        [HttpGet("[action]")]
        public string CurrentVersion()
        {
            var filePath = Path.Combine(HostingEnv.ContentRootPath, "CurrentVersion.txt");
            if (!System.IO.File.Exists(filePath))
            {
                return "0.0.0.0";
            }
            return System.IO.File.ReadAllText(filePath).Trim();
        }

        [HttpGet("[action]")]
        public int UpdateWindow()
        {
            return DataService.GetDeviceCount() * 10;
        }
    }
}
