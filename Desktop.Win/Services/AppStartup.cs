using Remotely.Desktop.Native.Windows;
using Remotely.Desktop.Shared.Abstractions;
using Remotely.Desktop.Shared.Enums;
using Remotely.Desktop.Shared.Services;
using Remotely.Desktop.UI.Services;
using Remotely.Shared.Models;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Remotely.Desktop.Win;   // <— add this if not already present




namespace Remotely.Desktop.Win.Services;

internal class AppStartup : IAppStartup
{
    private readonly IAppState _appState;
    private readonly IKeyboardMouseInput _inputService;
    private readonly IDesktopHubConnection _desktopHub;
    private readonly IClipboardService _clipboardService;
    private readonly IChatHostService _chatHostService;
    private readonly ICursorIconWatcher _cursorIconWatcher;
    private readonly IMessageLoop _messageLoop;
    private readonly IUiDispatcher _uiDispatcher;
    private readonly IIdleTimer _idleTimer;
    private readonly IShutdownService _shutdownService;
    private readonly IBrandingProvider _brandingProvider;
    private readonly ILogger<AppStartup> _logger;
    private NotifyIcon? _tray;
    private ToolStripMenuItem? _privacyItem;


    public AppStartup(
        IAppState appState,
        IKeyboardMouseInput inputService,
        IDesktopHubConnection desktopHub,
        IClipboardService clipboardService,
        IChatHostService chatHostService,
        ICursorIconWatcher iconWatcher,
        IMessageLoop messageLoop,
        IUiDispatcher uiDispatcher,
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
        _messageLoop = messageLoop;
        _uiDispatcher = uiDispatcher;
        _idleTimer = idleTimer;
        _shutdownService = shutdownService;
        _brandingProvider = brandingProvider;
        _logger = logger;
    }

    public async Task Run()
    {
        await _brandingProvider.Initialize();


        _messageLoop.StartMessageLoop();
        // ===== Tray icon + Privacy toggle =====
        if (_tray is null)
        {
            _tray = new NotifyIcon();

            // Try to load an icon from Assets; fallback to default
            var iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "favicon.ico");
            _tray.Icon = File.Exists(iconPath) ? new Icon(iconPath) : SystemIcons.Application;
            _tray.Visible = true;
            _tray.Text = "Remotely Agent";

            var menu = new ContextMenuStrip();

            _privacyItem = new ToolStripMenuItem("Privacy Mode (Black Screen)")
            {
                CheckOnClick = true
            };
            _privacyItem.CheckedChanged += (s, e) =>
            {
                if (_privacyItem.Checked)
                {
                    PrivacyOverlay.Enable();
                }
                else
                {
                    PrivacyOverlay.Disable();
                }
            };

            var exitItem = new ToolStripMenuItem("Exit Agent");
            exitItem.Click += (s, e) =>
            {
                try { PrivacyOverlay.Disable(); } catch { }
                if (_tray is not null) { _tray.Visible = false; }
                Application.Exit();
            };

            menu.Items.Add(_privacyItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(exitItem);

            _tray.ContextMenuStrip = menu;
        }

        // Ensure cleanup when the app is exiting (service stop / app exit)
        _uiDispatcher.ApplicationExitingToken.Register(() =>
        {
            try { PrivacyOverlay.Disable(); } catch { }
            if (_tray is not null)
            {
                _tray.Visible = false;
                _tray.Dispose();
                _tray = null;
            }
        });


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
                    var result = await _uiDispatcher.StartHeadless().ConfigureAwait(false);
                    if (!result.IsSuccess)
                    {
                        return;
                    }
                    await StartScreenCasting().ConfigureAwait(false);
                    break;
                }
            case AppMode.Attended:
                {
                    _uiDispatcher.StartClassicDesktop();
                    break;
                }
            case AppMode.Chat:
                {
                    var result = await _uiDispatcher.StartHeadless().ConfigureAwait(false);
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
        if (!await _desktopHub.Connect(TimeSpan.FromSeconds(30), _uiDispatcher.ApplicationExitingToken))
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
            if (Win32Interop.GetCurrentDesktop(out var currentDesktopName))
            {
                _logger.LogInformation("Setting initial desktop to {currentDesktopName}.", currentDesktopName);
            }
            else
            {
                _logger.LogWarning("Failed to get initial desktop name.");
            }

            if (!Win32Interop.SwitchToInputDesktop())
            {
                _logger.LogWarning("Failed to set initial desktop.");
            }

            if (_appState.IsRelaunch)
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
