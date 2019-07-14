using Remotely.Desktop.Win.Controls;
using Remotely.Desktop.Win.Services;
using Remotely.Shared.Models;
using Remotely.ScreenCast.Core;
using Remotely.ScreenCast.Core.Capture;
using Remotely.ScreenCast.Core.Models;
using Remotely.ScreenCast.Core.Services;
using Remotely.ScreenCast.Win;
using Remotely.ScreenCast.Win.Capture;
using Remotely.ScreenCast.Win.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Principal;
using System.Security.Claims;
using System.Windows.Input;
using System.Windows.Controls;

namespace Remotely.Desktop.Win.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string host;
        private string sessionID;
        public MainWindowViewModel()
        {
            Current = this;
            Conductor = new Conductor();
            Conductor.SessionIDChanged += SessionIDChanged;
            Conductor.ViewerRemoved += ViewerRemoved;
            Conductor.ViewerAdded += ViewerAdded;
            Conductor.ScreenCastRequested += ScreenCastRequested;
            Conductor.AudioToggled += AudioToggled;
            Conductor.ClipboardTransferred += Conductor_ClipboardTransferred;
            CursorIconWatcher = new CursorIconWatcher(Conductor);
            CursorIconWatcher.OnChange += CursorIconWatcher_OnChange;
            AudioCapturer = new AudioCapturer(Conductor);
        }

        public static MainWindowViewModel Current { get; private set; }

        public AudioCapturer AudioCapturer { get; private set; }

        public ICommand ChangeServerCommand
        {
            get
            {
                return new Executor(async (param) =>
                {
                    PromptForHostName();
                    await Init();
                });
            }
        }

        public Conductor Conductor { get; }

        public CursorIconWatcher CursorIconWatcher { get; private set; }

        public string Host
        {
            get => host;
            set
            {
                host = value;
                FirePropertyChanged("Host");
            }
        }

        public bool IsAdministrator => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        public ICommand RemoveViewersCommand
        {
            get
            {
                return new Executor(async (param) =>
                {
                    foreach (Viewer viewer in (param as IList<object>))
                    {
                        viewer.DisconnectRequested = true;
                        await Conductor.CasterSocket.SendViewerRemoved(viewer.ViewerConnectionID);
                    }
                },
                (param) =>
                {
                    return (param as IList<object>)?.Count > 0;
                });
            }

        }

        public ICommand RestartAsAdminCommand
        {
            get
            {
                return new Executor((param) =>
                {
                    try
                    {
                        var psi = new ProcessStartInfo(Assembly.GetExecutingAssembly().Location);
                        psi.Verb = "RunAs";
                        Process.Start(psi);
                        Environment.Exit(0);
                    }
                    // Exception can be thrown if UAC is dialog is cancelled.
                    catch { }
                }, (param) =>
                {
                    return !IsAdministrator;
                });
            }
        }

        public string SessionID
        {
            get => sessionID;
            set
            {
                sessionID = value;
                FirePropertyChanged("SessionID");
            }
        }

        public ObservableCollection<Viewer> Viewers { get; } = new ObservableCollection<Viewer>();

        public void CopyLink()
        {
            Clipboard.SetText($"{Host}/RemoteControl?sessionID={SessionID.Replace(" ", "")}");
        }

        public async Task Init()
        {
            SessionID = "Retrieving...";

            var config = Config.GetConfig();
            Host = config.Host;

            while (string.IsNullOrWhiteSpace(Host))
            {
                Host = "https://";
                PromptForHostName();
            }

            Conductor.ProcessArgs(new string[] { "-mode", "Normal", "-host", Host });
            try
            {
                await Conductor.Connect();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                MessageBox.Show(Application.Current.MainWindow, "Failed to connect to server.", "Connection Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Conductor.SetMessageHandlers(new WinInput());
            await Conductor.CasterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName, Conductor.DeviceID);
            await Conductor.CasterSocket.GetSessionID();
        }

        public void PromptForHostName()
        {
            var prompt = new HostNamePrompt();
            if (!string.IsNullOrWhiteSpace(Host))
            {
                HostNamePromptViewModel.Current.Host = Host;
            }
            prompt.Owner = App.Current?.MainWindow;
            prompt.ShowDialog();
            var result = HostNamePromptViewModel.Current.Host.TrimEnd("/".ToCharArray());
            if (!result.StartsWith("https://") && !result.StartsWith("http://"))
            {
                result = $"https://{result}";
            }
            if (result != Host)
            {
                Host = result;
                var config = Config.GetConfig();
                config.Host = Host;
                config.Save();
            }
        }

        private void AudioToggled(object sender, bool toggleOn)
        {
            if (toggleOn)
            {
                AudioCapturer.Start();
            }
            else
            {
                AudioCapturer.Stop();
            }
        }

        private void Conductor_ClipboardTransferred(object sender, string transferredText)
        {
            Application.Current.Dispatcher.Invoke(() => {
                Clipboard.SetText(transferredText);
            });
        }
        private async void CursorIconWatcher_OnChange(object sender, CursorInfo cursor)
        {
            if (Conductor?.CasterSocket != null)
            {
                await Conductor?.CasterSocket?.SendCursorChange(cursor, Conductor.Viewers.Keys.ToList());
            }
        }
        private void ScreenCastRequested(object sender, ScreenCastRequest screenCastRequest)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var result = MessageBox.Show(Application.Current.MainWindow, $"You've received a connection request from {screenCastRequest.RequesterName}.  Accept?", "Connection Request", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Task.Run(async () =>
                    {
                        ICapturer capturer;
                        try
                        {
                            if (Conductor.Viewers.Count == 0)
                            {
                                capturer = new DXCapture();
                            }
                            else
                            {
                                capturer = new BitBltCapture();
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Write(ex);
                            capturer = new BitBltCapture();
                        }
                        await Conductor.CasterSocket.SendCursorChange(CursorIconWatcher.GetCurrentCursor(), new List<string>() { screenCastRequest.ViewerID });
                        ScreenCaster.BeginScreenCasting(screenCastRequest.ViewerID, screenCastRequest.RequesterName, capturer, Conductor);
                    });
                }
            });
        }
        private void SessionIDChanged(object sender, string sessionID)
        {
            var formattedSessionID = "";
            for (var i = 0; i < sessionID.Length; i += 3)
            {
                formattedSessionID += sessionID.Substring(i, 3) + " ";
            }
            SessionID = formattedSessionID.Trim();
        }

        private void ViewerAdded(object sender, Viewer viewer)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Viewers.Add(viewer);
            });
        }

        private void ViewerRemoved(object sender, string viewerID)
        {
            App.Current.Dispatcher.Invoke(() =>
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
