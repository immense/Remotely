using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using Remotely.Desktop.Linux.Controls;
using Remotely.Desktop.Linux.Services;
using Remotely.Desktop.Linux.Views;
using Remotely.Desktop.Core;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Models;
using Remotely.Desktop.Core.Services;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Utilities;

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
            if (!EnvironmentHelper.IsLinux)
            {
                return;
            }

            Conductor = Services.GetRequiredService<Conductor>();
            CasterSocket = Services.GetRequiredService<CasterSocket>();

            Conductor.SessionIDChanged += SessionIDChanged;
            Conductor.ViewerRemoved += ViewerRemoved;
            Conductor.ViewerAdded += ViewerAdded;
            Conductor.ScreenCastRequested += ScreenCastRequested;
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
                await CasterSocket.DisconnectViewer(viewer, true);
            }
        });

        public string SessionID
        {
            get => sessionID;
            set => this.RaiseAndSetIfChanged(ref sessionID, value);
        }

        public ObservableCollection<Viewer> Viewers { get; } = new ObservableCollection<Viewer>();
        private static IServiceProvider Services => ServiceContainer.Instance;
        private CasterSocket CasterSocket { get; }
        private Conductor Conductor { get; }
        public async Task GetSessionID()
        {
            await CasterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName, Conductor.DeviceID);
            await CasterSocket.GetSessionID();
        }

        public async Task Init()
        {
            try
            {

                SessionID = "Retrieving...";

                await CheckDependencies();


                Host = Config.GetConfig().Host;

                while (string.IsNullOrWhiteSpace(Host))
                {
                    Host = "https://";
                    await PromptForHostName();
                }
                Conductor.ProcessArgs(new string[] { "-mode", "Normal", "-host", Host });

                await CasterSocket.Connect(Conductor.Host);

                CasterSocket.Connection.Closed += async (ex) =>
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        SessionID = "Disconnected";
                    });
                };

                CasterSocket.Connection.Reconnecting += async (ex) =>
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        SessionID = "Reconnecting";
                    });
                };

                CasterSocket.Connection.Reconnected += async (arg) =>
                {
                    await GetSessionID();
                };


                await CasterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName, Conductor.DeviceID);
                await CasterSocket.GetSessionID();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                sessionID = "Failed";
                await MessageBox.Show("Failed to connect to server.", "Connection Failed", MessageBoxType.OK);
                return;
            }
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

            if (result is null)
            {
                return;
            }

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


        private async Task CheckDependencies()
        {
            var dependencies = new string[]
            {
                "libx11-dev",
                "libc6-dev",
                "libgdiplus",
                "libxtst-dev",
                "xclip"
            };

            foreach (var dependency in dependencies)
            {
                var proc = Process.Start("dpkg", $"-s {dependency}");
                proc.WaitForExit();
                if (proc.ExitCode != 0)
                {
                    var commands = "sudo apt-get -y install libx11-dev ; " +
                                "sudo apt-get -y install libc6-dev ; " +
                                "sudo apt-get -y install libgdiplus ; " +
                                "sudo apt-get -y install libxtst-dev ; " +
                                "sudo apt-get -y install xclip";

                    await App.Current.Clipboard.SetTextAsync(commands);

                    var message = "The following dependencies are required.  Install commands have been copied to your clipboard." +
                        Environment.NewLine + Environment.NewLine +
                        "Please paste them into a terminal and run, then try opening Remotely again." +
                        Environment.NewLine + Environment.NewLine +
                        "libx11-dev" + Environment.NewLine +
                        "libc6-dev" + Environment.NewLine +
                        "libgdiplus" + Environment.NewLine + 
                        "libxtst-dev" + Environment.NewLine + 
                        "xclip";

                    await MessageBox.Show(message, "Dependencies Required", MessageBoxType.OK);

                    Environment.Exit(0);
                }
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
