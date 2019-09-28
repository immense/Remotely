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
using Microsoft.Win32;

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

        public static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Conductor = new Conductor();
                Conductor.ProcessArgs(args);

                Conductor.ScreenCastInitiated += ScreenCastInitiated;
                Conductor.AudioToggled += AudioToggled;
                Conductor.ClipboardTransferred += Conductor_ClipboardTransferred;

                Conductor.Connect().ContinueWith(async (task) =>
                {
                    Conductor.SetMessageHandlers(new WinInput());
                    AudioCapturer = new AudioCapturer(Conductor);
                    CursorIconWatcher = new CursorIconWatcher(Conductor);
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

        private static void Conductor_ClipboardTransferred(object sender, string transferredText)
        {
            var thread = new Thread(() =>
            {
                Clipboard.SetText(transferredText);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
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
