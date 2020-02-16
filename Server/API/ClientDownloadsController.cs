using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using Remotely.Server.Services;
using Remotely.Server.Auth;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientDownloadsController : ControllerBase
    {
        public ClientDownloadsController(IWebHostEnvironment hostEnv, DataService dataService, ApplicationConfig appConfig)
        {
            HostEnv = hostEnv;
            DataService = dataService;
            AppConfig = appConfig;
        }

        private ApplicationConfig AppConfig { get; }
        private DataService DataService { get; set; }
        private IWebHostEnvironment HostEnv { get; set; }

        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        [HttpGet("{platformID}")]
        public async Task<ActionResult> Get(string platformID)
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);
            return await GetInstallFile(orgID, platformID);
        }

        private async Task<ActionResult> GetInstallFile(string organizationID, string platformID)
        {
            var scheme = AppConfig.RedirectToHttps ? "https" : Request.Scheme;
            var fileContents = new List<string>();
            string fileName;
            byte[] fileBytes;
            switch (platformID)
            {
                case "Win10":
                    {
                        fileName = $"Install-{platformID}.ps1";

                        fileContents.AddRange(await System.IO.File.ReadAllLinesAsync(Path.Combine(HostEnv.WebRootPath, "Downloads", $"{fileName}")));

                        var hostIndex = fileContents.IndexOf("[string]$HostName = $null");
                        var orgIndex = fileContents.IndexOf("[string]$Organization = $null");

                        fileContents[hostIndex] = $"[string]$HostName = \"{scheme}://{Request.Host}\"";
                        fileContents[orgIndex] = $"[string]$Organization = \"{organizationID}\"";
                        fileBytes = System.Text.Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, fileContents));
                        break;
                    }

                case "Linux-x64":
                    {
                        fileName = "Install-Linux-x64.sh";

                        fileContents.AddRange(await System.IO.File.ReadAllLinesAsync(Path.Combine(HostEnv.WebRootPath, "Downloads", $"{fileName}")));

                        var hostIndex = fileContents.IndexOf("HostName=");
                        var orgIndex = fileContents.IndexOf("Organization=");

                        fileContents[hostIndex] = $"HostName=\"{scheme}://{Request.Host}\"";
                        fileContents[orgIndex] = $"Organization=\"{organizationID}\"";
                        fileBytes = System.Text.Encoding.UTF8.GetBytes(string.Join("\n", fileContents));
                        break;
                    }

                default:
                    return BadRequest();
            }

            return File(fileBytes, "application/octet-stream", $"{fileName}");
        }
    }
}
