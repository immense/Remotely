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
using Remotely.Shared.Models;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Threading;
using Remotely.Shared.Services;

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientDownloadsController : ControllerBase
    {
        public ClientDownloadsController(IWebHostEnvironment hostEnv,
            IConfiguration configuration,
            ApplicationConfig appConfig,
            DataService dataService)
        {
            HostEnv = hostEnv;
            AppConfig = appConfig;
            DataService = dataService;
            Configuration = configuration;
        }

        private ApplicationConfig AppConfig { get; }
        private IConfiguration Configuration { get; }
        private DataService DataService { get; }
        private SemaphoreSlim FileLock { get; } = new SemaphoreSlim(1);
        private IWebHostEnvironment HostEnv { get; set; }

        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        [HttpGet("{platformID}")]
        public async Task<ActionResult> Get(string platformID)
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);
            return await GetInstallFile(orgID, platformID);
        }

        [HttpGet("{organizationID}/{platformID}")]
        public async Task<ActionResult> Get(string organizationID, string platformID)
        {
            return await GetInstallFile(organizationID, platformID);
        }

        private async Task<ActionResult> GetInstallFile(string organizationID, string platformID)
        {
            try
            {
                if (await FileLock.WaitAsync(TimeSpan.FromSeconds(15)))
                {
                    var organizationName = DataService.GetOrganizationNameById(organizationID);
                    var scheme = AppConfig.RedirectToHttps ? "https" : Request.Scheme;
                    var fileContents = new List<string>();
                    string fileName;
                    byte[] fileBytes;
                    switch (platformID)
                    {
                        case "Windows":
                            {
                                var filePath = Path.Combine(HostEnv.WebRootPath, "Downloads", $"Remotely_Installer.exe");
                                fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                                fileName = $"Remotely_Installer-{organizationID}.exe";
                                break;
                            }
                        // TODO: Remove after a few versions.
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
                                fileBytes = Encoding.UTF8.GetBytes(string.Join("\n", fileContents));
                                break;
                            }

                        default:
                            return BadRequest();
                    }

                    return File(fileBytes, "application/octet-stream", $"{fileName}");
                }
                else
                {
                    return StatusCode(StatusCodes.Status408RequestTimeout);
                }
            }
            finally
            {
                if (FileLock.CurrentCount == 0)
                {
                    FileLock.Release();
                }
            }
          
        }
    }
}
