using Remotely.ScreenCast.Core;
using Remotely.ScreenCast.Core.Services;
using System;
using System.Threading;
using Remotely.ScreenCast.Linux.Services;
using Remotely.ScreenCast.Linux.Capture;
using Remotely.ScreenCast.Core.Communication;
using Remotely.ScreenCast.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

                Conductor.ProcessArgs(args);

                if (Conductor.Mode == Core.Enums.AppMode.Chat)
                {
                    Services.GetRequiredService<ChatHostService>().StartChat(Conductor.RequesterID).Wait();
                }
                else
                {
                    Conductor.Connect().ContinueWith(async (task) =>
                    {
                        await Conductor.CasterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName, Conductor.DeviceID);
                        await Conductor.CasterSocket.NotifyRequesterUnattendedReady(Conductor.RequesterID);
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
                builder.AddConsole().AddEventLog();
            });

            serviceCollection.AddSingleton<IScreenCaster, LinuxScreenCaster>();
            serviceCollection.AddSingleton<IKeyboardMouseInput, X11Input>();
            serviceCollection.AddSingleton<IClipboardService, LinuxClipboardService>();
            serviceCollection.AddSingleton<IAudioCapturer, LinuxAudioCapturer>();
            serviceCollection.AddSingleton<CasterSocket>();
            serviceCollection.AddSingleton<IdleTimer>();
            serviceCollection.AddSingleton<Conductor>();
            serviceCollection.AddSingleton<ChatHostService>();
            serviceCollection.AddTransient<ICapturer, X11Capture>();

            ServiceContainer.Instance = serviceCollection.BuildServiceProvider();
        }


        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write((Exception)e.ExceptionObject);
        }
    }
}
