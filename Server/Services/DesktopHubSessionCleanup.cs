using Immense.RemoteControl.Server.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;
using Timer = System.Timers.Timer;
using Immense.RemoteControl.Shared.Services;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Collections.Generic;
using Immense.RemoteControl.Server.Models;

namespace Remotely.Server.Services
{
    public class DesktopHubSessionCleanup : BackgroundService
    {
        private readonly IDesktopHubSessionCache _sessionCache;
        private readonly ISystemTime _systemTime;
        private readonly ILogger<DesktopHubSessionCleanup> _logger;
        private Task _cleanupTask;
        private CancellationToken _stoppingToken;

        public DesktopHubSessionCleanup(
            IDesktopHubSessionCache sessionCache, 
            ISystemTime systemTime,
            ILogger<DesktopHubSessionCleanup> logger)
        {
            _sessionCache = sessionCache;
            _systemTime = systemTime;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _stoppingToken = stoppingToken;
            _cleanupTask = Task.Run(CleanupSessions, stoppingToken);
            return Task.CompletedTask;
        }

        private async Task CleanupSessions()
        {
            while (!_stoppingToken.IsCancellationRequested)
            {
                foreach (var session in _sessionCache.Sessions)
                {
                    if (session.Value.Mode == RemoteControlMode.Unattended &&
                        !session.Value.ViewerList.Any() &&
                        session.Value.Created < _systemTime.Now.AddMinutes(-1))
                    {
                        _logger.LogWarning("Removing expired session: {session}", JsonSerializer.Serialize(session.Value));
                        _sessionCache.Sessions.Remove(session.Key, out _);
                    }
                }
                await Task.Delay(30_000, _stoppingToken);
            }
        }
    }
}
