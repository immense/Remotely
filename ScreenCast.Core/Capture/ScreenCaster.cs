using Microsoft.AspNetCore.SignalR.Client;
using Remotely.ScreenCast.Core.Models;
using Remotely.ScreenCast.Core.Sockets;
using Remotely.ScreenCast.Core.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Remotely.Shared.Services;
using Remotely.Shared.Win32;

namespace Remotely.ScreenCast.Core.Capture
{
    public class ScreenCaster
    {
        public static async void BeginScreenCasting(string viewerID,
                                                   string requesterName,
                                                   ICapturer capturer,
                                                   Conductor conductor)
        {
            Viewer viewer;
            byte[] encodedImageBytes;
           

            Logger.Write($"Starting screen cast.  Requester: {requesterName}. Viewer ID: {viewerID}. Capturer: {capturer.GetType().ToString()}.  App Mode: {conductor.Mode}  Desktop: {conductor.CurrentDesktopName}");

            viewer = new Viewer()
            {
                Capturer = capturer,
                DisconnectRequested = false,
                Name = requesterName,
                ViewerConnectionID = viewerID,
                HasControl = true
            };

            conductor.Viewers.AddOrUpdate(viewerID, viewer, (id, v) => viewer);

            if (conductor.Mode == Enums.AppMode.Normal)
            {
                conductor.InvokeViewerAdded(viewer);
            }

            await conductor.CasterSocket.SendMachineName(Environment.MachineName, viewerID);

            await conductor.CasterSocket.SendScreenCount(
                   capturer.SelectedScreen,
                   capturer.GetScreenCount(),
                   viewerID);

            await conductor.CasterSocket.SendScreenSize(capturer.CurrentScreenBounds.Width, capturer.CurrentScreenBounds.Height, viewerID);

            capturer.ScreenChanged += async (sender, bounds) =>
            {
                await conductor.CasterSocket.SendScreenSize(bounds.Width, bounds.Height, viewerID);
            };

            var desktopName = string.Empty;
            if (OSUtils.IsWindows)
            {
                desktopName = Win32Interop.GetCurrentDesktop();
            }

            while (!viewer.DisconnectRequested)
            {
                try
                {
                    var currentDesktopName = Win32Interop.GetCurrentDesktop();
                    if (desktopName.ToLower() != currentDesktopName.ToLower())
                    {
                        desktopName = currentDesktopName;
                        Logger.Write($"Switching to desktop {desktopName} in ScreenCaster.");
                        Win32Interop.SwitchToInputDesktop();
                        continue;
                    }

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
                        
                        encodedImageBytes = ImageUtils.EncodeBitmap(newImage, viewer.EncoderParams);

                        if (encodedImageBytes?.Length > 0)
                        {
                            await conductor.CasterSocket.SendScreenCapture(encodedImageBytes, viewerID, diffArea.Left, diffArea.Top, diffArea.Width, diffArea.Height, DateTime.UtcNow);
                            viewer.PendingFrames++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
                finally
                {
                    // TODO: Even after disposing of the bitmap, GC doesn't collect in time.  Memory usage soars quickly.
                    // Need to revisit this later.
                    GC.Collect();
                }
            }
            Logger.Write($"Ended screen cast.  Requester: {requesterName}. Viewer ID: {viewerID}.");
            conductor.Viewers.TryRemove(viewerID, out _);

            capturer.Dispose();

            // Close if no one is viewing.
            if (conductor.Viewers.Count == 0 && conductor.Mode == Enums.AppMode.Unattended)
            {
                Environment.Exit(0);
            }

        }
    }
}
