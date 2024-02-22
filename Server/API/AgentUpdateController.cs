using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Remotely.Server.Hubs;
using Remotely.Server.RateLimiting;
using Remotely.Server.Services;
using Remotely.Shared.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Remotely.Server.API;

[Route("api/[controller]")]
[ApiController]
public class AgentUpdateController : ControllerBase
{
    private readonly IHubContext<AgentHub, IAgentHubClient> _agentHubContext;
    private readonly ILogger<AgentUpdateController> _logger;
    private readonly IDataService _dataService;
    private readonly IWebHostEnvironment _hostEnv;
    private readonly IAgentHubSessionCache _serviceSessionCache;

    public AgentUpdateController(IWebHostEnvironment hostingEnv,
        IDataService dataService,
        IAgentHubSessionCache serviceSessionCache,
        IHubContext<AgentHub, IAgentHubClient> agentHubContext,
        ILogger<AgentUpdateController> logger)
    {
        _hostEnv = hostingEnv;
        _dataService = dataService;
        _serviceSessionCache = serviceSessionCache;
        _agentHubContext = agentHubContext;
        _logger = logger;
    }

    [HttpGet("[action]/{platform}")]
    [EnableRateLimiting(PolicyNames.AgentUpdateDownloads)]
    public async Task<ActionResult> DownloadPackage(string platform)
    {
        try
        {
            var remoteIp = $"{Request?.HttpContext?.Connection?.RemoteIpAddress}";

            if (await CheckForDeviceBan(remoteIp))
            {
                return BadRequest();
            }

            string filePath;

            switch (platform.ToLower())
            {
                case "win-x64":
                    filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "Remotely-Win-x64.zip");
                    break;
                case "win-x86":
                    filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "Remotely-Win-x86.zip");
                    break;
                case "linux":
                    filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "Remotely-Linux.zip");
                    break;
                case "macos-x64":
                    filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "Remotely-MacOS-x64.zip");
                    break;
                default:
                    _logger.LogWarning(
                        "Unknown platform requested in {className}. " +
                        "Platform: {platform}. " +
                        "IP: {remoteIp}.",
                        nameof(AgentUpdateController),
                        platform,
                        remoteIp);
                    return BadRequest();
            }

            var fileStream = System.IO.File.OpenRead(filePath);

            return File(fileStream, "application/octet-stream", "RemotelyUpdate.zip");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while downloading package.");
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }


    [HttpGet("[action]/{platform}/{downloadId}")]
    [EnableRateLimiting(PolicyNames.AgentUpdateDownloads)]
    [Obsolete("This method is only for backwards compatibility.  Remove after a few releases.")]
    public async Task<ActionResult> DownloadPackage(string platform, string downloadId)
    {
        return await DownloadPackage(platform);
    }

    private async Task<bool> CheckForDeviceBan(string deviceIp)
    {
        if (string.IsNullOrWhiteSpace(deviceIp))
        {
            return false;
        }

        var settings = await _dataService.GetSettings();
        if (settings.BannedDevices.Contains(deviceIp))
        {
            _logger.LogInformation("Device IP ({deviceIp}) is banned.  Sending uninstall command.", deviceIp);

            
            var bannedDevices = _serviceSessionCache.GetAllDevices().Where(x => x.PublicIP == deviceIp);
            var connectionIds = _serviceSessionCache.GetConnectionIdsByDeviceIds(bannedDevices.Select(x => x.ID));
            await _agentHubContext.Clients.Clients(connectionIds).UninstallAgent();

            return true;
        }

        return false;
    }
}
