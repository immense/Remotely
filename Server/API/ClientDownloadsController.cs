using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Attributes;
using Remotely.Server.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientDownloadsController : ControllerBase
    {
        public ClientDownloadsController(IWebHostEnvironment hostEnv,
            IApplicationConfig appConfig)
        {
            HostEnv = hostEnv;
            AppConfig = appConfig;
        }

        private IApplicationConfig AppConfig { get; }
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
                    var scheme = AppConfig.RedirectToHttps ? "https" : Request.Scheme;
                    
                    switch (platformID)
                    {
                        case "Windows":
                            {
                                var filePath = Path.Combine(HostEnv.WebRootPath, "Downloads", $"Remotely_Installer.exe");
                                var fileName = $"Remotely_Installer-{organizationID}.exe";
                                var fileStream = System.IO.File.OpenRead(filePath);
                                return File(fileStream, "application/octet-stream", $"{fileName}");
                            }
                        case "Manjaro-x64":
                        case "Ubuntu-x64":
                        // TODO: Remove this and delete the file after a few releases.
                        case "Linux-x64":
                            {
                                var fileContents = new List<string>();
                                var fileName = $"Install-{platformID}.sh";

                                fileContents.AddRange(await System.IO.File.ReadAllLinesAsync(Path.Combine(HostEnv.WebRootPath, "Downloads", $"{fileName}")));

                                var hostIndex = fileContents.IndexOf("HostName=");
                                var orgIndex = fileContents.IndexOf("Organization=");

                                fileContents[hostIndex] = $"HostName=\"{scheme}://{Request.Host}\"";
                                fileContents[orgIndex] = $"Organization=\"{organizationID}\"";
                                var fileBytes = Encoding.UTF8.GetBytes(string.Join("\n", fileContents));
                                return File(fileBytes, "application/octet-stream", $"{fileName}");
                            }

                        default:
                            return BadRequest();
                    }
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
