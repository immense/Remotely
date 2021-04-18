using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Remotely.Desktop.Core;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Services;
using Remotely.Desktop.Linux.Controls;
using Remotely.Desktop.Linux.Native;
using Remotely.Desktop.Linux.Services;
using Remotely.Desktop.Linux.Views;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Remotely.Desktop.Linux.ViewModels
{
    public class MainWindowViewModel : BrandedViewModelBase
    {
        private readonly ICasterSocket _casterSocket;
        private readonly Conductor _conductor;
        private readonly IConfigService _configService;
        private double _copyMessageOpacity;
        private string _host;
        private bool _isCopyMessageVisible;
        private string _sessionID;

        public MainWindowViewModel()
        {
            Current = this;

            _configService = Services.GetRequiredService<IConfigService>();
            _conductor = Services.GetRequiredService<Conductor>();
            _casterSocket = Services.GetRequiredService<ICasterSocket>();

            _conductor.SessionIDChanged += SessionIDChanged;
            _conductor.ViewerRemoved += ViewerRemoved;
            _conductor.ViewerAdded += ViewerAdded;
            _conductor.ScreenCastRequested += ScreenCastRequested;

            if (!EnvironmentHelper.IsLinux)
            {
                return;
            }

            Services.GetRequiredService<IClipboardService>().BeginWatching();
            Services.GetRequiredService<IKeyboardMouseInput>().Init();
        }


        public static MainWindowViewModel Current { get; private set; }

        public ICommand ChangeServerCommand => new Executor(async (param) =>
        {
            await PromptForHostName();
            await Init();
        });

        public ICommand CloseCommand => new Executor((param) =>
        {
            (param as Window)?.Close();
            Environment.Exit(0);
        });

        public ICommand CopyLinkCommand => new Executor(async (param) =>
        {
            await App.Current.Clipboard.SetTextAsync($"{Host}/RemoteControl?sessionID={SessionID.Replace(" ", "")}");

            CopyMessageOpacity = 1;
            IsCopyMessageVisible = true;
            await Task.Delay(1000);
            while (_copyMessageOpacity > 0)
            {
                CopyMessageOpacity -= .05;
                await Task.Delay(25);
            }
            IsCopyMessageVisible = false;
        });

        public double CopyMessageOpacity
        {
            get => _copyMessageOpacity;
            set => this.RaiseAndSetIfChanged(ref _copyMessageOpacity, value);
        }

        public string Host
        {
            get => _host;
            set => this.RaiseAndSetIfChanged(ref _host, value);
        }

        public bool IsCopyMessageVisible
        {
            get => _isCopyMessageVisible;
            set => this.RaiseAndSetIfChanged(ref _isCopyMessageVisible, value);
        }

        public ICommand MinimizeCommand => new Executor((param) =>
        {
            (param as Window).WindowState = WindowState.Minimized;
        });

        public ICommand OpenOptionsMenu => new Executor((param) =>
        {
            if (param is Button)
            {
                (param as Button).ContextMenu?.Open(param as Button);
            }
        });

        public ICommand RemoveViewerCommand => new Executor(async (param) =>
        {
            var viewerList = param as AvaloniaList<object> ?? new AvaloniaList<object>();
            foreach (Viewer viewer in viewerList)
            {
                await _casterSocket.DisconnectViewer(viewer, true);
            }
        });

        public string SessionID
        {
            get => _sessionID;
            set => this.RaiseAndSetIfChanged(ref _sessionID, value);
        }

        public ObservableCollection<Viewer> Viewers { get; } = new ObservableCollection<Viewer>();
        private static IServiceProvider Services => ServiceContainer.Instance;
        public async Task GetSessionID()
        {
            await _casterSocket.SendDeviceInfo(_conductor.ServiceID, Environment.MachineName, _conductor.DeviceID);
            await _casterSocket.GetSessionID();
        }

        public async Task Init()
        {
            try
            {
                if (!EnvironmentHelper.IsDebug && Libc.geteuid() != 0)
                {
                    await MessageBox.Show("Please run with sudo.", "Sudo Required", MessageBoxType.OK);
                    Environment.Exit(0);
                }

                SessionID = "Initializing...";

                await InstallDependencies();

                SessionID = "Retrieving...";

                Host = _configService.GetConfig().Host;

                while (string.IsNullOrWhiteSpace(Host))
                {
                    Host = "https://";
                    await PromptForHostName();
                }

                _conductor.ProcessArgs(new string[] { "-mode", "Normal", "-host", Host });

                await _casterSocket.Connect(_conductor.Host);

                if (_casterSocket.Connection is null)
                {
                    return;
                }

                _casterSocket.Connection.Closed += async (ex) =>
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        SessionID = "Disconnected";
                    });
                };

                _casterSocket.Connection.Reconnecting += async (ex) =>
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        SessionID = "Reconnecting";
                    });
                };

                _casterSocket.Connection.Reconnected += async (arg) =>
                {
                    await GetSessionID();
                };

                await DeviceInitService.GetInitParams();

                ApplyBranding();

                await _casterSocket.SendDeviceInfo(_conductor.ServiceID, Environment.MachineName, _conductor.DeviceID);
                await _casterSocket.GetSessionID();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                _sessionID = "Failed";
                await MessageBox.Show("Failed to connect to server.", "Connection Failed", MessageBoxType.OK);
                return;
            }
        }

        public async Task PromptForHostName()
        {
            var prompt = new HostNamePrompt();

            if (!string.IsNullOrWhiteSpace(Host))
            {
                prompt.ViewModel.Host = Host;
            }

            await prompt.ShowDialog(MainWindow.Current);
            var result = prompt.ViewModel.Host?.Trim()?.TrimEnd('/');

            if (!Uri.TryCreate(result, UriKind.Absolute, out var serverUri) ||
                (serverUri.Scheme != Uri.UriSchemeHttp && serverUri.Scheme != Uri.UriSchemeHttps))
            {
                Logger.Write("Server URL is not valid.");
                await MessageBox.Show("Server URL must be a valid Uri (e.g. https://app.remotely.one).", "Invalid Server URL", MessageBoxType.OK);
                return;
            }

            Host = result;
            var config = _configService.GetConfig();
            config.Host = Host;
            _configService.Save(config);
        }


        private async Task InstallDependencies()
        {
            try
            {
                var psi = new ProcessStartInfo()
                {
                    FileName = "sudo",
                    Arguments = "bash -c \"apt-get -y install libx11-dev ; " +
                        "apt-get -y install libxrandr-dev ; " +
                        "apt-get -y install libc6-dev ; " +
                        "apt-get -y install libgdiplus ; " +
                        "apt-get -y install libxtst-dev ; " +
                        "apt-get -y install xclip\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                };

                await Task.Run(() => Process.Start(psi).WaitForExit());
            }
            catch
            {
                Logger.Write("Failed to install dependencies.", Shared.Enums.EventType.Error);
            }
          
        }
        private void ScreenCastRequested(object sender, ScreenCastRequest screenCastRequest)
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var result = await MessageBox.Show($"You've received a connection request from {screenCastRequest.RequesterName}.  Accept?", "Connection Request", MessageBoxType.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _ = Task.Run(() =>
                    {
                        Services.GetRequiredService<IScreenCaster>().BeginScreenCasting(screenCastRequest);
                    });
                }
            });
        }
        private async void SessionIDChanged(object sender, string sessionID)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var formattedSessionID = "";
                for (var i = 0; i < sessionID.Length; i += 3)
                {
                    formattedSessionID += sessionID.Substring(i, 3) + " ";
                }
                SessionID = formattedSessionID.Trim();
            });
        }

        private void ViewerAdded(object sender, Viewer viewer)
        {

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Viewers.Add(viewer);
            });
        }

        private async void ViewerRemoved(object sender, string viewerID)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var viewer = Viewers.FirstOrDefault(x => x.ViewerConnectionID == viewerID);
                if (viewer != null)
                {
                    Viewers.Remove(viewer);
                }
            });
        }
    }
}
