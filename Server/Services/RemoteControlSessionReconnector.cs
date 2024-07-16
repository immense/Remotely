using Immense.RemoteControl.Server.Abstractions;
using Immense.RemoteControl.Server.Enums;
using Immense.RemoteControl.Server.Hubs;
using Immense.RemoteControl.Shared.Helpers;
using Immense.RemoteControl.Shared.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Immense.RemoteControl.Server.Services;

internal class RemoteControlSessionReconnector : BackgroundService
{
    private readonly IRemoteControlSessionCache _sessionCache;
    private readonly IHubEventHandler _hubEvents;
    private readonly IHubContext<ViewerHub, IViewerHubClient> _viewerHub;
    private readonly ILogger<RemoteControlSessionCleaner> _logger;

    public RemoteControlSessionReconnector(
        IRemoteControlSessionCache sessionCache,
        IHubEventHandler hubEvents,
        IHubContext<ViewerHub, IViewerHubClient> viewerHub,
        ILogger<RemoteControlSessionCleaner> logger)
    {
        _sessionCache = sessionCache;
        _hubEvents = hubEvents;
        _viewerHub = viewerHub;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            foreach (var session in _sessionCache.Sessions)
            {
                try
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        break;
                    }

                    if (session.Mode != Models.RemoteControlMode.Unattended)
                    {
                        continue;
                    }

                    // Skip sessions that are either:
                    //   - Already connected
                    //   - In the process of shutting down
                    //   - Have had a state change in the last 5 seconds
                    //   - Have no viewers connected
                    if (session.StreamerState.HasFlag(StreamerState.Connected) ||
                        session.StreamerState.HasFlag(StreamerState.WindowsShuttingDown) ||
                        session.LastStateChange > DateTimeOffset.Now.AddSeconds(-5) ||
                        !session.ViewerList.Any())
                    {
                        continue;
                    }

                    await RateLimiter.Throttle(async () =>
                        {
                            await _viewerHub.Clients.Clients(session.ViewerList).Reconnecting();
                            await _hubEvents.RestartScreenCaster(session);
                        },
                        TimeSpan.FromSeconds(10),
                        key: $"{session.UnattendedSessionId}",
                        cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while removing expired desktop sessions.");
                }
            }
        }
    }
}
