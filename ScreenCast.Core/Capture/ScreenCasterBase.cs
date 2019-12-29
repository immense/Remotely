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
using Remotely.ScreenCast.Core.Interfaces;
using System.Diagnostics;
using System.Threading;

namespace Remotely.ScreenCast.Core.Capture
{
    public class ScreenCasterBase
    {
        public async Task BeginScreenCasting(string viewerID,
                                                   string requesterName,
                                                   ICapturer capturer)
        {
            var conductor = Conductor.Current;
            Logger.Write($"Starting screen cast.  Requester: {requesterName}. Viewer ID: {viewerID}. Capturer: {capturer.GetType().ToString()}.  App Mode: {conductor.Mode}  Desktop: {conductor.CurrentDesktopName}");

            var desktopName = string.Empty;
            byte[] encodedImageBytes;
            var fpsQueue = new Queue<DateTime>();

            var viewer = new Viewer()
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

            if (OSUtils.IsWindows)
            {
                desktopName = Win32Interop.GetCurrentDesktop();
            }


            while (!viewer.DisconnectRequested)
            {
                try
                {
                    if (Conductor.Current.IsDebug)
                    {
                        while (fpsQueue.Any() && DateTime.Now - fpsQueue.Peek() > TimeSpan.FromSeconds(1))
                        {
                            fpsQueue.Dequeue();
                        }
                        fpsQueue.Enqueue(DateTime.Now);
                        Debug.WriteLine("Capture FPS: " + fpsQueue.Count);
                    }

                    if (OSUtils.IsWindows)
                    {
                        var currentDesktopName = Win32Interop.GetCurrentDesktop();
                        if (desktopName.ToLower() != currentDesktopName.ToLower())
                        {
                            desktopName = currentDesktopName;
                            Logger.Write($"Switching to desktop {desktopName} in ScreenCaster.");
                            Win32Interop.SwitchToInputDesktop();
                            continue;
                        }
                    }


                    if (viewer.PendingFrames > 10)
                    {
                        Logger.Write($"Throttling screen capture. Latency: {viewer.Latency}.  Pending Frames: {viewer.PendingFrames}");
                        // This is to prevent dead-lock in case updates are missed from the browser.
                        viewer.PendingFrames = Math.Max(0, viewer.PendingFrames - 1);
                        await Task.Delay(10);
                        continue;
                    }

                    capturer.GetNextFrame();

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
                    GC.Collect();
                }
            }


            Logger.Write($"Ended screen cast.  Requester: {requesterName}. Viewer ID: {viewerID}.");
            conductor.Viewers.TryRemove(viewerID, out _);

            capturer.Dispose();

            // Close if no one is viewing.
            if (conductor.Viewers.Count == 0 && conductor.Mode == Enums.AppMode.Unattended)
            {
                await conductor.CasterSocket.Disconnect();
                Environment.Exit(0);
            }
        }
    }
}
