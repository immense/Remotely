using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Remotely.Shared.Models;
using Remotely.ScreenCast.Core;
using Remotely.ScreenCast.Core.Capture;
using Remotely.ScreenCast.Core.Enums;
using Remotely.ScreenCast.Core.Models;
using Remotely.ScreenCast.Core.Sockets;
using Remotely.ScreenCast.Core.Services;
using Remotely.ScreenCast.Win.Capture;
using Remotely.ScreenCast.Win.Input;
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
using Remotely.Shared.Win32;
using NAudio.Wave;
using Remotely.Shared.Services;
using System.Runtime.InteropServices;
using System.Threading;

namespace Remotely.ScreenCast.Win
{
	public class Program
	{
        public static AudioCapturer AudioCapturer { get; private set; }
        public static Conductor Conductor { get; private set; }
        public static CursorIconWatcher CursorIconWatcher { get; private set; }
        public static async void CursorIconWatcher_OnChange(object sender, CursorInfo cursor)
        {
            if (Conductor?.CasterSocket != null)
            {
                await Conductor.CasterSocket.SendCursorChange(cursor, Conductor.Viewers.Keys.ToList());
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
                    conductor.IsSwitchingDesktops = true;
                    // TODO: SetThreadDesktop causes issues with input after switching.
                    //var inputDesktop = Win32Interop.OpenInputDesktop();
                    //User32.SetThreadDesktop(inputDesktop);
                    //User32.CloseDesktop(inputDesktop);
                    conductor.Connection.InvokeAsync("SwitchingDesktops", conductor.Viewers.Keys.ToList()).Wait();
                    var result = Win32Interop.OpenInteractiveProcess(Assembly.GetExecutingAssembly().Location + $" -mode {conductor.Mode.ToString()} -requester {conductor.RequesterID} -serviceid {conductor.ServiceID} -deviceid {conductor.DeviceID} -host {conductor.Host} -relaunch true -desktop {desktopName} -viewers {String.Join(",", conductor.Viewers.Keys.ToList())}", desktopName, true, out _);
                    if (!result)
                    {
                        Logger.Write($"Desktop switch to {desktopName} failed.");
                        conductor.CasterSocket.SendConnectionFailedToViewers(conductor.Viewers.Keys.ToList()).Wait();
                    }
                }
                await Task.Delay(100);
            }
        }

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
                Conductor.AudioToggled += AudioToggled;
                Conductor.ClipboardTransferred += Conductor_ClipboardTransferred;
                AudioCapturer = new AudioCapturer(Conductor);
                CursorIconWatcher = new CursorIconWatcher(Conductor);
                CursorIconWatcher.OnChange += CursorIconWatcher_OnChange;
                Conductor.CasterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName, Conductor.DeviceID).Wait();
                CheckInitialDesktop();
                CheckForRelaunch();
                Conductor.IdleTimer = new IdleTimer(Conductor.Viewers);
                Conductor.IdleTimer.Start();
                HandleConnection(Conductor).Wait();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                throw;
            }
        }

        private static void AudioToggled(object sender, bool toggledOn)
        {
            if (toggledOn)
            {
                AudioCapturer.Start();
            }
            else
            {
                AudioCapturer.Stop();
            }
        }

        private static void CheckForRelaunch()
        {

            if (Conductor.ArgDict.ContainsKey("relaunch"))
            {
                Logger.Write($"Resuming after relaunch in desktop {Conductor.CurrentDesktopName}.");
                var viewersString = Conductor.ArgDict["viewers"];
                var viewerIDs = viewersString.Split(",".ToCharArray());
                Conductor.CasterSocket.NotifyViewersRelaunchedScreenCasterReady(viewerIDs).Wait();
            }
            else
            {
                Conductor.CasterSocket.NotifyRequesterUnattendedReady(Conductor.RequesterID).Wait();
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

        private static void Conductor_ClipboardTransferred(object sender, string transferredText)
        {
            var thread = new Thread(() => {
                Clipboard.SetText(transferredText);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write((Exception)e.ExceptionObject);
        }

        private static async void ScreenCastInitiated(object sender, ScreenCastRequest screenCastRequest)
        {
            ICapturer capturer;
            try
            {
                if (Conductor.Viewers.Count == 0)
                {
                    capturer = new DXCapture();
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
            await Conductor.CasterSocket.SendCursorChange(CursorIconWatcher.GetCurrentCursor(), new List<string>() { screenCastRequest.ViewerID });
            ScreenCaster.BeginScreenCasting(screenCastRequest.ViewerID, screenCastRequest.RequesterName, capturer, Conductor);
        }
    }
}
