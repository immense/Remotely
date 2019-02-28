using Microsoft.AspNetCore.SignalR.Client;
using Remotely_ScreenCast;
using Remotely_ScreenCast.Capture;
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
        public static ICapturer Capturer { get; private set; }
        public static CaptureMode CaptureMode { get; private set; }
        public static bool DisconnectRequested { get; set; }
        public static string Mode { get; private set; }
        public static string RequesterID { get; private set; }
        public static string ServiceID { get; private set; }
        public static string Host { get; private set; }
        public static HubConnection Connection { get; private set; }
        public static OutgoingMessages OutgoingMessages { get; private set; }
        public static ConcurrentDictionary<string, Viewer> Viewers { get; } = new ConcurrentDictionary<string, Viewer>();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var argDict = ProcessArgs(args);
            Mode = argDict["mode"];
            RequesterID = argDict["requester"];
            Host = argDict["host"];

            Connection = new HubConnectionBuilder()
                .WithUrl($"{Host}/RCDeviceHub")
                .Build();

            Connection.StartAsync().Wait();

            OutgoingMessages = new OutgoingMessages(Connection);

            MessageHandlers.ApplyConnectionHandlers(Connection, OutgoingMessages);

            OutgoingMessages.NotifyRequesterUnattendedReady(RequesterID).Wait();

            StartWaitForViewerTimer();

            Console.Read();
        }

        private static void StartWaitForViewerTimer()
        {
            var timer = new System.Timers.Timer(5000);
            timer.AutoReset = false;
            timer.Elapsed += (sender, arg) =>
            {
                // Shut down if no viewers have connected within 5 seconds.
                if (Viewers.Count == 0)
                {
                    Logger.Write("No viewers connected after 5 seconds.  Shutting down.");
                    Environment.Exit(0);
                }
            };
            timer.Start();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write((Exception)e.ExceptionObject);
        }

        private static Dictionary<string, string> ProcessArgs(string[] args)
        {
            var argDict = new Dictionary<string, string>();

            for (var i = 0; i < args.Length; i += 2)
            {
                var key = args?[i];
                if (key != null)
                {
                    key = key.Trim().Replace("-", "").ToLower();
                    var value = args?[i + 1];
                    if (value != null)
                    {
                        argDict[key] = args[i + 1].Trim();
                    }
                }

            }
            return argDict;
        }

    
    }
}
