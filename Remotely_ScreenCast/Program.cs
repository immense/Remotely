using Microsoft.AspNetCore.SignalR.Client;
using Remotely_ScreenCapture;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Win32;

namespace Remotely_ScreenCast
{
	class Program
	{
        static Stopwatch stopwatch;
        static CaptureMode captureMode;
        static void Main(string[] args)
        {
            var argDict = ProcessArgs(args);
            var mode = argDict["mode"];
            var requesterID = argDict["requester"];
            var hostname = argDict["hostname"];

            var hubConnection = new HubConnectionBuilder()
                .WithUrl($"https://{argDict["hostname"]}/DeviceHub")
                .Build();

            hubConnection.Closed += (ex) =>
            {
                Logger.Write($"Error: {ex.Message}");
                return Task.CompletedTask;
            };

            hubConnection.StartAsync().Wait();

            var screenshotPath = $@"{Path.GetTempPath()}Screens\";
            if (!Directory.Exists(screenshotPath))
            {
                Directory.CreateDirectory(screenshotPath);
            }


            ICapturer capturer;
            try
            {
                capturer = new DXCapture();
                captureMode = CaptureMode.DirectX;
            }
            catch
            {
                capturer = new BitBltCapture();
                captureMode = CaptureMode.BitBtl;
            }

            while (true)
            {
                try
                {
                    capturer.Capture();
                    var newImage = ImageDiff.GetImageDiff(capturer.CurrentFrame, capturer.PreviousFrame, capturer.CaptureFullscreen);
                    var img = ImageDiff.EncodeBitmap(newImage);
                    if (capturer.CaptureFullscreen)
                    {
                        capturer.CaptureFullscreen = false;
                    }
                    if (img?.Length > 0)
                    {
                        File.WriteAllBytes($@"{screenshotPath}{Path.GetRandomFileName()}.png", img);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write($"Outer Error: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
                System.Threading.Thread.Sleep(3000);
            }

        }

        enum CaptureMode
        {
            BitBtl,
            DirectX
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
