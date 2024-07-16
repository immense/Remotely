using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Immense.RemoteControl.Desktop.Shared.Services;
using Microsoft.Extensions.Logging;
using Immense.RemoteControl.Desktop.UI.Services;

namespace Immense.RemoteControl.Desktop.Linux.Services;

public class ShutdownServiceLinux : IShutdownService
{
    private readonly IDesktopHubConnection _hubConnection;
    private readonly IUiDispatcher _dispatcher;
    private readonly IAppState _appState;
    private readonly ILogger<ShutdownServiceLinux> _logger;

    public ShutdownServiceLinux(
        IDesktopHubConnection hubConnection,
        IUiDispatcher dispatcher,
        IAppState appState,
        ILogger<ShutdownServiceLinux> logger)
    {
        _hubConnection = hubConnection;
        _dispatcher = dispatcher;
        _appState = appState;
        _logger = logger;
    }

    public async Task Shutdown()
    {
        _logger.LogDebug("Exiting process ID {processId}.", Environment.ProcessId);
        await TryDisconnectViewers();
        _dispatcher.Shutdown();
    }

    private async Task TryDisconnectViewers()
    {
        try
        {
            if (_hubConnection.IsConnected && _appState.Viewers.Any())
            {
                await _hubConnection.DisconnectAllViewers();
                await _hubConnection.Disconnect();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending shutdown notice to viewers.");
        }
    }
}
