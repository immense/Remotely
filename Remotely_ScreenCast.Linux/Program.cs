using Remotely_ScreenCast.Core;
using Remotely_ScreenCast.Core.Capture;
using Remotely_ScreenCast.Core.Utilities;
using Remotely_ScreenCast.Linux.Capture;
using Remotely_ScreenCast.Linux.Input;
using Remotely_ScreenCast.Linux.X11Interop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Remotely_ScreenCast.Linux
{
    public class Program
    {
        public static Conductor Conductor { get; private set; }
        //public static CursorIconWatcher CursorIconWatcher { get; private set; }

        public static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                // TODO: Works.  Now make it happen.
                //var display = Xlib.XOpenDisplay(null);
                //Console.WriteLine($"Display is {display.ToString()}");
                //var count = Xlib.XScreenCount(display);
                //Console.WriteLine($"Count is {count}");
                //using (var bitmap = new System.Drawing.Bitmap(800, 600))
                //{
                //    using (var graphic = System.Drawing.Graphics.FromImage(bitmap))
                //    {
                //        graphic.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(800, 600));
                //    }
                //    bitmap.Save("Test.jpg");
                //}
                //var width = Xlib.XDisplayWidth(display, 0);
                //var height = Xlib.XDisplayHeight(display, 0);
                //Console.WriteLine($"Width: {width}, Height: {height}");
                //Console.ReadLine();
                Conductor = new Conductor();
                Conductor.ProcessArgs(args);
                Conductor.Connect().Wait();
                Conductor.SetMessageHandlers(new X11Input());
                Conductor.ScreenCastInitiated += ScreenCastInitiated;
                //CursorIconWatcher = new CursorIconWatcher(Conductor);
                //CursorIconWatcher.OnChange += CursorIconWatcher_OnChange;
                Conductor.OutgoingMessages.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName).Wait();
                Conductor.StartWaitForViewerTimer();
                HandleConnection(Conductor).Wait();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private static void ScreenCastInitiated(object sender, Tuple<string, string> viewerAndRequester)
        {
            ICapturer capturer;
            try
            {
                capturer = new X11Capture();
                //await Conductor.OutgoingMessages.SendCursorChange(CursorIconWatcher.GetCurrentCursor(), new List<string>() { viewerAndRequester.Item1 });
                ScreenCaster.BeginScreenCasting(viewerAndRequester.Item1, viewerAndRequester.Item2, Conductor.OutgoingMessages, capturer, Conductor);
                Conductor.OutgoingMessages.SendConnectionFailedToViewers(new List<string>() { viewerAndRequester.Item1 }).Wait();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        //public static async void CursorIconWatcher_OnChange(object sender, CursorInfo cursor)
        //{
        //    await Conductor.OutgoingMessages.SendCursorChange(cursor, Conductor.Viewers.Keys.ToList());
        //}

        public static async Task HandleConnection(Conductor conductor)
        {
                await Task.Delay(100);
            
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write((Exception)e.ExceptionObject);
        }
    }
}
