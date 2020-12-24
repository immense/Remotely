using Microsoft.Extensions.DependencyInjection;
using Remotely.Desktop.Core.Enums;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Utilities;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Desktop.Core.Services
{
    public class ScreenCaster : IScreenCaster
    {
        private readonly Conductor _conductor;
        private readonly ICursorIconWatcher _cursorIconWatcher;
        private readonly ISessionIndicator _sessionIndicator;
        private readonly IShutdownService _shutdownService;

        public ScreenCaster(Conductor conductor,
            ICursorIconWatcher cursorIconWatcher,
            ISessionIndicator sessionIndicator,
            IShutdownService shutdownService)
        {
            _conductor = conductor;
            _cursorIconWatcher = cursorIconWatcher;
            _sessionIndicator = sessionIndicator;
            _shutdownService = shutdownService;
        }


        public async Task BeginScreenCasting(ScreenCastRequest screenCastRequest)
        {
        
            try
            {
                var sendFramesLock = new SemaphoreSlim(1, 1);
                Bitmap currentFrame = null;
                Bitmap previousFrame = null;
                var fpsQueue = new Queue<DateTimeOffset>();

                var viewer = ServiceContainer.Instance.GetRequiredService<Viewer>();
                viewer.Name = screenCastRequest.RequesterName;
                viewer.ViewerConnectionID = screenCastRequest.ViewerID;

                Logger.Write($"Starting screen cast.  Requester: {viewer.Name}. " +
                    $"Viewer ID: {viewer.ViewerConnectionID}.  App Mode: {_conductor.Mode}");

                _conductor.Viewers.AddOrUpdate(viewer.ViewerConnectionID, viewer, (id, v) => viewer);

                if (_conductor.Mode == AppMode.Normal)
                {
                    _conductor.InvokeViewerAdded(viewer);
                }

                if (_conductor.Mode == AppMode.Unattended && screenCastRequest.NotifyUser)
                {
                    _sessionIndicator.Show();
                }

                await viewer.SendViewerConnected();

                await viewer.SendMachineName(Environment.MachineName);

                await viewer.SendScreenData(
                       viewer.Capturer.SelectedScreen,
                       viewer.Capturer.GetDisplayNames().ToArray());

                await viewer.SendScreenSize(viewer.Capturer.CurrentScreenBounds.Width,
                    viewer.Capturer.CurrentScreenBounds.Height);

                await viewer.SendCursorChange(_cursorIconWatcher.GetCurrentCursor());

                await viewer.SendWindowsSessions();

                viewer.Capturer.ScreenChanged += async (sender, bounds) =>
                {
                    await viewer.SendScreenSize(bounds.Width, bounds.Height);
                };

                using (var initialFrame = viewer.Capturer.GetNextFrame())
                {
                    if (initialFrame != null)
                    {
                        await viewer.SendScreenCapture(
                         ImageUtils.EncodeBitmap(initialFrame, viewer.EncoderParams),
                         viewer.Capturer.CurrentScreenBounds.Left,
                         viewer.Capturer.CurrentScreenBounds.Top,
                         viewer.Capturer.CurrentScreenBounds.Width,
                         viewer.Capturer.CurrentScreenBounds.Height);
                    }
                }

                if (EnvironmentHelper.IsWindows)
                {
                    await viewer.InitializeWebRtc();
                }

                while (!viewer.DisconnectRequested && viewer.IsConnected)
                {
                    try
                    {
                        if (viewer.IsUsingWebRtcVideo)
                        {
                            Thread.Sleep(100);
                            continue;
                        }
                        if (viewer.IsStalled)
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

                        viewer.ThrottleIfNeeded();

                        if (currentFrame != null)
                        {
                            previousFrame?.Dispose();
                            previousFrame = (Bitmap)currentFrame.Clone();
                        }

                        currentFrame?.Dispose();
                        currentFrame = viewer.Capturer.GetNextFrame();

                        var diffAreas = ImageUtils.GetDiffAreas(currentFrame, previousFrame, viewer.Capturer.CaptureFullscreen);

                        if (diffAreas.Count == 0)
                        {
                            continue;
                        }

                        await sendFramesLock.WaitAsync();
                        SendFrames((Bitmap)currentFrame.Clone(), diffAreas, viewer, sendFramesLock);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex);
                    }
                }

                Logger.Write($"Ended screen cast.  Requester: {viewer.Name}. Viewer ID: {viewer.ViewerConnectionID}.");
                _conductor.Viewers.TryRemove(viewer.ViewerConnectionID, out _);
                viewer.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                // Close if no one is viewing.
                if (_conductor.Viewers.IsEmpty && _conductor.Mode == AppMode.Unattended)
                {
                    Logger.Write("No more viewers.  Calling shutdown service.");
                    await _shutdownService.Shutdown();
                }
            }
        }

        private void SendFrames(Bitmap currentFrame, List<Rectangle> diffAreas, Viewer viewer, SemaphoreSlim sendFramesLock)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    foreach (var diffArea in diffAreas)
                    {
                        using var newImage = currentFrame.Clone(diffArea, PixelFormat.Format32bppArgb);

                        if (viewer.Capturer.CaptureFullscreen)
                        {
                            viewer.Capturer.CaptureFullscreen = false;
                        }

                        var encodedImageBytes = ImageUtils.EncodeBitmap(newImage, viewer.EncoderParams);

                        if (encodedImageBytes?.Length > 0)
                        {
                            await viewer.SendScreenCapture(encodedImageBytes, diffArea.Left, diffArea.Top, diffArea.Width, diffArea.Height);
                        }
                    }
                }
                finally
                {
                    sendFramesLock.Release();
                    currentFrame.Dispose();
                }
            });
        }
    }
}
