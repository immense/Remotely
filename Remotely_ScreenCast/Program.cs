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
        public static ICapturer Capturer { get; set; }
        public static CaptureMode CaptureMode { get; set; }
        public static bool DisconnectRequested { get; set; }
        public static string Mode { get; set; }
        public static string RequesterID { get; set; }
        public static string HostName { get; set; }
        public static HubConnection Connection { get; set; }
        private static OutgoingMessages OutgoingMessages { get; set; }

        static void Main(string[] args)
        {
            var argDict = ProcessArgs(args);
            Mode = argDict["mode"];
            RequesterID = argDict["requester"];
            HostName = argDict["hostname"];

            Connection = new HubConnectionBuilder()
                .WithUrl($"http://{HostName}/RCDeviceHub")
                .Build();

            MessageHandlers.ApplyConnectionHandlers(Connection);

            Connection.StartAsync().Wait();

            OutgoingMessages = new OutgoingMessages(Connection);

            try
            {
                Capturer = new DXCapture();
                CaptureMode = CaptureMode.DirectX;
            }
            catch
            {
                Capturer = new BitBltCapture();
                CaptureMode = CaptureMode.BitBtl;
            }


            OutgoingMessages.SendScreenCount(
                Screen.AllScreens.ToList().IndexOf(Screen.PrimaryScreen), 
                Screen.AllScreens.Length, 
                RequesterID).Wait();

            Capturer.ScreenChanged += HandleScreenChanged;

            OutgoingMessages.SendScreenSize(Capturer.CurrentScreenSize.Width, Capturer.CurrentScreenSize.Height).Wait();

            while (!DisconnectRequested)
            {
                try
                {
                    Capturer.Capture();
                    var newImage = ImageDiff.GetImageDiff(Capturer.CurrentFrame, Capturer.PreviousFrame, Capturer.CaptureFullscreen);
                    var img = ImageDiff.EncodeBitmap(newImage);
                    if (Capturer.CaptureFullscreen)
                    {
                        Capturer.CaptureFullscreen = false;
                    }
                    if (img?.Length > 0)
                    {
                        OutgoingMessages.SendScreenCapture(img).Wait();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write($"Outer Error: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }

        private static async void HandleScreenChanged(object sender, Size size)
        {
            await OutgoingMessages.SendScreenSize(size.Width, size.Height);
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
