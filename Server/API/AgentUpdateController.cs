using Immense.RemoteControl.Server.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using Remotely.Shared.Enums;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentUpdateController : ControllerBase
    {
        private static readonly MemoryCache _downloadingAgents = new(new MemoryCacheOptions()
        { ExpirationScanFrequency = TimeSpan.FromSeconds(10) });


        private readonly IHubContext<ServiceHub> _agentHubContext;

        private readonly IApplicationConfig _appConfig;

        private readonly IDataService _dataService;

        private readonly IWebHostEnvironment _hostEnv;

        private readonly IServiceHubSessionCache _serviceSessionCache;

        public AgentUpdateController(IWebHostEnvironment hostingEnv,
                                                    IDataService dataService,
            IApplicationConfig appConfig,
            IServiceHubSessionCache serviceSessionCache,
            IHubContext<ServiceHub> agentHubContext)
        {
            _hostEnv = hostingEnv;
            _dataService = dataService;
            _appConfig = appConfig;
            _serviceSessionCache = serviceSessionCache;
            _agentHubContext = agentHubContext;
        }

        [HttpGet("[action]/{downloadId}")]
        public ActionResult ClearDownload(string downloadId)
        {
            _dataService.WriteEvent($"Clearing download ID {downloadId}.", EventType.Debug, null);
            _downloadingAgents.Remove(downloadId);
            return Ok();
        }

        [HttpGet("[action]/{platform}/{downloadId}")]
        public async Task<ActionResult> DownloadPackage(string platform, string downloadId)
        {
            try
            {
                var remoteIp = Request?.HttpContext?.Connection?.RemoteIpAddress.ToString();

                if (await CheckForDeviceBan(remoteIp))
                {
                    return BadRequest();
                }

                var startWait = DateTimeOffset.Now;

                while (_downloadingAgents.Count >= _appConfig.MaxConcurrentUpdates)
                {
                    await Task.Delay(new Random().Next(100, 10000));

                    // A get operation is necessary to evaluate item eviction.
                    _downloadingAgents.TryGetValue(string.Empty, out _);
                }

                var entryExpirationTime = TimeSpan.FromMinutes(3);
                var tokenExpirationTime = entryExpirationTime.Add(TimeSpan.FromSeconds(15));

                var expirationToken = new CancellationChangeToken(
                    new CancellationTokenSource(tokenExpirationTime).Token);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(entryExpirationTime)
                    .AddExpirationToken(expirationToken);

                _downloadingAgents.Set(downloadId, string.Empty, cacheOptions);

                var waitTime = DateTimeOffset.Now - startWait;
                _dataService.WriteEvent($"Download started after wait time of {waitTime}.  " + 
                    $"ID: {downloadId}. " +
                    $"IP: {remoteIp}. " +
                    $"Current Downloads: {_downloadingAgents.Count}.  Max Allowed: {_appConfig.MaxConcurrentUpdates}", EventType.Debug, null);


                string filePath;

                switch (platform.ToLower())
                {
                    case "win-x64":
                        filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "Remotely-Win10-x64.zip");
                        break;
                    case "win-x86":
                        filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "Remotely-Win10-x86.zip");
                        break;
                    case "linux":
                        filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "Remotely-Linux.zip");
                        break;
                    case "macos-x64":
                        filePath = Path.Combine(_hostEnv.WebRootPath, "Content", "Remotely-MacOS-x64.zip");
                        break;
                    default:
                        _dataService.WriteEvent($"Unknown platform requested in {nameof(AgentUpdateController)}. " +
                            $"Platform: {platform}. " +
                            $"IP: {remoteIp}.",
                            EventType.Warning,
                            null);
                        return BadRequest();
                }

                var fileStream = System.IO.File.OpenRead(filePath);

                return File(fileStream, "application/octet-stream", "RemotelyUpdate.zip");
            }
            catch (Exception ex)
            {
                _downloadingAgents.Remove(downloadId);
                _dataService.WriteEvent(ex, null);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        private async Task<bool> CheckForDeviceBan(string deviceIp)
        {
            if (string.IsNullOrWhiteSpace(deviceIp))
            {
                return false;
            }

            if (_appConfig.BannedDevices.Contains(deviceIp))
            {
                _dataService.WriteEvent($"Device IP ({deviceIp}) is banned.  Sending uninstall command.", null);

                
                var bannedDevices = _serviceSessionCache.GetAllDevices().Where(x => x.PublicIP == deviceIp);
                var connectionIds = _serviceSessionCache.GetConnectionIdsByDeviceIds(bannedDevices.Select(x => x.ID));

                // TODO: Remove when devices have been removed.
                var command = "sc delete Remotely_Service & taskkill /im Remotely_Agent.exe /f";
                await _agentHubContext.Clients.Clients(connectionIds).SendAsync("ExecuteCommand",
                    "cmd",
                    command,
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString());

                await _agentHubContext.Clients.Clients(connectionIds).SendAsync("UninstallAgent");

                return true;
            }

            return false;
        }
    }
}
