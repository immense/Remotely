using MailKit.Search;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Remotely.Server.Auth;
using Remotely.Server.Services;
using Remotely.Shared;
using Remotely.Shared.Models;
using Remotely.Shared.Services;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientDownloadsController : ControllerBase
    {
        private readonly IEmbeddedServerDataSearcher _embeddedDataSearcher;
        private readonly SemaphoreSlim _fileLock = new(1,1);
        private readonly IWebHostEnvironment _hostEnv;

        public ClientDownloadsController(
            IWebHostEnvironment hostEnv,
            IEmbeddedServerDataSearcher embeddedDataSearcher)
        {
            _hostEnv = hostEnv;
            _embeddedDataSearcher = embeddedDataSearcher;
        }

        [HttpGet("desktop/{platformID}")]
        public async Task<IActionResult> GetDesktop(string platformID)
        {
            switch (platformID)
            {
                case "WindowsDesktop-x64":
                    {
                        var filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "Win-x64", "Remotely_Desktop.exe");
                        return await GetDesktopFile(filePath);
                    }
                case "WindowsDesktop-x86":
                    {
                        var filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "Win-x86", "Remotely_Desktop.exe");
                        return await GetDesktopFile(filePath);
                    }
                case "UbuntuDesktop":
                    {
                        var filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "Linux-x64", "Remotely_Desktop");
                        return await GetDesktopFile(filePath);
                    }
                case "MacOS-x64":
                    {
                        var filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "MacOS-x64", "Remotely_Desktop");
                        return await GetDesktopFile(filePath);
                    }
                case "MacOS-arm64":
                    {
                        var filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "MacOS-arm64", "Remotely_Desktop");
                        return await GetDesktopFile(filePath);
                    }
                default:
                    return NotFound();
            }
        }


        [HttpGet("desktop/{platformId}/{organizationId}")]
        public async Task<IActionResult> GetDesktop(string platformId, string organizationId)
        {
            switch (platformId)
            {
                case "WindowsDesktop-x64":
                    {
                        var filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "Win-x64", "Remotely_Desktop.exe");
                        return await GetDesktopFile(filePath, organizationId);
                    }
                case "WindowsDesktop-x86":
                    {
                        var filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "Win-x86", "Remotely_Desktop.exe");
                        return await GetDesktopFile(filePath, organizationId);
                    }
                case "UbuntuDesktop":
                    {
                        var filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "Linux-x64", "Remotely_Desktop");
                        return await GetDesktopFile(filePath, organizationId);
                    }
                case "MacOS-x64":
                    {
                        var filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "MacOS-x64", "Remotely_Desktop");
                        return await GetDesktopFile(filePath);
                    }
                case "MacOS-arm64":
                    {
                        var filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "MacOS-arm64", "Remotely_Desktop");
                        return await GetDesktopFile(filePath);
                    }
                default:
                    return NotFound();
            }
        }

        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        [HttpGet("{platformID}")]
        public async Task<IActionResult> GetInstaller(string platformID)
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);
            return await GetInstallFile(orgID, platformID);
        }

        [HttpGet("{organizationID}/{platformID}")]
        public async Task<IActionResult> GetInstaller(string organizationID, string platformID)
        {
            return await GetInstallFile(organizationID, platformID);
        }

        private async Task<IActionResult> GetBashInstaller(string fileName, string organizationId)
        {
            var fileContents = new List<string>();
            fileContents.AddRange(await System.IO.File.ReadAllLinesAsync(Path.Combine(_hostEnv.WebRootPath, "Content", fileName)));

            var hostIndex = fileContents.IndexOf("HostName=");
            var orgIndex = fileContents.IndexOf("Organization=");

            fileContents[hostIndex] = $"HostName=\"{Request.Scheme}://{Request.Host}\"";
            fileContents[orgIndex] = $"Organization=\"{organizationId}\"";
            var fileBytes = Encoding.UTF8.GetBytes(string.Join("\n", fileContents));
            return File(fileBytes, "application/octet-stream", fileName);
        }

        private async Task<IActionResult> GetDesktopFile(string filePath, string organizationId = null)
        {
            var serverUrl = $"{Request.Scheme}://{Request.Host}";
            var embeddedData = new EmbeddedServerData(new Uri(serverUrl), organizationId);
            var result = await _embeddedDataSearcher.GetRewrittenStream(filePath, embeddedData);

            if (!result.IsSuccess)
            {
                throw result.Exception;
            }

            return File(result.Value, "application/octet-stream", Path.GetFileName(filePath));
        }

        private async Task<IActionResult> GetInstallFile(string organizationId, string platformID)
        {
            try
            {
                if (await _fileLock.WaitAsync(TimeSpan.FromSeconds(15)))
                {
                    switch (platformID)
                    {
                        case "WindowsInstaller":
                            {
                                var filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "Remotely_Installer.exe");
                                var serverUrl = $"{Request.Scheme}://{Request.Host}";
                                var embeddedData = new EmbeddedServerData(new Uri(serverUrl), organizationId);
                                var result = await _embeddedDataSearcher.GetRewrittenStream(filePath, embeddedData);

                                if (!result.IsSuccess)
                                {
                                    throw result.Exception;
                                }

                                return File(result.Value, "application/octet-stream", "Remotely_Installer.exe");
                            }
                        case "ManjaroInstaller-x64":
                            {
                                var fileName = "Install-Manjaro-x64.sh";

                                return await GetBashInstaller(fileName, organizationId);
                            }
                        case "UbuntuInstaller-x64":
                            {
                                var fileName = "Install-Ubuntu-x64.sh";

                                return await GetBashInstaller(fileName, organizationId);
                            }
                        case "MacOSInstaller-x64":
                            {
                                var fileName = "Install-MacOS-x64.sh";

                                return await GetBashInstaller(fileName, organizationId);
                            }
                        case "MacOSInstaller-arm64":
                            {
                                var fileName = "Install-MacOS-arm64.sh";

                                return await GetBashInstaller(fileName, organizationId);
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
                if (_fileLock.CurrentCount == 0)
                {
                    _fileLock.Release();
                }
            }
        }
    }
}
