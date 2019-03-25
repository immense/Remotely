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
            byte[] encodedImageBytes;
            var success = false;
            
            try
            {
                if (Program.Viewers.Count == 0)
                {
                    capturer = new DXCapture();
                    captureMode = CaptureMode.DirectX;
                    capturer.Init();
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

                        //long newQuality;
                        //if (viewer.PendingFrames < 8)
                        //{
                        //    newQuality = Math.Min(1, viewer.ImageQuality + .1);
                        //}
                        //else
                        //{
                        //    newQuality = Math.Max(0, viewer.ImageQuality - .1);
                        //}

                        //if (newQuality != viewer.ImageQuality)
                        //{
                        //    Logger.Write($"New quality: {newQuality}");
                        //    viewer.ImageQuality = newQuality;
                        //    viewer.FullScreenRefreshNeeded = true;
                        //}
                        //else if (viewer.FullScreenRefreshNeeded)
                        //{
                        //    Logger.Write($"Quality stabilized.");
                        //    capturer.CaptureFullscreen = true;
                        //    viewer.FullScreenRefreshNeeded = false;
                        //}

                        encodedImageBytes = ImageUtils.EncodeBitmap(newImage);

                        if (encodedImageBytes?.Length > 0)
                        {
                            await outgoingMessages.SendScreenCapture(encodedImageBytes, viewerID, diffArea.Left, diffArea.Top, diffArea.Width, diffArea.Height, DateTime.UtcNow);
                            viewer.PendingFrames++;
                        }
                        // TODO: Even after disposing of the bitmap, GC doesn't collect in time.  Memory usage soars quickly.
                        // Need to revisit this later.
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
