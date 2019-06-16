using Remotely.Shared.Models;
using Remotely.ScreenCast.Core;
using Remotely.ScreenCast.Core.Capture;
using Remotely.ScreenCast.Core.Utilities;
using Remotely.ScreenCast.Linux.Capture;
using Remotely.ScreenCast.Linux.Input;
using Remotely.ScreenCast.Linux.X11Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

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
                Conductor = new Conductor();
                Conductor.ProcessArgs(args);
                Conductor.Connect().Wait();
                Conductor.SetMessageHandlers(new X11Input());
                Conductor.ScreenCastInitiated += ScreenCastInitiated;
                Conductor.CasterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName).Wait();
                Conductor.CasterSocket.NotifyRequesterUnattendedReady(Conductor.RequesterID).Wait();
                Conductor.StartWaitForViewerTimer();
                while (true)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                throw;
            }
        }

        private static async void ScreenCastInitiated(object sender, ScreenCastRequest screenCastRequest)
        {
            try
            {
                var capturer = new X11Capture();
                await Conductor.CasterSocket.SendCursorChange(new CursorInfo(null, Point.Empty, "default"), new List<string>() { screenCastRequest.ViewerID });
                ScreenCaster.BeginScreenCasting(screenCastRequest.ViewerID, screenCastRequest.RequesterName, capturer, Conductor);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write((Exception)e.ExceptionObject);
        }
    }
}
