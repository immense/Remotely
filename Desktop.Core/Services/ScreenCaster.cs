using Microsoft.Extensions.DependencyInjection;
using Remotely.Desktop.Core.Enums;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Models;
using Remotely.Desktop.Core.Utilities;
using Remotely.Shared.Utilities;
using Remotely.Shared.Models;
using System;
using System.Collections.Concurrent;
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
        private readonly int _maxQuality = 75;
        private readonly int _minQuality = 10;
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
                var refreshTimer = Stopwatch.StartNew();
                var refreshNeeded = false;
                var currentQuality = _maxQuality;
                Bitmap currentFrame = null;
                Bitmap previousFrame = null;
                var sw = Stopwatch.StartNew();

                var viewer = ServiceContainer.Instance.GetRequiredService<Viewer>();
                viewer.Name = screenCastRequest.RequesterName;
                viewer.ViewerConnectionID = screenCastRequest.ViewerID;

                var screenBounds = viewer.Capturer.CurrentScreenBounds;

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

                await viewer.SendScreenSize(screenBounds.Width, screenBounds.Height);

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
                        await viewer.SendScreenCapture(new CaptureFrame()
                        {
                            EncodedImageBytes = ImageUtils.EncodeJpeg(initialFrame, _maxQuality),
                            Left = screenBounds.Left,
                            Top = screenBounds.Top,
                            Width = screenBounds.Width,
                            Height = screenBounds.Height
                        });
                    }
                }


                if (EnvironmentHelper.IsWindows && screenCastRequest.UseWebRtc)
                {
                    await viewer.InitializeWebRtc();
                }

                // Wait until the first image is received.
                TaskHelper.DelayUntil(() => !viewer.PendingSentFrames.Any(), TimeSpan.MaxValue);

                while (!viewer.DisconnectRequested && viewer.IsConnected)
                {
                    try
                    {
                        TaskHelper.DelayUntil(() => sw.Elapsed.TotalMilliseconds > 40, TimeSpan.FromSeconds(5));
                        sw.Restart();

                        if (viewer.IsUsingWebRtcVideo)
                        {
                            Thread.Sleep(100);
                            continue;
                        }
                        if (viewer.IsStalled)
                        {
                            // Viewer isn't responding.  Abort sending.
                            Logger.Write("Viewer stalled.  Ending send loop.");
                            break;
                        }

                        viewer.ThrottleIfNeeded();

                        if (currentFrame != null)
                        {
                            previousFrame?.Dispose();
                            previousFrame = (Bitmap)currentFrame.Clone();
                        }

                        currentFrame?.Dispose();
                        currentFrame = viewer.Capturer.GetNextFrame();

                        if (currentFrame is null)
                        {
                            continue;
                        }

                        if (refreshNeeded && refreshTimer.Elapsed.TotalSeconds > 5)
                        {
                            viewer.Capturer.CaptureFullscreen = true;
                        }


                        var diffArea = ImageUtils.GetDiffArea(currentFrame, previousFrame, viewer.Capturer.CaptureFullscreen);

                        if (diffArea.IsEmpty)
                        {
                            continue;
                        }

                        
                        if (viewer.Capturer.CaptureFullscreen)
                        {
                            refreshTimer.Restart();
                            refreshNeeded = false;
                        }

                        byte[] encodedImageBytes;
                        if (viewer.Capturer.CaptureFullscreen)
                        {
                            // Recalculate Bps.
                            viewer.AverageBytesPerSecond = 0;
                            encodedImageBytes = ImageUtils.EncodeJpeg(currentFrame, _maxQuality);
                        }
                        else
                        {
                            if (!viewer.AutoQuality)
                            {
                                currentQuality = _maxQuality;
                            }
                            else if (viewer.AverageBytesPerSecond > 0)
                            {
                                var expectedSize = diffArea.Height * diffArea.Width * 4 * .1;
                                var timeToSend = expectedSize / viewer.AverageBytesPerSecond;
                                currentQuality = Math.Max(_minQuality, Math.Min(_maxQuality, (int)(.1 / timeToSend * _maxQuality)));
                                if (currentQuality < _maxQuality - 5)
                                {
                                    refreshNeeded = true;
                                    Debug.WriteLine($"Quality Reduced: {currentQuality}");
                                }
                            }

                            using var clone = currentFrame.Clone(diffArea, currentFrame.PixelFormat);
                            //var resizeW = diffArea.Width * currentQuality / _maxQuality;
                            //var resizeH = diffArea.Height * currentQuality / _maxQuality;
                            //using var resized = new Bitmap(clone, new Size(resizeW, resizeH));
                            encodedImageBytes = ImageUtils.EncodeJpeg(clone, currentQuality);
                        }

                        viewer.Capturer.CaptureFullscreen = false;

                        await sendFramesLock.WaitAsync();
                        SendFrame(encodedImageBytes, diffArea, viewer, sendFramesLock);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex);
                    }
                }

                Logger.Write($"Ended screen cast.  " +
                    $"Requester: {viewer.Name}. " +
                    $"Viewer ID: {viewer.ViewerConnectionID}. " +
                    $"Viewer WS Connected: {viewer.IsConnected}.  " +
                    $"Viewer Stalled: {viewer.IsStalled}.  " +
                    $"Viewer Disconnected Requested: {viewer.DisconnectRequested}");

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

        private static void SendFrame(byte[] encodedImageBytes, Rectangle diffArea, Viewer viewer, SemaphoreSlim sendFramesLock)
        {
            _ = Task.Run(async () =>
            {
                try
                {

                    if (encodedImageBytes.Length == 0)
                    {
                        return;
                    }

                    await viewer.SendScreenCapture(new CaptureFrame()
                    {
                        EncodedImageBytes = encodedImageBytes,
                        Top = diffArea.Top,
                        Left = diffArea.Left,
                        Width = diffArea.Width,
                        Height = diffArea.Height,
                    });

                }
                finally
                {
                    sendFramesLock.Release();
                }
            });
        }
    }
}
