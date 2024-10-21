using Avalonia.Controls;
using Remotely.Desktop.Shared.Services;
using Remotely.Shared.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Remotely.Desktop.Shared.Reactive;
using Microsoft.Extensions.DependencyInjection;
using Remotely.Desktop.UI.Controls.Dialogs;

namespace Remotely.Desktop.UI.ViewModels;

public interface IMainViewViewModel : IBrandedViewModelBase
{
    AsyncRelayCommand ChangeServerCommand { get; }
    AsyncRelayCommand CopyLinkCommand { get; }
    double CopyMessageOpacity { get; set; }
    string Host { get; set; }
    bool IsCopyMessageVisible { get; set; }
    ICommand OpenOptionsMenu { get; }
    AsyncRelayCommand RemoveViewersCommand { get; }
    IList<IViewer> SelectedViewers { get; }
    string StatusMessage { get; set; }
    ObservableCollection<IViewer> Viewers { get; }
    Task ChangeServer();
    Task CopyLink();
    Task GetSessionID();
    Task Init();
    Task PromptForHostName();
    Task RemoveViewers();
}

public class MainViewViewModel : BrandedViewModelBase, IMainViewViewModel
{
    private readonly IAppState _appState;
    private readonly IDesktopEnvironment _environment;
    private readonly IDialogProvider _dialogProvider;
    private readonly IDesktopHubConnection _hubConnection;
    private readonly IServiceProvider _serviceProvider;
    private readonly IViewModelFactory _viewModelFactory;
    private IList<IViewer> _selectedViewers = new List<IViewer>();

    public MainViewViewModel(
      IBrandingProvider brandingProvider,
      IUiDispatcher dispatcher,
      IAppState appState,
      IDesktopHubConnection hubConnection,
      IServiceProvider serviceProvider,
      IViewModelFactory viewModelFactory,
      IDesktopEnvironment environmentHelper,
      IDialogProvider dialogProvider,
      ILogger<MainViewViewModel> logger)
      : base(brandingProvider, dispatcher, logger)
    {
        _appState = appState;
        _hubConnection = hubConnection;
        _serviceProvider = serviceProvider;
        _viewModelFactory = viewModelFactory;
        _environment = environmentHelper;
        _dialogProvider = dialogProvider;

        _appState.ViewerRemoved += ViewerRemoved;
        _appState.ViewerAdded += ViewerAdded;
        _appState.ScreenCastRequested += ScreenCastRequested;

        Host = appState.Host;
        ChangeServerCommand = new AsyncRelayCommand(ChangeServer);
        CopyLinkCommand = new AsyncRelayCommand(CopyLink);
        RemoveViewersCommand = new AsyncRelayCommand(RemoveViewers, CanRemoveViewers);
    }

    public AsyncRelayCommand ChangeServerCommand { get; }

    public AsyncRelayCommand CopyLinkCommand { get; }

    public double CopyMessageOpacity
    {
        get => Get<double>();
        set => Set(value);
    }

    public string Host
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    public bool IsCopyMessageVisible
    {
        get => Get<bool>();
        set => Set(value);
    }

    public ICommand OpenOptionsMenu { get; } = new RelayCommand<Button>(button =>
    {
        button?.ContextMenu?.Open(button);
    });

    public AsyncRelayCommand RemoveViewersCommand { get; }

    public IList<IViewer> SelectedViewers
    {
        get => _selectedViewers;
        set
        {
            _selectedViewers = value ?? new List<IViewer>();
            NotifyPropertyChanged();
            RemoveViewersCommand.NotifyCanExecuteChanged();
        }
    }

    public string StatusMessage
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    public ObservableCollection<IViewer> Viewers { get; } = new();

    public async Task ChangeServer()
    {
        await PromptForHostName();
        await Init();
    }

    public async Task CopyLink()
    {
        if (_dispatcher?.Clipboard is null)
        {
            return;
        }

        await _dispatcher.Clipboard.SetTextAsync($"{Host}/Viewer?sessionId={StatusMessage.Replace(" ", "")}");

        CopyMessageOpacity = 1;
        IsCopyMessageVisible = true;
        await Task.Delay(1000);
        while (CopyMessageOpacity > 0)
        {
            CopyMessageOpacity -= .05;
            await Task.Delay(25);
        }
        IsCopyMessageVisible = false;
    }

    public async Task GetSessionID()
    {
        var sessionId = await _hubConnection.GetSessionID();
        await _hubConnection.SendAttendedSessionInfo(Environment.MachineName);

        var formattedSessionID = "";
        for (var i = 0; i < sessionId.Length; i += 3)
        {
            formattedSessionID += $"{sessionId.Substring(i, 3)} ";
        }

        await _dispatcher.InvokeAsync(() =>
        {
            StatusMessage = formattedSessionID.Trim();
        });
    }

    public async Task Init()
    {
        if (!_environment.IsDebug && 
            OperatingSystem.IsLinux() && 
            !_environment.IsElevated)
        {
            await _dialogProvider.Show("Please run with sudo.", "Sudo Required", MessageBoxType.OK);
            Environment.Exit(0);
        }

        StatusMessage = "Initializing...";

        await InstallDependencies();

        StatusMessage = "Retrieving...";

        while (string.IsNullOrWhiteSpace(Host))
        {
            Host = "https://";
            await PromptForHostName();
        }

        _appState.Host = Host;
        _appState.Mode = Shared.Enums.AppMode.Attended;

        try
        {
            var result = await _hubConnection.Connect(TimeSpan.FromSeconds(10), _dispatcher.ApplicationExitingToken);

            if (result && _hubConnection.Connection is not null)
            {
                _hubConnection.Connection.Closed += async (ex) =>
                {
                    await _dispatcher.InvokeAsync(() =>
                    {
                        Viewers.Clear();
                        StatusMessage = "Disconnected";
                    });
                };

                _hubConnection.Connection.Reconnecting += async (ex) =>
                {
                    await _dispatcher.InvokeAsync(() =>
                    {
                        Viewers.Clear();
                        StatusMessage = "Reconnecting";
                    });
                };

                _hubConnection.Connection.Reconnected += async (id) =>
                {
                    await GetSessionID();
                };

            }

            await GetSessionID();

            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during initialization.");
        }

        // If we got here, something went wrong.
        StatusMessage = "Failed";
        await _dialogProvider.Show("Failed to connect to server.", "Connection Failed", MessageBoxType.OK);
    }

    public async Task PromptForHostName()
    {
        var viewModel = _viewModelFactory.CreateHostNamePromptViewModel();
        var prompt = new HostNamePrompt()
        {
            DataContext = viewModel
        };

        if (!string.IsNullOrWhiteSpace(Host))
        {
            viewModel.Host = Host;
        }

        await _dispatcher.ShowDialog(prompt);

        var result = prompt.ViewModel?.Host?.Trim()?.TrimEnd('/');

        if (!Uri.TryCreate(result, UriKind.Absolute, out var serverUri) ||
            serverUri.Scheme != Uri.UriSchemeHttp && serverUri.Scheme != Uri.UriSchemeHttps)
        {
            _logger.LogWarning("Server URL is not valid.");
            await _dialogProvider.Show("Server URL must be a valid Uri (e.g. https://example.com).", "Invalid Server URL", MessageBoxType.OK);
            return;
        }

        Host = result;
    }

    public async Task RemoveViewers()
    {
        if (!SelectedViewers.Any())
        {
            return;
        }

        foreach (var viewer in SelectedViewers)
        {
            await _hubConnection.DisconnectViewer(viewer, true);
        }
    }

    private bool CanRemoveViewers()
    {
        return SelectedViewers.Any() == true;
    }

    private async Task InstallDependencies()
    {
        if (OperatingSystem.IsLinux())
        {
            try
            {
                var psi = new ProcessStartInfo()
                {
                    FileName = "sudo",
                    Arguments = "bash -c \"apt-get -y install libx11-dev ; " +
                        "apt-get -y install libxrandr-dev ; " +
                        "apt-get -y install libc6-dev ; " +
                        "apt-get -y install libxtst-dev ; " +
                        "apt-get -y install xclip\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                };

                await Task.Run(() => Process.Start(psi)?.WaitForExit());
            }
            catch
            {
                _logger.LogError("Failed to install dependencies.");
            }
        }


    }

    private async void ScreenCastRequested(object? sender, ScreenCastRequest screenCastRequest)
    {
        var result = await _dialogProvider.Show(
            $"You've received a connection request from {screenCastRequest.RequesterName}.  Accept?",
            "Connection Request",
            MessageBoxType.YesNo);

        if (result == MessageBoxResult.Yes)
        {
            using var screenCaster = _serviceProvider.GetRequiredService<IScreenCaster>();
            await screenCaster.BeginScreenCasting(screenCastRequest);
        }
        else
        {
            await _hubConnection.SendConnectionRequestDenied(screenCastRequest.ViewerId);
        }
    }

    private async void ViewerAdded(object? sender, IViewer viewer)
    {
        await _dispatcher.InvokeAsync(() =>
        {
            Viewers.Add(viewer);
        });
    }

    private async void ViewerRemoved(object? sender, string viewerID)
    {
        await _dispatcher.InvokeAsync(() =>
        {
            var viewer = Viewers.FirstOrDefault(x => x.ViewerConnectionId == viewerID);
            if (viewer != null)
            {
                Viewers.Remove(viewer);
            }
        });
    }
}
