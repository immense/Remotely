using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Remotely.Desktop.Core;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Services;
using Remotely.Desktop.Win.Services;
using Remotely.Desktop.Win.Views;
using Remotely.Shared.Helpers;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using Remotely.Shared.Win32;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Remotely.Desktop.Win
{
    public class Program
    {
        public static Form BackgroundForm { get; private set; }
        private static CasterSocket CasterSocket { get; set; }
        private static Conductor Conductor { get; set; }
        private static ICursorIconWatcher CursorIconWatcher { get; set; }
        private static IServiceProvider Services => ServiceContainer.Instance;
        public static async void CursorIconWatcher_OnChange(object sender, CursorInfo cursor)
        {
            if (Conductor?.Viewers?.Count > 0)
            {
                foreach (var viewer in Conductor.Viewers.Values)
                {
                    await viewer.SendCursorChange(cursor);
                }
            }
        }
        [STAThread]
        public static async Task Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                BuildServices();

                Conductor = Services.GetRequiredService<Conductor>();
                CasterSocket = Services.GetRequiredService<CasterSocket>();
                Conductor.ProcessArgs(args);

                SystemEvents.SessionEnding += async (s, e) =>
                {
                    if (e.Reason == SessionEndReasons.SystemShutdown)
                    {
                        await CasterSocket.DisconnectAllViewers();
                    }
                };

                if (Conductor.Mode == Core.Enums.AppMode.Chat)
                {
                    StartUiThreads(null);
                    await Task.Run(async () =>
                    {
                        var chatService = Services.GetRequiredService<IChatHostService>();
                        await chatService.StartChat(Conductor.RequesterID, Conductor.OrganizationName);
                    });
                }
                else if (Conductor.Mode == Core.Enums.AppMode.Unattended)
                {
                    StartUiThreads(null);
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        App.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    });
                    await Task.Run(StartScreenCasting);
                }
                else
                {
                    StartUiThreads(() => new MainWindow());
                }

                TaskHelper.DelayUntil(() => App.Current?.Dispatcher?.HasShutdownStarted != false,
                    TimeSpan.MaxValue, 
                    1000);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                throw;
            }
        }

        private static void BuildServices()
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
            serviceCollection.AddSingleton<CasterSocket>();
            serviceCollection.AddSingleton<IdleTimer>();
            serviceCollection.AddSingleton<Conductor>();
            serviceCollection.AddSingleton<IChatHostService, ChatHostServiceWin>();
            serviceCollection.AddTransient<IScreenCapturer, ScreenCapturerWin>();
            serviceCollection.AddTransient<Viewer>();
            serviceCollection.AddScoped<IWebRtcSessionFactory, WebRtcSessionFactory>();
            serviceCollection.AddScoped<IFileTransferService, FileTransferServiceWin>();
            serviceCollection.AddSingleton<ISessionIndicator, SessionIndicatorWin>();
            serviceCollection.AddSingleton<IShutdownService, ShutdownServiceWin>();
            serviceCollection.AddScoped<IDtoMessageHandler, DtoMessageHandler>();
            serviceCollection.AddScoped<IRemoteControlAccessService, RemoteControlAccessServiceWin>();

            BackgroundForm = new Form()
            {
                Visible = false,
                Opacity = 0,
                ShowIcon = false,
                ShowInTaskbar = false,
                WindowState = FormWindowState.Minimized
            };
            serviceCollection.AddSingleton((serviceProvider) => BackgroundForm);

            ServiceContainer.Instance = serviceCollection.BuildServiceProvider();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write((Exception)e.ExceptionObject);
        }

        private static async Task SendReadyNotificationToViewers()
        {

            if (Conductor.ArgDict.ContainsKey("relaunch"))
            {
                Logger.Write($"Resuming after relaunch.");
                var viewersString = Conductor.ArgDict["viewers"];
                var viewerIDs = viewersString.Split(",".ToCharArray());
                await CasterSocket.NotifyViewersRelaunchedScreenCasterReady(viewerIDs);
            }
            else
            {
                await CasterSocket.NotifyRequesterUnattendedReady(Conductor.RequesterID);
            }
        }

        private static async Task StartScreenCasting()
        {
            CursorIconWatcher = Services.GetRequiredService<ICursorIconWatcher>();

            await CasterSocket.Connect(Conductor.Host);
            await CasterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName, Conductor.DeviceID);

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
        }

        private static void StartUiThreads(Func<Window> createWindowFunc)
        {
            var wpfUiThread = new Thread(() =>
            {
                var app = new App();
                app.InitializeComponent();
                if (createWindowFunc is null)
                {
                    app.Run();
                }
                else
                {
                    app.Run(createWindowFunc());
                }
            });
            wpfUiThread.TrySetApartmentState(ApartmentState.STA);
            wpfUiThread.Start();

            var winformsThread = new Thread(() =>
            {
                System.Windows.Forms.Application.Run(BackgroundForm);
            });
            winformsThread.TrySetApartmentState(ApartmentState.STA);
            winformsThread.Start();


            while (App.Current is null)
            {
                Thread.Sleep(100);
            }
        }
    }
}
