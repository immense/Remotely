using Remotely.ScreenCast.Core;
using Remotely.ScreenCast.Core.Services;
using System;
using System.Threading;
using Remotely.ScreenCast.Linux.Services;

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
                Conductor = new Conductor(
                    new X11Input(), 
                    new LinuxAudioCapturer(), 
                    new LinuxClipboardService(),
                    new LinuxScreenCaster());

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
