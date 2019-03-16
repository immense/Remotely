using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Remotely_ScreenCast;
using Remotely_ScreenCast.Capture;
using Remotely_ScreenCast.Enums;
using Remotely_ScreenCast.Models;
using Remotely_ScreenCast.Sockets;
using Remotely_ScreenCast.Utilities;
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

namespace Remotely_ScreenCast
{
	public class Program
	{
        public static AppMode Mode { get; private set; }
        public static string RequesterID { get; private set; }
        public static string ServiceID { get; private set; }
        public static string Host { get; private set; }
        public static HubConnection Connection { get; private set; }
        public static OutgoingMessages OutgoingMessages { get; private set; }
        public static string CurrentDesktopName { get; set; }
        public static ConcurrentDictionary<string, Viewer> Viewers { get; } = new ConcurrentDictionary<string, Viewer>();
        public static Dictionary<string,string> ArgDict { get; set; }

        public static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                ProcessArgs(args);
                Connect().Wait();
                SetEventHandlers();
                HandleConnection().Wait();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public static async Task HandleConnection()
        {
            if (Mode == AppMode.Unattended)
            {
                OutgoingMessages.SendServiceID(ServiceID).Wait();

                var desktopName = Win32Interop.GetCurrentDesktop();
                if (desktopName.ToLower() != CurrentDesktopName.ToLower())
                {
                    CurrentDesktopName = desktopName;
                    Logger.Write($"Setting initial desktop to {desktopName}.");
                    ArgDict["desktop"] = desktopName;
                    var openProcessString = Assembly.GetExecutingAssembly().Location;
                    foreach (var arg in ArgDict)
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

                if (ArgDict.ContainsKey("relaunch"))
                {
                    Logger.Write($"Resuming after relaunch in desktop {CurrentDesktopName}.");
                    var viewersString = ArgDict["viewers"];
                    var viewerIDs = viewersString.Split(",".ToCharArray());
                    OutgoingMessages.NotifyViewersRelaunchedScreenCasterReady(viewerIDs).Wait();
                }
                else
                {
                    OutgoingMessages.NotifyRequesterUnattendedReady(RequesterID).Wait();
                }

                StartWaitForViewerTimer();
            }
            else if (Mode == AppMode.Normal)
            {
                OutgoingMessages.GetSessionID().Wait();
            }



            while (true)
            {
                if (Mode == AppMode.Unattended)
                {
                    var desktopName = Win32Interop.GetCurrentDesktop();
                    if (desktopName.ToLower() != CurrentDesktopName.ToLower() && Viewers.Count > 0)
                    {
                        CurrentDesktopName = desktopName;
                        Logger.Write($"Switching desktops to {desktopName}.");
                        // TODO: SetThradDesktop causes issues with input after switching.
                        //var inputDesktop = Win32Interop.OpenInputDesktop();
                        //User32.SetThreadDesktop(inputDesktop);
                        //User32.CloseDesktop(inputDesktop);
                        Connection.InvokeAsync("SwitchingDesktops", Viewers.Keys.ToList()).Wait();
                        var result = Win32Interop.OpenInteractiveProcess(Assembly.GetExecutingAssembly().Location + $" -mode {Mode.ToString()} -requester {RequesterID} -serviceid {ServiceID} -host {Host} -relaunch true -desktop {desktopName} -viewers {String.Join(",", Viewers.Keys.ToList())}", desktopName, true, out _);
                        if (!result)
                        {
                            Logger.Write($"Desktop switch to {desktopName} failed.");
                            OutgoingMessages.SendConnectionFailedToViewers(Viewers.Keys.ToList()).Wait();
                        }
                    }
                }
                await Task.Delay(100);
            }
        }

        public static void SetEventHandlers()
        {
            OutgoingMessages = new OutgoingMessages(Connection);

            MessageHandlers.ApplyConnectionHandlers(Connection, OutgoingMessages);

            CursorIconWatcher.Current.OnChange += CursorIconWatcher_OnChange;
        }

        public static Task Connect()
        {
            Connection = new HubConnectionBuilder()
                .WithUrl($"{Host}/RCDeviceHub")
                .AddMessagePackProtocol()
                .Build();

            return Connection.StartAsync();
        }

 
        private static async void CursorIconWatcher_OnChange(object sender, string cursor)
        {
            await OutgoingMessages.SendCursorChange(cursor, Viewers.Keys.ToList());
        }

        private static void StartWaitForViewerTimer()
        {
            var timer = new System.Timers.Timer(10000);
            timer.AutoReset = false;
            timer.Elapsed += (sender, arg) =>
            {
                // Shut down if no viewers have connected within 10 seconds.
                if (Viewers.Count == 0)
                {
                    Logger.Write("No viewers connected after 10 seconds.  Shutting down.");
                    Environment.Exit(0);
                }
            };
            timer.Start();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write((Exception)e.ExceptionObject);
        }

        public static void ProcessArgs(string[] args)
        {
            ArgDict = new Dictionary<string, string>();

            for (var i = 0; i < args.Length; i += 2)
            {
                var key = args?[i];
                if (key != null)
                {
                    key = key.Trim().Replace("-", "").ToLower();
                    var value = args?[i + 1];
                    if (value != null)
                    {
                        ArgDict[key] = args[i + 1].Trim();
                    }
                }

            }

            Mode = (AppMode)Enum.Parse(typeof(AppMode), Program.ArgDict["mode"]);
            Host = Program.ArgDict["host"];

            if (Mode == AppMode.Unattended)
            {
                RequesterID = Program.ArgDict["requester"];
                CurrentDesktopName = Program.ArgDict["desktop"];
                ServiceID = Program.ArgDict["serviceid"];
            }

        }
        public static EventHandler<string> SessionIDChanged { get; set; }
        public static EventHandler<string> ViewerRemoved { get; set; }
        public static EventHandler<Viewer> ViewerAdded { get; set; }
        public static EventHandler<Tuple<string, string>> ScreenCastRequested { get; set; }
    }
}
