using Microsoft.AspNetCore.SignalR.Client;
using Remotely_ScreenCast_Linux;
using Remotely_ScreenCast_Linux.Capture;
using Remotely_ScreenCast_Linux.Models;
using Remotely_ScreenCast_Linux.Sockets;
using Remotely_ScreenCast_Linux.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely_ScreenCast_Linux.Capture
{
    public class ScreenCaster
    {
        public static async void BeginScreenCasting(string viewerID,
                                                   string requesterName,
                                                   OutgoingMessages outgoingMessages)
        {
            ICapturer capturer;
            capturer = new X11Capture();
            capturer.Init();
            Viewer viewer;
            byte[] encodedImageBytes;
            var success = false;
            
            Logger.Write($"Starting screen cast.  Requester: {requesterName}. Viewer ID: {viewerID}.  App Mode: {Program.Mode}");

            viewer = new Viewer()
            {
                Capturer = capturer,
                DisconnectRequested = false,
                Name = requesterName,
                ViewerConnectionID = viewerID,
                HasControl = true,
                ImageQuality = 1
            };

            while (!success)
            {
                success = Program.Viewers.TryAdd(viewerID, viewer);
            }

            if (Program.Mode == Enums.AppMode.Normal)
            {
                Program.ViewerAdded?.Invoke(null, viewer);
            }

            await outgoingMessages.SendScreenCount(
                   capturer.SelectedScreen,
                   //Screen.AllScreens.Length,
                   1,
                   viewerID);

            await outgoingMessages.SendScreenSize(capturer.CurrentScreenBounds.Width, capturer.CurrentScreenBounds.Height, viewerID);

            capturer.ScreenChanged += async (sender, bounds) =>
            {
                await outgoingMessages.SendScreenSize(bounds.Width, bounds.Height, viewerID);
            };

            //await outgoingMessages.SendCursorChange(CursorIconWatcher.Current.GetCurrentCursor(), new List<string>() { viewerID });

            while (!viewer.DisconnectRequested)
            {
                try
                {
                    while (viewer.PendingFrames > 10)
                    {
                        await Task.Delay(1);
                    }

                    capturer.Capture();

                    var diffArea = ImageUtils.GetDiffArea(capturer.CurrentFrame, capturer.PreviousFrame, capturer.CaptureFullscreen);

                    if (diffArea.IsEmpty)
                    {
                        continue;
                    }

                    using (var newImage = capturer.CurrentFrame.Clone(diffArea, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                    {
                        if (capturer.CaptureFullscreen)
                        {
                            capturer.CaptureFullscreen = false;
                        }

                        encodedImageBytes = ImageUtils.EncodeBitmap(newImage);

                        if (encodedImageBytes?.Length > 0)
                        {
                            await outgoingMessages.SendScreenCapture(encodedImageBytes, viewerID, diffArea.Left, diffArea.Top, diffArea.Width, diffArea.Height, DateTime.UtcNow);
                            viewer.PendingFrames++;
                        }
                        GC.Collect();
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

            capturer.Dispose();

            // Close if no one is viewing.
            if (Program.Viewers.Count == 0 && Program.Mode == Enums.AppMode.Unattended)
            {
                Environment.Exit(0);
            }

        }
        public static Tuple<double, double> GetAbsolutePercentFromRelativePercent(double percentX, double percentY, ICapturer capturer)
        {
            var absoluteX = (capturer.CurrentScreenBounds.Width * percentX) + capturer.CurrentScreenBounds.Left;
            var absoluteY = (capturer.CurrentScreenBounds.Height * percentY) + capturer.CurrentScreenBounds.Top;
            return new Tuple<double, double>(800, 600);
            //return new Tuple<double, double>(absoluteX / SystemInformation.VirtualScreen.Width, absoluteY / SystemInformation.VirtualScreen.Height);
        }
        public static Tuple<double, double> GetAbsolutePointFromRelativePercent(double percentX, double percentY, ICapturer capturer)
        {
            var absoluteX = (capturer.CurrentScreenBounds.Width * percentX) + capturer.CurrentScreenBounds.Left;
            var absoluteY = (capturer.CurrentScreenBounds.Height * percentY) + capturer.CurrentScreenBounds.Top;
            return new Tuple<double, double>(absoluteX, absoluteY);
        }
    }
}
