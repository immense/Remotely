using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Desktop.Core;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Models;
using Remotely.Desktop.Core.Services;
using Remotely.Desktop.Linux.Services;
using Remotely.Desktop.Linux.Views;
using Remotely.Shared.Utilities;
using System;
using System.Linq;
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
                .LogToDebug()
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
                    BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
                });

                while (App.Current is null)
                {
                    Thread.Sleep(100);
                }

                if (Conductor.Mode == Core.Enums.AppMode.Chat)
                {
                    await Services.GetRequiredService<IChatHostService>().StartChat(Conductor.RequesterID, Conductor.OrganizationName);
                }
                else if (Conductor.Mode == Core.Enums.AppMode.Unattended)
                {
                    var casterSocket = Services.GetRequiredService<CasterSocket>();
                    await casterSocket.Connect(Conductor.Host).ContinueWith(async (task) =>
                    {
                        await casterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName, Conductor.DeviceID);
                        await casterSocket.NotifyRequesterUnattendedReady(Conductor.RequesterID);
                        Services.GetRequiredService<IdleTimer>().Start();
                        Services.GetRequiredService<IClipboardService>().BeginWatching();
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
            serviceCollection.AddSingleton<CasterSocket>();
            serviceCollection.AddSingleton<IdleTimer>();
            serviceCollection.AddSingleton<Conductor>();
            serviceCollection.AddSingleton<IChatHostService, ChatHostServiceLinux>();
            serviceCollection.AddTransient<IScreenCapturer, ScreenCapturerLinux>();
            serviceCollection.AddTransient<Viewer>();
            serviceCollection.AddScoped<IFileTransferService, FileTransferService>();
            serviceCollection.AddScoped<IWebRtcSessionFactory, WebRtcSessionFactory>();
            serviceCollection.AddSingleton<ICursorIconWatcher, CursorIconWatcherLinux>();
            serviceCollection.AddSingleton<ISessionIndicator, SessionIndicatorLinux>();
            serviceCollection.AddSingleton<IShutdownService, ShutdownServiceLinux>();
            serviceCollection.AddScoped<IDtoMessageHandler, DtoMessageHandler>();

            ServiceContainer.Instance = serviceCollection.BuildServiceProvider();
        }


        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write((Exception)e.ExceptionObject);
        }
    }
}
