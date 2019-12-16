using Remotely.Shared.Models;
using Remotely.ScreenCast.Core;
using Remotely.ScreenCast.Core.Services;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Remotely.Shared.Win32;
using System.Threading;
using Remotely.ScreenCast.Win.Services;
using Remotely.ScreenCast.Core.Interfaces;
using Remotely.ScreenCast.Win.Capture;

namespace Remotely.ScreenCast.Win
{
    public class Program
	{
        public static Conductor Conductor { get; private set; }
        public static CursorIconWatcher CursorIconWatcher { get; private set; }
        public static async void CursorIconWatcher_OnChange(object sender, CursorInfo cursor)
        {
            if (Conductor?.CasterSocket != null)
            {
                await Conductor.CasterSocket.SendCursorChange(cursor, Conductor.Viewers.Keys.ToList());
            }
        }

        public static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                CursorIconWatcher = new CursorIconWatcher(Conductor);
                Conductor = new Conductor(
                    new WinInput(), 
                    new WinAudioCapturer(), 
                    new WinClipboardService(), 
                    new WinScreenCaster(CursorIconWatcher));
                Conductor.ProcessArgs(args);

                Conductor.Connect().ContinueWith(async (task) =>
                {
                    CursorIconWatcher.OnChange += CursorIconWatcher_OnChange;
                    await Conductor.CasterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName, Conductor.DeviceID);
                    CheckInitialDesktop();
                    await CheckForRelaunch();
                    Conductor.IdleTimer = new IdleTimer(Conductor.Viewers);
                    Conductor.IdleTimer.Start();

                    await HandleConnection(Conductor);
                });

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                throw;
            }
        }

        private static async Task CheckForRelaunch()
        {

            if (Conductor.ArgDict.ContainsKey("relaunch"))
            {
                Logger.Write($"Resuming after relaunch in desktop {Conductor.CurrentDesktopName}.");
                var viewersString = Conductor.ArgDict["viewers"];
                var viewerIDs = viewersString.Split(",".ToCharArray());
                await Conductor.CasterSocket.NotifyViewersRelaunchedScreenCasterReady(viewerIDs);
            }
            else
            {
                await Conductor.CasterSocket.NotifyRequesterUnattendedReady(Conductor.RequesterID);
            }
        }

        private static void CheckInitialDesktop()
        {
            var desktopName = Win32Interop.GetCurrentDesktop();
            if (desktopName.ToLower() != Conductor.CurrentDesktopName.ToLower())
            {
                Conductor.CurrentDesktopName = desktopName;
                Logger.Write($"Setting initial desktop to {desktopName}.");
                Conductor.ArgDict["desktop"] = desktopName;
                var openProcessString = Assembly.GetExecutingAssembly().Location;
                foreach (var arg in Conductor.ArgDict)
                {
                    openProcessString += $" -{arg.Key} {arg.Value}";
                }
                var result = Win32Interop.OpenInteractiveProcess(openProcessString, desktopName, true, out _);
                if (!result)
                {
                    Logger.Write($"Desktop relaunch to {desktopName} failed.");
                }
                Environment.Exit(0);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write((Exception)e.ExceptionObject);
        }


        private static async Task HandleConnection(Conductor conductor)
        {
            while (true)
            {
                var desktopName = Win32Interop.GetCurrentDesktop();
                if (desktopName.ToLower() != conductor.CurrentDesktopName.ToLower() && conductor.Viewers.Count > 0)
                {
                    conductor.CurrentDesktopName = desktopName;
                    Logger.Write($"Switching desktops to {desktopName}.");
                    Win32Interop.SwitchToInputDesktop();
                }
                await Task.Delay(1000);
            }
        }
    }
}
