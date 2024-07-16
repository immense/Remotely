using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Immense.RemoteControl.Server.Services;

public class RemoteControlSessionCleaner : BackgroundService
{
    private readonly IRemoteControlSessionCache _sessionCache;
    private readonly ILogger<RemoteControlSessionCleaner> _logger;

    public RemoteControlSessionCleaner(
        IRemoteControlSessionCache sessionCache,
        ILogger<RemoteControlSessionCleaner> logger)
    {
        _sessionCache = sessionCache;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await _sessionCache.RemoveExpiredSessions();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while removing expired desktop sessions.");
            }
        }
    }
}
