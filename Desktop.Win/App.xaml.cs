using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Remotely.Desktop.Core;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Services;
using Remotely.Desktop.Win.Services;
using Remotely.Desktop.Win.Views;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using Remotely.Shared.Win32;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using Form = System.Windows.Forms.Form;

namespace Remotely.Desktop.Win
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Form BackgroundForm { get; private set; }
        private ICasterSocket _casterSocket { get; set; }
        private Conductor _conductor { get; set; }
        private ICursorIconWatcher CursorIconWatcher { get; set; }
        private IServiceProvider Services => ServiceContainer.Instance;

        public async void CursorIconWatcher_OnChange(object sender, CursorInfo cursor)
        {
            if (_conductor?.Viewers?.Count > 0)
            {
                foreach (var viewer in _conductor.Viewers.Values)
                {
                    await viewer.SendCursorChange(cursor);
                }
            }
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Write(e.Exception);
            MessageBox.Show("There was an unhandled exception.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (Environment.GetCommandLineArgs().Contains("-elevate"))
            {
                var commandLine = Win32Interop.GetCommandLine().Replace(" -elevate", "");

                Logger.Write($"Elevating process {commandLine}.");
                var result = Win32Interop.OpenInteractiveProcess(
                    commandLine,
                    -1,
                    false,
                    "default",
                    true,
                    out var procInfo);
                Logger.Write($"Elevate result: {result}. Process ID: {procInfo.dwProcessId}.");
                Environment.Exit(0);
            }

            _ = Task.Run(Initialize);
        }

        private void BuildServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConsole().AddDebug().AddEventLog();
            });

            serviceCollection.AddSingleton<ICursorIconWatcher, CursorIconWatcherWin>();
            serviceCollection.AddSingleton<IScreenCaster, ScreenCaster>();
            serviceCollection.AddSingleton<IKeyboardMouseInput, KeyboardMouseInputWin>();
            serviceCollection.AddSingleton<IClipboardService, ClipboardServiceWin>();
            serviceCollection.AddSingleton<IAudioCapturer, AudioCapturerWin>();
            serviceCollection.AddSingleton<ICasterSocket, CasterSocket>();
            serviceCollection.AddSingleton<IdleTimer>();
            serviceCollection.AddSingleton<Conductor>();
            serviceCollection.AddSingleton<IChatClientService, ChatHostService>();
            serviceCollection.AddSingleton<IChatUiService, ChatUiServiceWin>();
            serviceCollection.AddTransient<IScreenCapturer, ScreenCapturerWin>();
            serviceCollection.AddTransient<Viewer>();
            serviceCollection.AddScoped<IWebRtcSessionFactory, WebRtcSessionFactory>();
            serviceCollection.AddScoped<IFileTransferService, FileTransferServiceWin>();
            serviceCollection.AddSingleton<ISessionIndicator, SessionIndicatorWin>();
            serviceCollection.AddSingleton<IShutdownService, ShutdownServiceWin>();
            serviceCollection.AddScoped<IDtoMessageHandler, DtoMessageHandler>();
            serviceCollection.AddScoped<IRemoteControlAccessService, RemoteControlAccessServiceWin>();
            serviceCollection.AddScoped<IConfigService, ConfigServiceWin>();
            serviceCollection.AddScoped<IDeviceInitService, DeviceInitService>();
            serviceCollection.AddScoped<IClickOnceService, ClickOnceService>();

            BackgroundForm = new Form()
            {
                Visible = false,
                Opacity = 0,
                ShowIcon = false,
                ShowInTaskbar = false,
                WindowState = System.Windows.Forms.FormWindowState.Minimized
            };
            serviceCollection.AddSingleton((serviceProvider) => BackgroundForm);

            ServiceContainer.Instance = serviceCollection.BuildServiceProvider();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write((Exception)e.ExceptionObject);
        }

        private async Task Initialize()
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                var args = Environment.GetCommandLineArgs()
                    .SkipWhile(x => !x.StartsWith("-"))
                    .ToArray();

                BuildServices();

                _conductor = Services.GetRequiredService<Conductor>();
                _casterSocket = Services.GetRequiredService<ICasterSocket>();
                _conductor.ProcessArgs(args);

                SystemEvents.SessionEnding += async (s, e) =>
                {
                    if (e.Reason == SessionEndReasons.SystemShutdown)
                    {
                        await _casterSocket.DisconnectAllViewers();
                    }
                };


                await Services.GetRequiredService<IClickOnceService>()
                    .TrySetBrandingFromActivationUri()
                    .ConfigureAwait(false);

                var deviceInitService = Services.GetRequiredService<IDeviceInitService>();
                await deviceInitService.GetInitParams().ConfigureAwait(false);

                StartWinFormsThread();

                if (_conductor.Mode == Core.Enums.AppMode.Chat)
                {
                    var chatService = Services.GetRequiredService<IChatClientService>();
                    await chatService.StartChat(_conductor.RequesterID, _conductor.OrganizationName).ConfigureAwait(false);
                }
                else if (_conductor.Mode == Core.Enums.AppMode.Unattended)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    });
                    await StartScreenCasting().ConfigureAwait(false);
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        MainWindow = new MainWindow();
                        MainWindow.Show();
                    });
                }

                WaitForAppExit();

            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                throw;
            }
        }

        private async Task SendReadyNotificationToViewers()
        {

            if (_conductor.ArgDict.ContainsKey("relaunch"))
            {
                Logger.Write($"Resuming after relaunch.");
                var viewersString = _conductor.ArgDict["viewers"];
                var viewerIDs = viewersString.Split(",".ToCharArray());
                await _casterSocket.NotifyViewersRelaunchedScreenCasterReady(viewerIDs);
            }
            else
            {
                await _casterSocket.NotifyRequesterUnattendedReady(_conductor.RequesterID);
            }
        }

        private async Task StartScreenCasting()
        {

            CursorIconWatcher = Services.GetRequiredService<ICursorIconWatcher>();

            await _casterSocket.Connect(_conductor.Host);
            await _casterSocket.SendDeviceInfo(_conductor.ServiceID, Environment.MachineName, _conductor.DeviceID);

            if (Win32Interop.GetCurrentDesktop(out var currentDesktopName))
            {
                Logger.Write($"Setting initial desktop to {currentDesktopName}.");
            }
            else
            {
                Logger.Write("Failed to get initial desktop name.");
            }

            if (!Win32Interop.SwitchToInputDesktop())
            {
                Logger.Write("Failed to set initial desktop.");
            }

            await SendReadyNotificationToViewers();
            Services.GetRequiredService<IdleTimer>().Start();
            CursorIconWatcher.OnChange += CursorIconWatcher_OnChange;
            Services.GetRequiredService<IClipboardService>().BeginWatching();
            Services.GetRequiredService<IKeyboardMouseInput>().Init();
        }

        private void StartWinFormsThread()
        {
            var winformsThread = new Thread(() =>
            {
                System.Windows.Forms.Application.Run(BackgroundForm);
            })
            {
                IsBackground = true
            };
            winformsThread.TrySetApartmentState(ApartmentState.STA);
            winformsThread.Start();

            Logger.Write("Background WinForms thread started.");
        }
        private void WaitForAppExit()
        {
            var appExitEvent = new ManualResetEventSlim();
            Dispatcher.Invoke(() =>
            {
                Exit += (s, a) =>
                {
                    appExitEvent.Set();
                };
            });
            appExitEvent.Wait();
        }
    }
}
