using Microsoft.AspNetCore.SignalR.Client;
using Remotely_ScreenCast.Models;
using Remotely_ScreenCast.Sockets;
using Remotely_ScreenCast.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32;

namespace Remotely_ScreenCast.Capture
{
    public class ScreenCaster
    {
        public static async void BeginScreenCasting(string viewerID,
                                                   string requesterName,
                                                   OutgoingMessages outgoingMessages)
        {
            ICapturer capturer;
            CaptureMode captureMode;
            Viewer viewer;
            var success = false;

            try
            {
                if (Program.CurrentDesktopName.ToLower() == "winlogon")
                {
                    capturer = new BitBltCapture();
                    captureMode = CaptureMode.BitBtl;
                }
                else if (Program.Viewers.Count == 0)
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

            Logger.Write($"Starting screen cast.  Requester: {requesterName}. Viewer ID: {viewerID}. Capture Mode: {captureMode.ToString()}.  App Mode: {Program.Mode}  Desktop: {Program.CurrentDesktopName}");

            viewer = new Viewer()
            {
                Capturer = capturer,
                DisconnectRequested = false,
                Name = requesterName,
                ViewerConnectionID = viewerID,
                HasControl = Program.Mode == Enums.AppMode.Unattended,
                ImageQuality = 1
            };

            while (!success)
            {
                success = Program.Viewers.TryAdd(viewerID, viewer);
            }

            await outgoingMessages.SendScreenCount(
                   capturer.SelectedScreen,
                   Screen.AllScreens.Length,
                   viewerID);

            await outgoingMessages.SendScreenSize(capturer.CurrentScreenBounds.Width, capturer.CurrentScreenBounds.Height, viewerID);

            capturer.ScreenChanged += async (sender, bounds) =>
            {
                await outgoingMessages.SendScreenSize(bounds.Width, bounds.Height, viewerID);
            };

            await outgoingMessages.SendCursorChange(CursorIconWatcher.Current.GetCurrentCursor(), new List<string>() { viewerID });

            // TODO: SetThradDesktop causes issues with input after switching.
            //var desktopName = Win32Interop.GetCurrentDesktop();
            while (!viewer.DisconnectRequested)
            {
                try
                {
                    // TODO: SetThradDesktop causes issues with input after switching.
                    //var currentDesktopName = Win32Interop.GetCurrentDesktop();
                    //if (desktopName.ToLower() != currentDesktopName.ToLower())
                    //{
                    //    desktopName = currentDesktopName;
                    //    Logger.Write($"Switching to desktop {desktopName} in ScreenCaster.");
                    //    var inputDesktop = Win32Interop.OpenInputDesktop();
                    //    User32.SetThreadDesktop(inputDesktop);
                    //    User32.CloseDesktop(inputDesktop);
                    //    continue;
                    //}

                    while (viewer.PendingFrames > 10)
                    {
                        Logger.Write("Waiting on pending frames.");
                        await Task.Delay(1);
                    }

                    capturer.Capture();

                    var diffArea = ImageUtils.GetDiffArea(capturer.CurrentFrame, capturer.PreviousFrame, capturer.CaptureFullscreen);

                    var newImage = capturer.CurrentFrame.Clone(diffArea, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    if (capturer.CaptureFullscreen)
                    {
                        capturer.CaptureFullscreen = false;
                    }

                    long newQuality;
                    if (viewer.PendingFrames < 5)
                    {
                        newQuality = (long)Math.Min(1, viewer.ImageQuality + .1);
                    }
                    else
                    {
                        newQuality = (long)Math.Max(.1, viewer.ImageQuality - .1);
                    }
                    Logger.Write($"New quality: {newQuality}");
                    if (newQuality != viewer.ImageQuality)
                    {
                        viewer.ImageQuality = newQuality;
                        viewer.FullScreenRefreshNeeded = true;
                    }
                    else
                    {
                        capturer.CaptureFullscreen = true;
                        viewer.FullScreenRefreshNeeded = false;
                    }

                    var img = ImageUtils.EncodeBitmap(newImage, viewer.EncoderParams);

                    if (img?.Length > 0)
                    {
                        await outgoingMessages.SendScreenCapture(img, viewerID, diffArea.Left, diffArea.Top, diffArea.Width, diffArea.Height, DateTime.UtcNow);
                        viewer.PendingFrames++;
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
