using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using Remotely.Shared.Helpers;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Primitives;

namespace Remotely.Desktop.UI.Services;

public interface IUiDispatcher
{
    CancellationToken ApplicationExitingToken { get; }
    IClipboard? Clipboard { get; }
    Application? CurrentApp { get; }

    Window? MainWindow { get; }

    void Invoke(Action action);
    Task InvokeAsync(Action action, DispatcherPriority priority = default);
    Task InvokeAsync(Func<Task> func, DispatcherPriority priority = default);
    Task<T> InvokeAsync<T>(Func<Task<T>> func, DispatcherPriority priority = default);
    void Post(Action action, DispatcherPriority priority = default);
    Task<bool> Show(Window window, TimeSpan timeout);

    Task ShowDialog(Window window);
    void ShowMainWindow(Window window);

    void ShowWindow(Window window);
    void Shutdown();
    void StartClassicDesktop();
    Task<Result> StartHeadless();
}

internal class UiDispatcher : IUiDispatcher
{
    private static readonly CancellationTokenSource _appCts = new();
    private static Application? _currentApp;
    private readonly ILogger<UiDispatcher> _logger;
    private AppBuilder? _appBuilder;
    private Window? _headlessMainWindow;

    public UiDispatcher(ILogger<UiDispatcher> logger)
    {
        _logger = logger;
    }

    public CancellationToken ApplicationExitingToken => _appCts.Token;

    public IClipboard? Clipboard
    {
        get
        {
            if (CurrentApp?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopApp)
            {
                return desktopApp.MainWindow?.Clipboard;
            }

            if (CurrentApp?.ApplicationLifetime is ISingleViewApplicationLifetime svApp)
            {
                return TopLevel.GetTopLevel(svApp.MainView)?.Clipboard;
            }

            if (_headlessMainWindow is not null)
            {
                return _headlessMainWindow.Clipboard;
            }

            return null;
        }
    }

    public Application? CurrentApp => _currentApp ?? _appBuilder?.Instance;

    public Window? MainWindow
    {
        get
        {
            if (CurrentApp?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime app)
            {
                return app.MainWindow;
            }

            return _headlessMainWindow;
        }
    }
    public void Invoke(Action action)
    {
        Dispatcher.UIThread.Invoke(action);
    }

    public Task InvokeAsync(Func<Task> func, DispatcherPriority priority = default)
    {

        return Dispatcher.UIThread.InvokeAsync(func, priority);
    }

    public Task<T> InvokeAsync<T>(Func<Task<T>> func, DispatcherPriority priority = default)
    {
        return Dispatcher.UIThread.InvokeAsync(func, priority);
    }

    public async Task InvokeAsync(Action action, DispatcherPriority priority = default)
    {
        await Dispatcher.UIThread.InvokeAsync(action, priority);
    }

    public void Post(Action action, DispatcherPriority priority = default)
    {
        Dispatcher.UIThread.Post(action, priority);
    }

    public async Task<bool> Show(Window window, TimeSpan timeout)
    {
        return await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            using var closeSignal = new SemaphoreSlim(0, 1);
            window.Closed += (sender, arg) =>
            {
                closeSignal.Release();
            };

            window.Show();
            var result = await closeSignal.WaitAsync(timeout);
            if (!result)
            {
                window.Close();
            }

            return result;
        });
    }

    public async Task ShowDialog(Window window)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (MainWindow is not null)
            {
                await window.ShowDialog(MainWindow);
            }
            else
            {
                using var closeSignal = new SemaphoreSlim(0, 1);
                window.Closed += (sender, arg) =>
                {
                    closeSignal.Release();
                };
                window.Show();

                await closeSignal.WaitAsync();
            }
        });
    }
    public void ShowMainWindow(Window window)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            _headlessMainWindow = window;
            window.Show();
        });
    }

    public void ShowWindow(Window window)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            if (MainWindow is not null)
            {
                window.Show(MainWindow);
            }
            else
            {
                window.Show();
            }
        });
    }

    public void Shutdown()
    {
        _appCts.Cancel();
        if (CurrentApp?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime &&
            lifetime.TryShutdown())
        {
            return;
        }

        Environment.Exit(0);
    }

    public void StartClassicDesktop()
    {
        try
        {
            var args = Environment.GetCommandLineArgs();
            _appBuilder = BuildAvaloniaApp();
            _appBuilder.StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while starting foreground app.");
            throw;
        }
    }

    public async Task<Result> StartHeadless()
    {
        try
        {
            var args = Environment.GetCommandLineArgs();
            var argString = string.Join(" ", args);
            _logger.LogInformation("Starting dispatcher in unattended mode with args: [{args}].", argString);

            _ = Task.Run(() =>
            {
                _appBuilder = BuildAvaloniaApp();
                _appBuilder.Start(RunHeadless, args);
            }, _appCts.Token);

            var waitResult = await WaitHelper.WaitForAsync(
                    () => CurrentApp is not null,
                    TimeSpan.FromSeconds(10))
                .ConfigureAwait(false);

            if (!waitResult)
            {
                const string err = "Unattended dispatcher failed to start in time.";
                _logger.LogError(err);
                Shutdown();
                return Result.Fail(err);
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while starting background app.");
            return Result.Fail(ex);
        }
    }
    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    private static void RunHeadless(Application app, string[] args)
    {
        _currentApp = app;
        app.Run(_appCts.Token);
    }
}
