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
using Remotely.ScreenCast.Core.Communication;

namespace Remotely.ScreenCast.Win
{
    public class Program
	{
        public static Conductor Conductor { get; private set; }
        public static CursorIconWatcher CursorIconWatcher { get; private set; }

        private static string CurrentDesktopName { get; set;
        }
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
                var screenCaster = new WinScreenCaster(CursorIconWatcher);
                var clipboardService = new WinClipboardService();
                var casterSocket = new CasterSocket(new WinInput(), screenCaster, new WinAudioCapturer(), clipboardService);
                Conductor = new Conductor(casterSocket, screenCaster);
                Conductor.ProcessArgs(args);

                Conductor.Connect().ContinueWith(async (task) =>
                {
                    await Conductor.CasterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName, Conductor.DeviceID);
                    if (Win32Interop.GetCurrentDesktop(out var currentDesktopName))
                    {
                        Logger.Write($"Setting initial desktop to {currentDesktopName}.");
                        if (Win32Interop.SwitchToInputDesktop())
                        {
                            CurrentDesktopName = currentDesktopName;
                        }
                        else
                        {
                            Logger.Write("Failed to set initial desktop.");
                        }
                    }
                    else
                    {
                        Logger.Write("Failed to get initial desktop name.");
                    }
                    await CheckForRelaunch();
                    Conductor.IdleTimer = new IdleTimer(Conductor.Viewers);
                    Conductor.IdleTimer.Start();
                    CursorIconWatcher.OnChange += CursorIconWatcher_OnChange;
                    clipboardService.BeginWatching();
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
                Logger.Write($"Resuming after relaunch in desktop {CurrentDesktopName}.");
                var viewersString = Conductor.ArgDict["viewers"];
                var viewerIDs = viewersString.Split(",".ToCharArray());
                await Conductor.CasterSocket.NotifyViewersRelaunchedScreenCasterReady(viewerIDs);
            }
            else
            {
                await Conductor.CasterSocket.NotifyRequesterUnattendedReady(Conductor.RequesterID);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write((Exception)e.ExceptionObject);
        }

    }
}
