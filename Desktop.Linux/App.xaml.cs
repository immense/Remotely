using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Desktop.Core;
using Remotely.Desktop.Core.Interfaces;
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
    public class App : Application
    {
        private static Conductor Conductor;
        private static IServiceProvider Services => ServiceContainer.Instance;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            //if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            //{
            //    desktop.MainWindow = new MainWindow
            //    {
            //        DataContext = new MainWindowViewModel(),
            //    };
            //}

            base.OnFrameworkInitializationCompleted();

            _ = Task.Run(Startup);
        }

        private void BuildServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConsole().AddDebug();
            });

            serviceCollection.AddSingleton<IScreenCaster, ScreenCaster>();
            serviceCollection.AddSingleton<ICasterSocket, CasterSocket>();
            serviceCollection.AddSingleton<IdleTimer>();
            serviceCollection.AddSingleton<Conductor>();
            serviceCollection.AddSingleton<IChatClientService, ChatHostService>();
            serviceCollection.AddTransient<Viewer>();
            serviceCollection.AddScoped<IWebRtcSessionFactory, WebRtcSessionFactory>();
            serviceCollection.AddScoped<IDtoMessageHandler, DtoMessageHandler>();
            serviceCollection.AddScoped<IDeviceInitService, DeviceInitService>();

            switch (EnvironmentHelper.Platform)
            {
                case Shared.Enums.Platform.Linux:
                    {
                        serviceCollection.AddSingleton<IKeyboardMouseInput, KeyboardMouseInputLinux>();
                        serviceCollection.AddSingleton<IClipboardService, ClipboardServiceLinux>();
                        serviceCollection.AddSingleton<IAudioCapturer, AudioCapturerLinux>();
                        serviceCollection.AddSingleton<IChatUiService, ChatUiServiceLinux>();
                        serviceCollection.AddTransient<IScreenCapturer, ScreenCapturerLinux>();
                        serviceCollection.AddScoped<IFileTransferService, FileTransferServiceLinux>();
                        serviceCollection.AddSingleton<ICursorIconWatcher, CursorIconWatcherLinux>();
                        serviceCollection.AddSingleton<ISessionIndicator, SessionIndicatorLinux>();
                        serviceCollection.AddSingleton<IShutdownService, ShutdownServiceLinux>();
                        serviceCollection.AddScoped<IRemoteControlAccessService, RemoteControlAccessServiceLinux>();
                        serviceCollection.AddScoped<IConfigService, ConfigServiceLinux>();
                    }
                    break;
                case Shared.Enums.Platform.OSX:
                    {

                    }
                    break;
                default:
                    throw new PlatformNotSupportedException();
            }

            ServiceContainer.Instance = serviceCollection.BuildServiceProvider();
        }


        private async Task Startup()
        {

            BuildServices();

            Conductor = Services.GetRequiredService<Conductor>();

            var args = Environment.GetCommandLineArgs().SkipWhile(x => !x.StartsWith("-"));
            Logger.Write("Processing Args: " + string.Join(", ", args));
            Conductor.ProcessArgs(args.ToArray());

            await Services.GetRequiredService<IDeviceInitService>().GetInitParams();

            if (Conductor.Mode == Core.Enums.AppMode.Chat)
            {
                await Services.GetRequiredService<IChatClientService>().StartChat(Conductor.RequesterID, Conductor.OrganizationName);
            }
            else if (Conductor.Mode == Core.Enums.AppMode.Unattended)
            {
                var casterSocket = Services.GetRequiredService<ICasterSocket>();
                await casterSocket.Connect(Conductor.Host).ConfigureAwait(false);
                await casterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName, Conductor.DeviceID).ConfigureAwait(false);
                await casterSocket.NotifyRequesterUnattendedReady(Conductor.RequesterID).ConfigureAwait(false);
                Services.GetRequiredService<IdleTimer>().Start();
                Services.GetRequiredService<IClipboardService>().BeginWatching();
                Services.GetRequiredService<IKeyboardMouseInput>().Init();
            }
            else
            {
                await Dispatcher.UIThread.InvokeAsync(() => {
                    this.RunWithMainWindow<MainWindow>();
                });
            }
        }


    }
}
