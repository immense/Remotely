using Remotely.Server.Enums;
using Remotely.Server.Hubs;
using Remotely.Shared.Helpers;
using Remotely.Shared.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Remotely.Server.Services;

internal class RemoteControlSessionReconnector : BackgroundService
{
    private readonly IRemoteControlSessionCache _sessionCache;
    private readonly IHubContext<ViewerHub, IViewerHubClient> _viewerHub;
    private readonly IHubContext<AgentHub, IAgentHubClient> _agentHub;
    private readonly ILogger<RemoteControlSessionCleaner> _logger;

    public RemoteControlSessionReconnector(
        IRemoteControlSessionCache sessionCache,
        IHubContext<ViewerHub, IViewerHubClient> viewerHub,
        IHubContext<AgentHub, IAgentHubClient> agentHub,
        ILogger<RemoteControlSessionCleaner> logger)
    {
        _sessionCache = sessionCache;
        _viewerHub = viewerHub;
        _agentHub = agentHub;
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
                            await _agentHub.Clients
                                .Client(session.AgentConnectionId)
                                .RestartScreenCaster(
                                    [.. session.ViewerList],
                                    $"{session.UnattendedSessionId}",
                                    session.AccessKey,
                                    session.UserConnectionId,
                                    session.RequesterName,
                                    session.OrganizationName,
                                    session.OrganizationId);
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
