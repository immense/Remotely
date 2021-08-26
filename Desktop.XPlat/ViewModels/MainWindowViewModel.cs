using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Remotely.Desktop.Core;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Services;
using Remotely.Desktop.XPlat.Controls;
using Remotely.Desktop.XPlat.Native.Linux;
using Remotely.Desktop.XPlat.Services;
using Remotely.Desktop.XPlat.Views;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Remotely.Desktop.XPlat.ViewModels
{
    public class MainWindowViewModel : BrandedViewModelBase
    {
        private readonly ICasterSocket _casterSocket;
        private readonly Conductor _conductor;
        private readonly IConfigService _configService;
        private double _copyMessageOpacity;
        private string _host;
        private bool _isCopyMessageVisible;
        private string _sessionId;
        private string _statusMessage;

        public MainWindowViewModel()
        {
            Current = this;

            _configService = Services.GetRequiredService<IConfigService>();
            _conductor = Services.GetRequiredService<Conductor>();
            _casterSocket = Services.GetRequiredService<ICasterSocket>();

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
            await App.Current.Clipboard.SetTextAsync($"{Host}/RemoteControl?sessionID={StatusMessage.Replace(" ", "")}");

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
        public string StatusMessage
        {
            get => _statusMessage;
            set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }

        public ObservableCollection<Viewer> Viewers { get; } = new ObservableCollection<Viewer>();
        private static IServiceProvider Services => ServiceContainer.Instance;

        public async Task GetSessionID()
        {
            await _casterSocket.SendDeviceInfo(_conductor.ServiceID, Environment.MachineName, _conductor.DeviceID);
            var sessionId = await _casterSocket.GetSessionID();

            var formattedSessionID = "";
            for (var i = 0; i < sessionId.Length; i += 3)
            {
                formattedSessionID += sessionId.Substring(i, 3) + " ";
            }

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _sessionId = formattedSessionID.Trim();
                StatusMessage = _sessionId;
            });
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

                StatusMessage = "Initializing...";

                await InstallDependencies();

                StatusMessage = "Retrieving...";

                Host = _configService.GetConfig().Host;

                while (string.IsNullOrWhiteSpace(Host))
                {
                    Host = "https://";
                    await PromptForHostName();
                }

                _conductor.ProcessArgs(new string[] { "-mode", "Normal", "-host", Host });

                var result = await _casterSocket.Connect(_conductor.Host);

                if (result)
                {
                    if (_casterSocket.Connection is null)
                    {
                        return;
                    }

                    _casterSocket.Connection.Closed += async (ex) =>
                    {
                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            StatusMessage = "Disconnected";
                        });
                    };

                    _casterSocket.Connection.Reconnecting += async (ex) =>
                    {
                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            StatusMessage = "Reconnecting";
                        });
                    };

                    _casterSocket.Connection.Reconnected += (id) =>
                    {
                        StatusMessage = _sessionId;
                        return Task.CompletedTask;
                    };

                    await DeviceInitService.GetInitParams();
                    ApplyBranding();

                    await GetSessionID();

                    return;
                }

            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }

            // If we got here, something went wrong.
            _statusMessage = "Failed";
            await MessageBox.Show("Failed to connect to server.", "Connection Failed", MessageBoxType.OK);
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
                    Services.GetRequiredService<IScreenCaster>().BeginScreenCasting(screenCastRequest);
                }
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
