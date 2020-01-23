using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using Remotely.Desktop.Linux.Controls;
using Remotely.Desktop.Linux.Services;
using Remotely.Desktop.Linux.Views;
using Remotely.ScreenCast.Core;
using Remotely.ScreenCast.Core.Capture;
using Remotely.ScreenCast.Core.Interfaces;
using Remotely.ScreenCast.Core.Models;
using Remotely.ScreenCast.Core.Services;
using Remotely.ScreenCast.Core.Communication;
using Remotely.ScreenCast.Linux.Capture;
using Remotely.ScreenCast.Linux.Services;
using Remotely.Shared.Models;
using Remotely.Shared.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Remotely.Desktop.Linux.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private double copyMessageOpacity;
        private string host;
        private bool isCopyMessageVisible;
        private string sessionID;

        public MainWindowViewModel()
        {
            Current = this;
            if (!OSUtils.IsLinux)
            {
                return;
            }

            BuildServices();

            Conductor = Services.GetRequiredService<Conductor>();

            Conductor.SessionIDChanged += SessionIDChanged;
            Conductor.ViewerRemoved += ViewerRemoved;
            Conductor.ViewerAdded += ViewerAdded;
            Conductor.ScreenCastRequested += ScreenCastRequested;
        }


        public static MainWindowViewModel Current { get; private set; }

        public static IServiceProvider Services => ServiceContainer.Instance;

        public ICommand ChangeServerCommand => new Executor(async (param) =>
        {
            await PromptForHostName();
            await Init();
        });

        public ICommand CloseCommand => new Executor((param) =>
        {
            (param as Window)?.Close();
        });

        public Conductor Conductor { get; }

        public ICommand CopyLinkCommand => new Executor(async (param) =>
        {
            await App.Current.Clipboard.SetTextAsync($"{Host}/RemoteControl?sessionID={SessionID.Replace(" ", "")}");

            CopyMessageOpacity = 1;
            IsCopyMessageVisible = true;
            await Task.Delay(1000);
            while (copyMessageOpacity > 0)
            {
                CopyMessageOpacity -= .05;
                await Task.Delay(25);
            }
            IsCopyMessageVisible = false;
        });

        public double CopyMessageOpacity
        {
            get => copyMessageOpacity;
            set => this.RaiseAndSetIfChanged(ref copyMessageOpacity, value);
        }

        public string Host
        {
            get => host;
            set => this.RaiseAndSetIfChanged(ref host, value);
        }

        public bool IsCopyMessageVisible
        {
            get => isCopyMessageVisible;
            set => this.RaiseAndSetIfChanged(ref isCopyMessageVisible, value);
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
                viewer.DisconnectRequested = true;
                await Conductor.CasterSocket.SendViewerRemoved(viewer.ViewerConnectionID);
            }
        });

        public string SessionID
        {
            get => sessionID;
            set => this.RaiseAndSetIfChanged(ref sessionID, value);
        }

        public ObservableCollection<Viewer> Viewers { get; } = new ObservableCollection<Viewer>();

        public async Task Init()
        {
            SessionID = "Retrieving...";
            Host = Config.GetConfig().Host;
            while (string.IsNullOrWhiteSpace(Host))
            {
                Host = "https://";
                await PromptForHostName();
            }
            Host = Host;
            Conductor.ProcessArgs(new string[] { "-mode", "Normal", "-host", Host });
            try
            {
                await Conductor.Connect();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                await MessageBox.Show("Failed to connect to server.", "Connection Failed", MessageBoxType.OK);
                return;
            }

            await Conductor.CasterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName, Conductor.DeviceID);
            await Conductor.CasterSocket.GetSessionID();
        }

        public async Task PromptForHostName()
        {
            var prompt = new HostNamePrompt();
            if (!string.IsNullOrWhiteSpace(Host))
            {
                HostNamePromptViewModel.Current.Host = Host;
            }
            prompt.Owner = MainWindow.Current;
            await prompt.ShowDialog(MainWindow.Current);
            var result = HostNamePromptViewModel.Current.Host;
            if (!result.StartsWith("https://") && !result.StartsWith("http://"))
            {
                result = $"https://{result}";
            }
            if (result != Host)
            {
                Host = result.TrimEnd('/');
                var config = Config.GetConfig();
                config.Host = Host;
                config.Save();
            }
        }

        private static void BuildServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConsole().AddEventLog();
            });

            serviceCollection.AddSingleton<IScreenCaster, LinuxScreenCaster>();
            serviceCollection.AddSingleton<IKeyboardMouseInput, X11Input>();
            serviceCollection.AddSingleton<IClipboardService, LinuxClipboardService>();
            serviceCollection.AddSingleton<IAudioCapturer, LinuxAudioCapturer>();
            serviceCollection.AddSingleton<CasterSocket>();
            serviceCollection.AddSingleton<IdleTimer>();
            serviceCollection.AddSingleton<Conductor>();
            serviceCollection.AddTransient<ICapturer, X11Capture>();


            ServiceContainer.Instance = serviceCollection.BuildServiceProvider();
        }
        private void ScreenCastRequested(object sender, ScreenCastRequest screenCastRequest)
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var result = await MessageBox.Show($"You've received a connection request from {screenCastRequest.RequesterName}.  Accept?", "Connection Request", MessageBoxType.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _ = Task.Run(async () =>
                    {
                        await Conductor.CasterSocket.SendCursorChange(new CursorInfo(null, Point.Empty, "default"), new List<string>() { screenCastRequest.ViewerID });
                            _ = Services.GetRequiredService<IScreenCaster>().BeginScreenCasting(screenCastRequest);
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

        private void ViewerRemoved(object sender, string viewerID)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
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
