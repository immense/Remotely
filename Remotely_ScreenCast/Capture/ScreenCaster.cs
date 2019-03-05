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
using Win32;

namespace Remotely_ScreenCast.Capture
{
    public class ScreenCaster
    {
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

            Logger.Write($"Starting screen cast.  Requester: {requesterName}. Viewer ID: {viewerID}. Capture Mode: {captureMode.ToString()}.  App Mode: {Program.Mode}  Desktop: {Win32Interop.GetCurrentDesktop()}");

            var viewer = new Models.Viewer()
            {
                Capturer = capturer,
                CurrentScreenIndex = capturer.SelectedScreen,
                DisconnectRequested = false,
                Name = requesterName,
                ViewerConnectionID = viewerID,
                HasControl = Program.Mode == Enums.AppMode.Unattended
            };


            var success = false;
            while (!success)
            {
                success = Program.Viewers.TryAdd(viewerID, viewer);
            }

            await outgoingMessages.SendScreenCount(
                   viewer.CurrentScreenIndex,
                   Screen.AllScreens.Length,
                   viewerID);

            await outgoingMessages.SendScreenSize(capturer.CurrentScreenBounds.Width, capturer.CurrentScreenBounds.Height, viewerID);

            capturer.ScreenChanged += async (sender, bounds) =>
            {
                await outgoingMessages.SendScreenSize(bounds.Width, bounds.Height, viewerID);
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
                    Logger.Write(ex);
                }
            }
            Logger.Write($"Ended screen cast.  Requester: {requesterName}. Viewer ID: {viewerID}.");
            success = false;
            while (!success)
            {
                success = Program.Viewers.TryRemove(viewerID, out _);
            }

            // Close if no one is viewing.
            if (Program.Viewers.Count == 0)
            {
                Environment.Exit(0);
            }

        }
        public static Tuple<double, double> GetAbsolutePercentFromRelativePercent(double percentX, double percentY, ICapturer capturer)
        {
            var absoluteX = (capturer.CurrentScreenBounds.Width * percentX) + capturer.CurrentScreenBounds.Left;
            var absoluteY = (capturer.CurrentScreenBounds.Height * percentY) + capturer.CurrentScreenBounds.Top;
            return new Tuple<double, double>(absoluteX / SystemInformation.VirtualScreen.Width, absoluteY / SystemInformation.VirtualScreen.Height);
        }
        public static Tuple<double, double> GetAbsolutePointFromRelativePercent(double percentX, double percentY, ICapturer capturer)
        {
            var absoluteX = (capturer.CurrentScreenBounds.Width * percentX) + capturer.CurrentScreenBounds.Left;
            var absoluteY = (capturer.CurrentScreenBounds.Height * percentY) + capturer.CurrentScreenBounds.Top;
            return new Tuple<double, double>(absoluteX, absoluteY);
        }
    }
}
