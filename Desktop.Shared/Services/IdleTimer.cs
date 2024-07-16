using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System.Timers;

namespace Immense.RemoteControl.Desktop.Shared.Services;

public interface IIdleTimer
{
    DateTimeOffset ViewersLastSeen { get; }

    void Start();
    void Stop();
}

public class IdleTimer : IIdleTimer
{
    private readonly IAppState _appState;
    private readonly IRemoteControlAccessService _accessService;
    private readonly IDesktopHubConnection _desktopHubConnection;
    private readonly IShutdownService _shutdownService;
    private readonly ILogger<IdleTimer> _logger;
    private readonly SemaphoreSlim _elapseLock = new(1, 1);
    private System.Timers.Timer? _timer;

    public IdleTimer(
        IAppState appState,
        IRemoteControlAccessService accessService,
        IDesktopHubConnection desktopHubConnection,
        IShutdownService shutdownService,
        ILogger<IdleTimer> logger)
    {
        _appState = appState;
        _accessService = accessService;
        _desktopHubConnection = desktopHubConnection;
        _shutdownService = shutdownService;
        _logger = logger;
    }


    public DateTimeOffset ViewersLastSeen { get; private set; } = DateTimeOffset.Now;


    public void Start()
    {
        _logger.LogInformation("Starting idle timer.");
        _timer?.Dispose();
        _timer = new System.Timers.Timer(100);
        _timer.Elapsed += Timer_Elapsed;
        _timer.Start();
    }

    public void Stop()
    {
        _timer?.Stop();
        _timer?.Dispose();
    }

    private async void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        if (!await _elapseLock.WaitAsync(0))
        {
            return;
        }

        try
        {
            if (_appState.Mode == Enums.AppMode.Unattended &&
                !_desktopHubConnection.IsConnected)
            {
                _logger.LogWarning(
                    "App is in unattended mode and is disconnected " +
                    "from the server.  Shutting down.");
                await _shutdownService.Shutdown();
                return;
            }

            if (!_appState.Viewers.IsEmpty ||
                _accessService.IsPromptOpen)
            {
                ViewersLastSeen = DateTimeOffset.Now;
                return;
            }

            if (DateTimeOffset.Now - ViewersLastSeen > TimeSpan.FromSeconds(30))
            {
                _logger.LogWarning("No viewers connected for 30 seconds.  Shutting down.");
                await _shutdownService.Shutdown();
            }
        }
        finally
        {
            _elapseLock.Release();
        }
    }
}
