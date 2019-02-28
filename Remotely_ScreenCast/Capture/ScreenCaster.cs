using Microsoft.AspNetCore.SignalR.Client;
using Remotely_ScreenCast.Sockets;
using Remotely_ScreenCast.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remotely_ScreenCast.Capture
{
    public class ScreenCaster
    {
        private static object CaptureLock { get; } = new object();
        public static async void BeginScreenCasting(HubConnection hubConnection,
                                                   string viewerID,
                                                   string requesterName,
                                                   OutgoingMessages outgoingMessages)
        {
            ICapturer capturer;
            CaptureMode captureMode;

            try
            {
                if (Program.Viewers.Count == 0)
                {
                    capturer = new DXCapture();
                    captureMode = CaptureMode.DirectX;
                }
                else
                {
                    capturer = new BitBltCapture();
                    captureMode = CaptureMode.BitBtl;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                capturer = new BitBltCapture();
                captureMode = CaptureMode.BitBtl;
            }

            Logger.Write($"Starting screen cast.  Requester: {requesterName}. Viewer ID: {viewerID}. Capture Mode: {captureMode.ToString()}.");

            var viewer = new Models.Viewer()
            {
                Capturer = capturer,
                CurrentScreenIndex = 0,
                DisconnectRequested = false,
                Name = requesterName,
                ViewerConnectionID = viewerID,
                HasControl = Program.Mode == "unattended"
            };


            var success = false;
            while (!success)
            {
                success = Program.Viewers.TryAdd(viewerID, viewer);
            }

            await outgoingMessages.SendScreenCount(
                   Screen.AllScreens.ToList().IndexOf(Screen.PrimaryScreen),
                   Screen.AllScreens.Length,
                   viewerID);

            await outgoingMessages.SendScreenSize(capturer.CurrentScreenBounds.Width, capturer.CurrentScreenBounds.Height, viewerID);

            capturer.ScreenChanged += async (sender, size) =>
            {
                await outgoingMessages.SendScreenSize(size.Width, size.Height, viewerID);
            };

            while (!viewer.DisconnectRequested)
            {
                try
                {
                    if (viewer.NextCaptureDelay > 0)
                    {
                        await Task.Delay(viewer.NextCaptureDelay);
                        viewer.NextCaptureDelay = 0;
                    }

                    capturer.Capture();

                    var newImage = ImageDiff.GetImageDiff(capturer.CurrentFrame, capturer.PreviousFrame, capturer.CaptureFullscreen);
                    var img = ImageDiff.EncodeBitmap(newImage);
                    if (capturer.CaptureFullscreen)
                    {
                        capturer.CaptureFullscreen = false;
                    }
                    if (img?.Length > 0)
                    {
                        await outgoingMessages.SendScreenCapture(img, viewerID, DateTime.Now);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write($"Outer Error: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }

            success = false;
            while (!success)
            {
                Program.Viewers.TryRemove(viewerID, out _);
            }
            Logger.Write($"Ended screen cast.  Requester: {requesterName}. Viewer ID: {viewerID}.");
        }
        public static Tuple<double, double> GetAbsoluteScreenCoordinatesFromPercentages(double percentX, double percentY, ICapturer capturer)
        {
            var absoluteX = (capturer.CurrentScreenBounds.Width * percentX) + capturer.CurrentScreenBounds.Left;
            var absoluteY = (capturer.CurrentScreenBounds.Height * percentY) + capturer.CurrentScreenBounds.Top;
            return new Tuple<double, double>(absoluteX / SystemInformation.VirtualScreen.Width, absoluteY / SystemInformation.VirtualScreen.Height);
        }
    }
}
