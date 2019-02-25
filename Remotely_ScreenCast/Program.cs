using Microsoft.AspNetCore.SignalR.Client;
using Remotely_ScreenCapture;
using Remotely_ScreenCapture.Capture;
using Remotely_ScreenCapture.Sockets;
using Remotely_ScreenCapture.Utilities;
using Remotely_ScreenCast.Capture;
using System;
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
        public static bool IsCapturing { get; set; }
        public static object LockObject { get; } = new object();
        public static string Mode { get; private set; }
        public static string RequesterID { get; private set; }
        public static string ViewerID { get; set; }
        public static string HostName { get; private set; }
        public static HubConnection Connection { get; private set; }
        public static OutgoingMessages OutgoingMessages { get; private set; }

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var argDict = ProcessArgs(args);
            Mode = argDict["mode"];
            RequesterID = argDict["requester"];
            HostName = argDict["hostname"];

            Connection = new HubConnectionBuilder()
                .WithUrl($"http://{HostName}/RCDeviceHub")
                .Build();

            Connection.StartAsync().Wait();
           
            OutgoingMessages = new OutgoingMessages(Connection);

            try
            {
                Capturer = new DXCapture();
                CaptureMode = CaptureMode.DirectX;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                Capturer = new BitBltCapture();
                CaptureMode = CaptureMode.BitBtl;
            }

            Capturer.ScreenChanged += HandleScreenChanged;

            MessageHandlers.ApplyConnectionHandlers(Connection, OutgoingMessages, Capturer);

            OutgoingMessages.NotifyRequesterUnattendedReady(RequesterID).Wait();

            Console.ReadKey();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write((Exception)e.ExceptionObject);
        }

        private static async void HandleScreenChanged(object sender, Size size)
        {
            if (!string.IsNullOrWhiteSpace(ViewerID))
            {
                await OutgoingMessages.SendScreenSize(size.Width, size.Height, ViewerID);
            }
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
