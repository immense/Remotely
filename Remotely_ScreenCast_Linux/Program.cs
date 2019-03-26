using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Remotely_ScreenCast_Linux;
using Remotely_ScreenCast_Linux.Capture;
using Remotely_ScreenCast_Linux.Enums;
using Remotely_ScreenCast_Linux.Models;
using Remotely_ScreenCast_Linux.Sockets;
using Remotely_ScreenCast_Linux.Utilities;
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

namespace Remotely_ScreenCast_Linux
{
    public class Program
    {
        public static AppMode Mode { get; private set; }
        public static string RequesterID { get; private set; }
        public static string ServiceID { get; private set; }
        public static string Host { get; private set; }
        public static HubConnection Connection { get; private set; }
        public static OutgoingMessages OutgoingMessages { get; private set; }
        public static ConcurrentDictionary<string, Viewer> Viewers { get; } = new ConcurrentDictionary<string, Viewer>();
        public static Dictionary<string, string> ArgDict { get; set; }

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
            OutgoingMessages.SendDeviceInfo(ServiceID, Environment.MachineName).Wait();


            if (Mode == AppMode.Unattended)
            {
                StartWaitForViewerTimer();
            }
            else if (Mode == AppMode.Normal)
            {
                OutgoingMessages.GetSessionID().Wait();
            }



            while (true)
            {
                await Task.Delay(100);
            }
        }

        public static void SetEventHandlers()
        {
            OutgoingMessages = new OutgoingMessages(Connection);

            MessageHandlers.ApplyConnectionHandlers(Connection, OutgoingMessages);

            //CursorIconWatcher.Current.OnChange += CursorIconWatcher_OnChange;
        }

        public static Task Connect()
        {
            Connection = new HubConnectionBuilder()
                .WithUrl($"{Host}/RCDeviceHub")
                .AddMessagePackProtocol()
                .Build();

            return Connection.StartAsync();
        }


        private static async void CursorIconWatcher_OnChange(object sender, CursorInfo cursor)
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

            Mode = (AppMode)Enum.Parse(typeof(AppMode), ArgDict["mode"]);
            Host = ArgDict["host"];

            if (Mode == AppMode.Unattended)
            {
                RequesterID = ArgDict["requester"];
                ServiceID = ArgDict["serviceid"];
            }

        }
        public static EventHandler<string> SessionIDChanged { get; set; }
        public static EventHandler<string> ViewerRemoved { get; set; }
        public static EventHandler<Viewer> ViewerAdded { get; set; }
        public static EventHandler<Tuple<string, string>> ScreenCastRequested { get; set; }
    }
}
