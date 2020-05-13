using Remotely.ScreenCast.Core.Models;
using Remotely.ScreenCast.Core.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remotely.ScreenCast.Core.Interfaces;
using System.Diagnostics;
using System.Drawing.Imaging;
using Microsoft.Extensions.DependencyInjection;
using Remotely.ScreenCast.Core.Utilities;
using Remotely.Shared.Utilities;
using System.Collections.Concurrent;
using Remotely.ScreenCast.Core.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Win32;

namespace Remotely.ScreenCast.Core.Services
{
    public class ScreenCaster : IScreenCaster
    {
        public ScreenCaster(Conductor conductor, 
            ICursorIconWatcher cursorIconWatcher)
        {
            Conductor = conductor;
            CursorIconWatcher = cursorIconWatcher;
        }

        private Conductor Conductor { get; }
        private ICursorIconWatcher CursorIconWatcher { get; }

        public async Task BeginScreenCasting(ScreenCastRequest screenCastRequest)
        {
            var viewers = new ConcurrentDictionary<string, Viewer>();
            var mode = AppMode.Unattended;

            try
            {
                byte[] encodedImageBytes;
                var fpsQueue = new Queue<DateTimeOffset>();
                mode = Conductor.Mode;
                var viewer = ServiceContainer.Instance.GetRequiredService<Viewer>();
                viewer.Name = screenCastRequest.RequesterName;
                viewer.ViewerConnectionID = screenCastRequest.ViewerID;
                viewers = Conductor.Viewers;

                Logger.Write($"Starting screen cast.  Requester: {viewer.Name}. Viewer ID: {viewer.ViewerConnectionID}.  App Mode: {mode}");

                viewers.AddOrUpdate(viewer.ViewerConnectionID, viewer, (id, v) => viewer);

                if (mode == AppMode.Normal)
                {
                    Conductor.InvokeViewerAdded(viewer);
                }

                if (EnvironmentHelper.IsWindows)
                {
                    Win32Interop.SwitchToInputDesktop();
                    await viewer.InitializeWebRtc();
                }

                await viewer.SendMachineName(Environment.MachineName);

                await viewer.SendScreenData(
                       viewer.Capturer.SelectedScreen,
                       viewer.Capturer.GetDisplayNames().ToArray());

                await viewer.SendScreenSize(viewer.Capturer.CurrentScreenBounds.Width,
                    viewer.Capturer.CurrentScreenBounds.Height);

                await viewer.SendCursorChange(CursorIconWatcher.GetCurrentCursor());

                await viewer.SendWindowsSessions();

                viewer.Capturer.ScreenChanged += async (sender, bounds) =>
                {
                    await viewer.SendScreenSize(bounds.Width, bounds.Height);
                };

                await viewer.SendScreenCapture(
                    ImageUtils.EncodeBitmap(viewer.Capturer.CurrentFrame, viewer.EncoderParams),
                    viewer.Capturer.CurrentScreenBounds.Left,
                    viewer.Capturer.CurrentScreenBounds.Top,
                    viewer.Capturer.CurrentScreenBounds.Width,
                    viewer.Capturer.CurrentScreenBounds.Height);

                while (!viewer.DisconnectRequested && viewer.IsConnected)
                {
                    try
                    {
                        if (viewer.IsStalled())
                        {
                            // Viewer isn't responding.  Abort sending.
                            break;
                        }

                        if (EnvironmentHelper.IsDebug)
                        {
                            while (fpsQueue.Any() && DateTimeOffset.Now - fpsQueue.Peek() > TimeSpan.FromSeconds(1))
                            {
                                fpsQueue.Dequeue();
                            }
                            fpsQueue.Enqueue(DateTimeOffset.Now);
                            Debug.WriteLine($"Capture FPS: {fpsQueue.Count}");
                        }

                        await viewer.ThrottleIfNeeded();

                        viewer.Capturer.GetNextFrame();

                        var diffArea = ImageUtils.GetDiffArea(viewer.Capturer.CurrentFrame, viewer.Capturer.PreviousFrame, viewer.Capturer.CaptureFullscreen);

                        if (diffArea.IsEmpty)
                        {
                            continue;
                        }

                        using (var newImage = viewer.Capturer.CurrentFrame.Clone(diffArea, PixelFormat.Format32bppArgb))
                        {
                            if (viewer.Capturer.CaptureFullscreen)
                            {
                                viewer.Capturer.CaptureFullscreen = false;
                            }

                            encodedImageBytes = ImageUtils.EncodeBitmap(newImage, viewer.EncoderParams);

                            if (encodedImageBytes?.Length > 0)
                            {

                                await viewer.SendScreenCapture(encodedImageBytes, diffArea.Left, diffArea.Top, diffArea.Width, diffArea.Height);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex);
                    }
                }

                Logger.Write($"Ended screen cast.  Requester: {viewer.Name}. Viewer ID: {viewer.ViewerConnectionID}.");
                viewers.TryRemove(viewer.ViewerConnectionID, out _);
                viewer.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                // Close if no one is viewing.
                if (viewers.Count == 0 && mode == AppMode.Unattended)
                {
                    Logger.Debug($"Exiting process ID {Process.GetCurrentProcess().Id}.");
                    Environment.Exit(0);
                }
            }
        }
    }
}
