using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Immense.RemoteControl.Desktop.Shared.Enums;
using Immense.RemoteControl.Desktop.Shared.Services;
using Immense.RemoteControl.Desktop.UI.Services;
using Immense.RemoteControl.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Immense.RemoteControl.Desktop.Linux.Services;

internal class AppStartup : IAppStartup
{
    private readonly IAppState _appState;
    private readonly IKeyboardMouseInput _inputService;
    private readonly IDesktopHubConnection _desktopHub;
    private readonly IClipboardService _clipboardService;
    private readonly IChatHostService _chatHostService;
    private readonly ICursorIconWatcher _cursorIconWatcher;
    private readonly IUiDispatcher _dispatcher;
    private readonly IIdleTimer _idleTimer;
    private readonly IShutdownService _shutdownService;
    private readonly IBrandingProvider _brandingProvider;
    private readonly ILogger<AppStartup> _logger;

    public AppStartup(
        IAppState appState,
        IKeyboardMouseInput inputService,
        IDesktopHubConnection desktopHub,
        IClipboardService clipboardService,
        IChatHostService chatHostService,
        ICursorIconWatcher iconWatcher,
        IUiDispatcher dispatcher,
        IIdleTimer idleTimer,
        IShutdownService shutdownService,
        IBrandingProvider brandingProvider,
        ILogger<AppStartup> logger)
    {
        _appState = appState;
        _inputService = inputService;
        _desktopHub = desktopHub;
        _clipboardService = clipboardService;
        _chatHostService = chatHostService;
        _cursorIconWatcher = iconWatcher;
        _dispatcher = dispatcher;
        _idleTimer = idleTimer;
        _shutdownService = shutdownService;
        _brandingProvider = brandingProvider;
        _logger = logger;
    }

    public async Task Run()
    {
        await _brandingProvider.Initialize();

        if (_appState.Mode is AppMode.Unattended or AppMode.Attended)
        {
            _clipboardService.BeginWatching();
            _inputService.Init();
            _cursorIconWatcher.OnChange += CursorIconWatcher_OnChange;
        }

        switch (_appState.Mode)
        {
            case AppMode.Unattended:
                {
                    var result = await _dispatcher.StartHeadless();
                    if (!result.IsSuccess)
                    {
                        return;
                    }
                    await StartScreenCasting().ConfigureAwait(false);
                    break;
                }
            case AppMode.Attended:
                {
                    _dispatcher.StartClassicDesktop();
                    break;
                }
            case AppMode.Chat:
                {
                    var result = await _dispatcher.StartHeadless();
                    if (!result.IsSuccess)
                    {
                        return;
                    }
                    await _chatHostService
                        .StartChat(_appState.PipeName, _appState.OrganizationName)
                        .ConfigureAwait(false);
                    break;
                }
            default:
                break;
        }
    }


    private async Task StartScreenCasting()
    {
        if (!await _desktopHub.Connect(TimeSpan.FromSeconds(30), _dispatcher.ApplicationExitingToken))
        {
            await _shutdownService.Shutdown();
            return;
        }

        var result = await _desktopHub.SendUnattendedSessionInfo(
            _appState.SessionId,
            _appState.AccessKey,
            Environment.MachineName,
            _appState.RequesterName,
            _appState.OrganizationName);

        if (!result.IsSuccess)
        {
            _logger.LogError(result.Exception, "An error occurred while trying to establish a session with the server.");
            await _shutdownService.Shutdown();
            return;
        }

        try
        {
            if (_appState.ArgDict.ContainsKey("relaunch"))
            {
                _logger.LogInformation("Resuming after relaunch.");
                var viewerIDs = _appState.RelaunchViewers;
                await _desktopHub.NotifyViewersRelaunchedScreenCasterReady(viewerIDs);
            }
            else
            {
                await _desktopHub.NotifyRequesterUnattendedReady();
            }
        }
        finally
        {
            _idleTimer.Start();
        }
    }



    private async void CursorIconWatcher_OnChange(object? sender, CursorInfo cursor)
    {
        if (_appState.Viewers.Any() == true &&
            _desktopHub.IsConnected)
        {
            foreach (var viewer in _appState.Viewers.Values)
            {
                await viewer.SendCursorChange(cursor);
            }
        }
    }
}
