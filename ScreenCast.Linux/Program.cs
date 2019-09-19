using Remotely.Shared.Models;
using Remotely.ScreenCast.Core;
using Remotely.ScreenCast.Core.Capture;
using Remotely.ScreenCast.Core.Services;
using Remotely.ScreenCast.Linux.Capture;
using Remotely.ScreenCast.Linux.Input;
using Remotely.ScreenCast.Linux.X11Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

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
                Conductor.Connect().ContinueWith(async (task) =>
                {
                    Conductor.SetMessageHandlers(new X11Input());
                    Conductor.ScreenCastInitiated += ScreenCastInitiated;
                    Conductor.ClipboardTransferred += Conductor_ClipboardTransferred;
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

        private static void Conductor_ClipboardTransferred(object sender, string transferredText)
        {
            var tempPath = Path.GetTempFileName();
            File.WriteAllText(tempPath, transferredText);
            try
            {
                var psi = new ProcessStartInfo("bash", $"-c \"cat {tempPath} | xclip -i -selection clipboard\"")
                {
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                var proc = Process.Start(psi);
                proc.WaitForExit();

            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                File.Delete(tempPath);
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
