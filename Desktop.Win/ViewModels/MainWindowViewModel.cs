using Remotely.Desktop.Win.Controls;
using Remotely.Desktop.Win.Services;
using Remotely.Shared.Models;
using Remotely.ScreenCast.Core;
using Remotely.ScreenCast.Core.Capture;
using Remotely.ScreenCast.Core.Models;
using Remotely.ScreenCast.Core.Services;
using Remotely.ScreenCast.Win.Capture;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Principal;
using System.Windows.Input;
using Remotely.ScreenCast.Win.Services;
using Remotely.ScreenCast.Core.Interfaces;
using Remotely.ScreenCast.Core.Communication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Win32;
using Screen = System.Windows.Forms.Screen;

namespace Remotely.Desktop.Win.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string host;
        private string sessionID;
        public MainWindowViewModel()
        {
            Current = this;

            BuildServices();

            CursorIconWatcher = Services.GetRequiredService<CursorIconWatcher>();
            CursorIconWatcher.OnChange += CursorIconWatcher_OnChange;
            Services.GetRequiredService<IClipboardService>().BeginWatching();
            Conductor = Services.GetRequiredService<Conductor>();
            Conductor.SessionIDChanged += SessionIDChanged;
            Conductor.ViewerRemoved += ViewerRemoved;
            Conductor.ViewerAdded += ViewerAdded;
            Conductor.ScreenCastRequested += ScreenCastRequested;
        }

        public static MainWindowViewModel Current { get; private set; }
        public static IServiceProvider Services => ServiceContainer.Instance;

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
                    foreach (Viewer viewer in (param as IList<object>).ToArray())
                    {
                        viewer.DisconnectRequested = true;
                        ViewerRemoved(this, viewer.ViewerConnectionID);
                        await Conductor.CasterSocket.SendViewerRemoved(viewer.ViewerConnectionID);
                    }
                },
                (param) =>
                {
                    return (param as IList<object>)?.Count > 0;
                });
            }

        }

        public ICommand ElevateToAdminCommand
        {
            get
            {
                return new Executor((param) =>
                {
                    try
                    {
                        var commandLine = Win32Interop.GetCommandLine().Trim(" -elevate".ToCharArray()).Trim('"');
                        var psi = new ProcessStartInfo(commandLine);
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

        public ICommand ElevateToServiceCommand
        {
            get
            {
                return new Executor((param) =>
                {
                    try
                    {
                        var psi = new ProcessStartInfo("cmd.exe")
                        {
                            WindowStyle = ProcessWindowStyle.Hidden,
                            CreateNoWindow = true
                        };
                        var commandLine = Win32Interop.GetCommandLine().Replace(" -elevate", "").Replace("\"", "");
                        Logger.Write($"Creating temporary service with command line {commandLine}.");
                        psi.Arguments = $"/c sc create Remotely_Temp binPath=\"{commandLine} -elevate\"";
                        Process.Start(psi).WaitForExit();
                        psi.Arguments = "/c sc start Remotely_Temp";
                        Process.Start(psi).WaitForExit();
                        psi.Arguments = "/c sc delete Remotely_Temp";
                        Process.Start(psi).WaitForExit();
                        App.Current.Shutdown();
                    }
                    catch { }
                }, (param) =>
                {
                    return IsAdministrator && !WindowsIdentity.GetCurrent().IsSystem;
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
            Clipboard.SetText($"{Host}/RemoteControl?sessionID={SessionID?.Replace(" ", "")}");
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

            serviceCollection.AddSingleton<CursorIconWatcher>();
            serviceCollection.AddSingleton<IScreenCaster, WinScreenCaster>();
            serviceCollection.AddSingleton<IKeyboardMouseInput, WinInput>();
            serviceCollection.AddSingleton<IClipboardService, WinClipboardService>();
            serviceCollection.AddSingleton<IAudioCapturer, WinAudioCapturer>();
            serviceCollection.AddSingleton<CasterSocket>();
            serviceCollection.AddSingleton<IdleTimer>();
            serviceCollection.AddSingleton<Conductor>();
            serviceCollection.AddTransient<ICapturer>(provider => {
                try
                {
                    var dxCapture = new DXCapture();
                    if (dxCapture.GetScreenCount() == Screen.AllScreens.Length)
                    {
                        return dxCapture;
                    }
                    else
                    {
                        Logger.Write("DX screen count doesn't match.  Using CPU capturer instead.");
                        dxCapture.Dispose();
                        return new BitBltCapture();
                    }
                }
                catch
                {
                    return new BitBltCapture();
                }
            });


            ServiceContainer.Instance = serviceCollection.BuildServiceProvider();
        }
        private async void CursorIconWatcher_OnChange(object sender, CursorInfo cursor)
        {
            if (Conductor?.CasterSocket != null && Conductor?.Viewers?.Count > 0)
            {
                await Conductor?.CasterSocket?.SendCursorChange(cursor, Conductor.Viewers.Keys.ToList());
            }
        }

        private void ScreenCastRequested(object sender, ScreenCastRequest screenCastRequest)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                App.Current.MainWindow.Activate();
                var result = MessageBox.Show(Application.Current.MainWindow, $"You've received a connection request from {screenCastRequest.RequesterName}.  Accept?", "Connection Request", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Task.Run(async () =>
                    {
                        await Conductor.CasterSocket.SendCursorChange(CursorIconWatcher.GetCurrentCursor(), new List<string>() { screenCastRequest.ViewerID });
                        _ = Services.GetRequiredService<IScreenCaster>().BeginScreenCasting(screenCastRequest);
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
                    viewer.Dispose();
                }
            });
        }
    }
}
