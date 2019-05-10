using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using Remotely_Desktop.Unix.Controls;
using Remotely_Desktop.Unix.Services;
using Remotely_ScreenCast.Core;
using Remotely_ScreenCast.Core.Capture;
using Remotely_ScreenCast.Core.Models;
using Remotely_ScreenCast.Core.Utilities;
using Remotely_ScreenCast.Linux.Capture;
using Remotely_ScreenCast.Linux.Input;
using Remotely_Shared.Models;
using Remotely_Shared.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Remotely_Desktop.Unix.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private string sessionID;

        public MainWindowViewModel()
        {
            Current = this;
            Conductor = new Conductor();
            Conductor.SessionIDChanged += SessionIDChanged;
            Conductor.ViewerRemoved += ViewerRemoved;
            Conductor.ViewerAdded += ViewerAdded;
            Conductor.ScreenCastRequested += ScreenCastRequested;
        }


        public static MainWindowViewModel Current { get; private set; }

        public bool AllowHostChange
        {
            get
            {
                return string.IsNullOrWhiteSpace(ForceHost);
            }
        }

        public ICommand ChangeHostCommand => new Executor(async (param) =>
        {
            await PromptForHostName();
            await Init();
        });

        public ICommand CloseCommand => new Executor((param) =>
        {
            (param as Window)?.Close();
        });

        public Conductor Conductor { get; }

        public Config Config { get; private set; }

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

        private double copyMessageOpacity;
        public double CopyMessageOpacity
        {
            get => copyMessageOpacity;
            set => this.RaiseAndSetIfChanged(ref copyMessageOpacity, value);
        }

        public string ForceHost { get; }

        private string host;
        public string Host
        {
            get => host;
            set => this.RaiseAndSetIfChanged(ref host, value);
        }

        private bool isCopyMessageVisible;
        public bool IsCopyMessageVisible
        {
            get => isCopyMessageVisible;
            set => this.RaiseAndSetIfChanged(ref isCopyMessageVisible, value);
        }
        public ICommand MinimizeCommand => new Executor((param) =>
        {
            (param as Window).WindowState = WindowState.Minimized;
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
            Config = Config.GetConfig();
            if (AllowHostChange)
            {
                while (string.IsNullOrWhiteSpace(Config.Host))
                {
                    Config.Host = "https://";
                    await PromptForHostName();
                }
            }
            else
            {
                Config.Host = ForceHost;
            }
            Host = Config.Host;
            Conductor.ProcessArgs(new string[] { "-mode", "Normal", "-host", Config.Host });
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

            if (OSUtils.IsLinux)
            {
                Conductor.SetMessageHandlers(new X11Input());
            }
            
            await Conductor.CasterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName);
            await Conductor.CasterSocket.GetSessionID();
        }

        public async Task PromptForHostName()
        {
            var prompt = new HostNamePrompt();
            if (!string.IsNullOrWhiteSpace(Config.Host))
            {
                HostNamePromptViewModel.Current.Host = Config.Host;
            }
            prompt.Owner = App.Current?.MainWindow;
            await prompt.ShowDialog(App.Current?.MainWindow);
            var result = HostNamePromptViewModel.Current.Host.TrimEnd("/".ToCharArray());
            if (!result.StartsWith("https://") && !result.StartsWith("http://"))
            {
                result = $"https://{result}";
            }
            if (result != Config.Host)
            {
                Config.Host = result;
                Host = result;
                Config.Save();
            }
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
                        ICapturer capturer = null;

                        if (OSUtils.IsLinux)
                        {
                            capturer = new X11Capture();
                        }
                        
                        await Conductor.CasterSocket.SendCursorChange(new CursorInfo(null, Point.Empty, "default"), new List<string>() { screenCastRequest.ViewerID });
                        ScreenCaster.BeginScreenCasting(screenCastRequest.ViewerID, screenCastRequest.RequesterName, capturer, Conductor);
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
