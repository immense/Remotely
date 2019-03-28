using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Remotely_ScreenCast.Core;
using Remotely_ScreenCast.Core.Capture;
using Remotely_ScreenCast.Core.Enums;
using Remotely_ScreenCast.Core.Models;
using Remotely_ScreenCast.Core.Sockets;
using Remotely_ScreenCast.Core.Utilities;
using Remotely_ScreenCast.Win.Capture;
using Remotely_ScreenCast.Win.Input;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32;

namespace Remotely_ScreenCast.Win
{
	public class Program
	{
        public static Conductor Conductor { get; private set; }
        public static CursorIconWatcher CursorIconWatcher { get; private set; }

        public static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Conductor = new Conductor();
                Conductor.ProcessArgs(args);
                Conductor.Connect().Wait();
                Conductor.SetMessageHandlers(new WinInput());
                Conductor.ScreenCastInitiated += ScreenCastInitiated;
                CursorIconWatcher = new CursorIconWatcher(Conductor);
                CursorIconWatcher.OnChange += CursorIconWatcher_OnChange;
                Conductor.OutgoingMessages.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName).Wait();
                CheckInitialDesktop();
                CheckForRelaunch();
                Conductor.StartWaitForViewerTimer();
                HandleConnection(Conductor).Wait();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private static void CheckForRelaunch()
        {

            if (Conductor.ArgDict.ContainsKey("relaunch"))
            {
                Logger.Write($"Resuming after relaunch in desktop {Conductor.CurrentDesktopName}.");
                var viewersString = Conductor.ArgDict["viewers"];
                var viewerIDs = viewersString.Split(",".ToCharArray());
                Conductor.OutgoingMessages.NotifyViewersRelaunchedScreenCasterReady(viewerIDs).Wait();
            }
            else
            {
                Conductor.OutgoingMessages.NotifyRequesterUnattendedReady(Conductor.RequesterID).Wait();
            }
        }

        private static async void ScreenCastInitiated(object sender, Tuple<string, string> viewerAndRequester)
        {
            ICapturer capturer;
            try
            {
                if (Conductor.Viewers.Count == 0)
                {
                    capturer = new DXCapture();
                    capturer.Init();
                }
                else
                {
                    capturer = new BitBltCapture();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                capturer = new BitBltCapture();
            }
            await Conductor.OutgoingMessages.SendCursorChange(CursorIconWatcher.GetCurrentCursor(), new List<string>() { viewerAndRequester.Item1 });
            ScreenCaster.BeginScreenCasting(viewerAndRequester.Item1, viewerAndRequester.Item2, Conductor.OutgoingMessages, capturer, Conductor);
        }

        public static async void CursorIconWatcher_OnChange(object sender, CursorInfo cursor)
        {
            if (Conductor?.OutgoingMessages != null)
            {
                await Conductor.OutgoingMessages.SendCursorChange(cursor, Conductor.Viewers.Keys.ToList());

            }
        }

        public static async Task HandleConnection(Conductor conductor)
        {
            while (true)
            {
                var desktopName = Win32Interop.GetCurrentDesktop();
                if (desktopName.ToLower() != conductor.CurrentDesktopName.ToLower() && conductor.Viewers.Count > 0)
                {
                    conductor.CurrentDesktopName = desktopName;
                    Logger.Write($"Switching desktops to {desktopName}.");
                    // TODO: SetThradDesktop causes issues with input after switching.
                    //var inputDesktop = Win32Interop.OpenInputDesktop();
                    //User32.SetThreadDesktop(inputDesktop);
                    //User32.CloseDesktop(inputDesktop);
                    conductor.Connection.InvokeAsync("SwitchingDesktops", conductor.Viewers.Keys.ToList()).Wait();
                    var result = Win32Interop.OpenInteractiveProcess(Assembly.GetExecutingAssembly().Location + $" -mode {conductor.Mode.ToString()} -requester {conductor.RequesterID} -serviceid {conductor.ServiceID} -host {conductor.Host} -relaunch true -desktop {desktopName} -viewers {String.Join(",", conductor.Viewers.Keys.ToList())}", desktopName, true, out _);
                    if (!result)
                    {
                        Logger.Write($"Desktop switch to {desktopName} failed.");
                        conductor.OutgoingMessages.SendConnectionFailedToViewers(conductor.Viewers.Keys.ToList()).Wait();
                    }
                }
                await Task.Delay(100);
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
    }
}
