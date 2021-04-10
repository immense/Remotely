using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Auth;
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
        private readonly IApplicationConfig _appConfig;
        private readonly IDataService _dataService;
        private readonly SemaphoreSlim _fileLock = new(1,1);
        private readonly IWebHostEnvironment _hostEnv;
        public ClientDownloadsController(
            IWebHostEnvironment hostEnv,
            IDataService dataService,
            IApplicationConfig appConfig)
        {
            _hostEnv = hostEnv;
            _appConfig = appConfig;
            _dataService = dataService;
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
                        var filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "Remotely_Desktop");
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
                        var filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "Remotely_Desktop");
                        return await GetDesktopFile(filePath, organizationId);
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
            var scheme = _appConfig.RedirectToHttps ? "https" : Request.Scheme;

            var fileContents = new List<string>();
            fileContents.AddRange(await System.IO.File.ReadAllLinesAsync(Path.Combine(_hostEnv.WebRootPath, "Content", fileName)));

            var hostIndex = fileContents.IndexOf("HostName=");
            var orgIndex = fileContents.IndexOf("Organization=");

            fileContents[hostIndex] = $"HostName=\"{scheme}://{Request.Host}\"";
            fileContents[orgIndex] = $"Organization=\"{organizationId}\"";
            var fileBytes = Encoding.UTF8.GetBytes(string.Join("\n", fileContents));
            return File(fileBytes, "application/octet-stream", fileName);
        }

        private async Task<IActionResult> GetDesktopFile(string filePath, string organizationId = null)
        {
            string relayCode;

            if (!string.IsNullOrWhiteSpace(organizationId))
            {
                var currentOrg = _dataService.GetOrganizationById(organizationId);
                relayCode = currentOrg.RelayCode;
            }
            else
            {
                relayCode = await _dataService.GetDefaultRelayCode();
            }

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

            if (!string.IsNullOrWhiteSpace(relayCode))
            {
                var downloadFileName = fileNameWithoutExtension + $"-[{relayCode}]" + Path.GetExtension(filePath);
                return File(fs, "application/octet-stream", downloadFileName);

            }
            else
            {
                return File(fs, "application/octet-stream", Path.GetFileName(filePath));
            }
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
                                var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                                var organization = _dataService.GetOrganizationById(organizationId);
                                var relayCode = organization.RelayCode;
                                return File(fs, "application/octet-stream", $"Remotely_Install-[{relayCode}].exe");
                            }
                        // TODO: Remove after a few releases.
                        case "Manjaro-x64":
                        case "ManjaroInstaller-x64":
                            {
                                var fileName = "Install-Manjaro-x64.sh";

                                return await GetBashInstaller(fileName, organizationId);
                            }
                        // TODO: Remove after a few releases.
                        case "Ubuntu-x64":
                        case "UbuntuInstaller-x64":
                            {
                                var fileName = "Install-Ubuntu-x64.sh";

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
