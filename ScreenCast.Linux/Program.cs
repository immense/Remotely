using Remotely.ScreenCast.Core;
using Remotely.ScreenCast.Core.Services;
using System;
using System.Threading;
using Remotely.ScreenCast.Linux.Services;
using Remotely.ScreenCast.Linux.Capture;
using Remotely.ScreenCast.Core.Sockets;

namespace Remotely.ScreenCast.Linux
{
    public class Program
    {
        public static Conductor Conductor { get; private set; }
        public static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                var screenCaster = new LinuxScreenCaster();
                var casterSocket = new CasterSocket(new X11Input(), screenCaster, new LinuxAudioCapturer(), new LinuxClipboardService());
                Conductor = new Conductor(casterSocket, screenCaster);

                Conductor.ProcessArgs(args);
                Conductor.Connect().ContinueWith(async (task) =>
                {
                    await Conductor.CasterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName, Conductor.DeviceID);
                    await Conductor.CasterSocket.NotifyRequesterUnattendedReady(Conductor.RequesterID);
                    Conductor.IdleTimer = new IdleTimer(Conductor.Viewers);
                    Conductor.IdleTimer.Start();
                });

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                throw;
            }
        }


        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write((Exception)e.ExceptionObject);
        }
    }
}
