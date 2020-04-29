using Remotely.ScreenCast.Core;
using Remotely.ScreenCast.Core.Services;
using System;
using System.Threading;
using Remotely.ScreenCast.Linux.Services;
using Remotely.ScreenCast.Core.Communication;
using Remotely.ScreenCast.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using Remotely.ScreenCast.Core.Models;
using Remotely.Shared.Utilities;

namespace Remotely.ScreenCast.Linux
{
    public class Program
    {
        public static Conductor Conductor { get; private set; }
        public static IServiceProvider Services => ServiceContainer.Instance;

        public static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                BuildServices();

                Conductor = Services.GetRequiredService<Conductor>();

                Logger.Write("Processing Args: " + string.Join(", ", Environment.GetCommandLineArgs()));
                Conductor.ProcessArgs(Environment.GetCommandLineArgs().SkipWhile(x => !x.StartsWith("-")).ToArray());

                if (Conductor.Mode == Core.Enums.AppMode.Chat)
                {
                    Services.GetRequiredService<ChatHostService>().StartChat(Conductor.RequesterID, Conductor.OrganizationName).Wait();
                }
                else
                {
                    var casterSocket = Services.GetRequiredService<CasterSocket>();
                    casterSocket.Connect(Conductor.Host).ContinueWith(async (task) =>
                    {
                        await casterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName, Conductor.DeviceID);
                        await casterSocket.NotifyRequesterUnattendedReady(Conductor.RequesterID);
                        Services.GetRequiredService<IdleTimer>().Start();
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
            serviceCollection.AddSingleton<ChatHostService>();
            serviceCollection.AddTransient<IScreenCapturer, ScreenCapturerLinux>();
            serviceCollection.AddTransient<Viewer>();
            serviceCollection.AddScoped<IFileTransferService, FileTransferService>();
            serviceCollection.AddScoped<IWebRtcSessionFactory, WebRtcSessionFactory>();
            serviceCollection.AddSingleton<ICursorIconWatcher, CursorIconWatcherLinux>();

            ServiceContainer.Instance = serviceCollection.BuildServiceProvider();
        }


        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write((Exception)e.ExceptionObject);
        }
    }
}
