using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Desktop.Core;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Services;
using Remotely.Desktop.Linux.Services;
using Remotely.Desktop.Linux.Views;
using Remotely.Shared.Services;
using Remotely.Shared.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Desktop.Linux
{
    class Program
    {

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();

        public static Conductor Conductor { get; private set; }
        public static IServiceProvider Services => ServiceContainer.Instance;

        public static async Task Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                BuildServices();

                Conductor = Services.GetRequiredService<Conductor>();

                Logger.Write("Processing Args: " + string.Join(", ", Environment.GetCommandLineArgs()));
                Conductor.ProcessArgs(args);

                _ = Task.Run(() =>
                {
                    BuildAvaloniaApp().Start(AppMain, args);
                });

                while (App.Current is null)
                {
                    await Task.Delay(100);
                }

                await Services.GetRequiredService<IDeviceInitService>().GetInitParams();

                if (Conductor.Mode == Core.Enums.AppMode.Chat)
                {
                    await Services.GetRequiredService<IChatClientService>().StartChat(Conductor.RequesterID, Conductor.OrganizationName);
                }
                else if (Conductor.Mode == Core.Enums.AppMode.Unattended)
                {
                    var casterSocket = Services.GetRequiredService<ICasterSocket>();
                    await casterSocket.Connect(Conductor.Host).ContinueWith(async (task) =>
                    {
                        await casterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName, Conductor.DeviceID);
                        await casterSocket.NotifyRequesterUnattendedReady(Conductor.RequesterID);
                        Services.GetRequiredService<IdleTimer>().Start();
                        Services.GetRequiredService<IClipboardService>().BeginWatching();
                        Services.GetRequiredService<IKeyboardMouseInput>().Init();
                    });
                }
                else
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        App.Current.RunWithMainWindow<MainWindow>();
                    });
                }

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                throw;
            }
        }

        private static void AppMain(Application app, string[] args)
        {
            var cts = new CancellationTokenSource();
            app.Run(cts.Token);
        }

        private static void BuildServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConsole().AddDebug();
            });

            serviceCollection.AddSingleton<IScreenCaster, ScreenCaster>();
            serviceCollection.AddSingleton<IKeyboardMouseInput, KeyboardMouseInputLinux>();
            serviceCollection.AddSingleton<IClipboardService, ClipboardServiceLinux>();
            serviceCollection.AddSingleton<IAudioCapturer, AudioCapturerLinux>();
            serviceCollection.AddSingleton<ICasterSocket, CasterSocket>();
            serviceCollection.AddSingleton<IdleTimer>();
            serviceCollection.AddSingleton<Conductor>();
            serviceCollection.AddSingleton<IChatClientService, ChatHostService>();
            serviceCollection.AddSingleton<IChatUiService, ChatUiServiceLinux>();
            serviceCollection.AddTransient<IScreenCapturer, ScreenCapturerLinux>();
            serviceCollection.AddTransient<Viewer>();
            serviceCollection.AddScoped<IFileTransferService, FileTransferServiceLinux>();
            serviceCollection.AddScoped<IWebRtcSessionFactory, WebRtcSessionFactory>();
            serviceCollection.AddSingleton<ICursorIconWatcher, CursorIconWatcherLinux>();
            serviceCollection.AddSingleton<ISessionIndicator, SessionIndicatorLinux>();
            serviceCollection.AddSingleton<IShutdownService, ShutdownServiceLinux>();
            serviceCollection.AddScoped<IDtoMessageHandler, DtoMessageHandler>();
            serviceCollection.AddScoped<IRemoteControlAccessService, RemoteControlAccessServiceLinux>();
            serviceCollection.AddScoped<IConfigService, ConfigServiceLinux>();
            serviceCollection.AddScoped<IDeviceInitService, DeviceInitService>();

            ServiceContainer.Instance = serviceCollection.BuildServiceProvider();
        }


        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write((Exception)e.ExceptionObject);
        }
    }
}
