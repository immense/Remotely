using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Remotely.Server.Services;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading;
using Remotely.Server.Attributes;

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientDownloadsController : ControllerBase
    {
        public ClientDownloadsController(IWebHostEnvironment hostEnv,
            ApplicationConfig appConfig)
        {
            HostEnv = hostEnv;
            AppConfig = appConfig;
        }

        private ApplicationConfig AppConfig { get; }
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
