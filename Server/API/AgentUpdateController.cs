using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Remotely.Server.Services;
using Remotely.Shared.Enums;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentUpdateController : ControllerBase
    {
        private static readonly MemoryCache _downloadingAgents = new MemoryCache(new MemoryCacheOptions()
        { ExpirationScanFrequency = TimeSpan.FromSeconds(10) });


        public AgentUpdateController(IWebHostEnvironment hostingEnv,
            IDataService dataService,
            IApplicationConfig appConfig)
        {
            HostEnv = hostingEnv;
            DataService = dataService;
            AppConfig = appConfig;
        }

        private IDataService DataService { get; }
        public IApplicationConfig AppConfig { get; }
        private IWebHostEnvironment HostEnv { get; }


        [HttpGet("[action]/{downloadId}")]
        public ActionResult ClearDownload(string downloadId)
        {
            DataService.WriteEvent($"Clearing download ID {downloadId}.", EventType.Debug, null);
            _downloadingAgents.Remove(downloadId);
            return Ok();
        }

        [HttpGet("[action]/{platform}/{downloadId}")]
        public async Task<ActionResult> DownloadPackage(string platform, string downloadId)
        {
            try
            {
                var startWait = DateTimeOffset.Now;

                while (_downloadingAgents.Count >= AppConfig.MaxConcurrentUpdates)
                {
                    await Task.Delay(new Random().Next(100, 10000));

                    _downloadingAgents.TryGetValue(string.Empty, out _);
                }

                var entryExpirationTime = TimeSpan.FromMinutes(6);
                var tokenExpirationTime = entryExpirationTime.Add(TimeSpan.FromSeconds(15));

                var expirationToken = new CancellationChangeToken(
                    new CancellationTokenSource(tokenExpirationTime).Token);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(entryExpirationTime)
                    .AddExpirationToken(expirationToken);

                _downloadingAgents.Set(downloadId, string.Empty, cacheOptions);

                var waitTime = DateTimeOffset.Now - startWait;
                DataService.WriteEvent($"Download started after wait time of {waitTime}.  " + "" +
                    $"Current Downloads: {_downloadingAgents.Count}.  Max Allowed: {AppConfig.MaxConcurrentUpdates}", EventType.Debug, null);


                string filePath;

                switch (platform.ToLower())
                {
                    case "win-x64":
                        filePath = Path.Combine(HostEnv.WebRootPath, "Downloads", "Remotely-Win10-x64.zip");
                        break;
                    case "win-x86":
                        filePath = Path.Combine(HostEnv.WebRootPath, "Downloads", "Remotely-Win10-x86.zip");
                        break;
                    case "linux":
                        filePath = Path.Combine(HostEnv.WebRootPath, "Downloads", "Remotely-Linux.zip");
                        break;
                    default:
                        return BadRequest();
                }

                var fileStream = System.IO.File.OpenRead(filePath);

                return File(fileStream, "application/octet-stream", "RemotelyUpdate.zip");
            }
            catch (Exception ex)
            {
                _downloadingAgents.Remove(downloadId);
                DataService.WriteEvent(ex, null);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
