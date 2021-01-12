using Microsoft.Extensions.DependencyInjection;
using Remotely.Desktop.Core.Enums;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Models;
using Remotely.Desktop.Core.Utilities;
using Remotely.Shared.Helpers;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
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
                        await viewer.SendScreenCapture(new CaptureFrame()
                        {
                            EncodedImageBytes = ImageUtils.EncodeBitmap(initialFrame, viewer.EncoderParams),
                            Left = viewer.Capturer.CurrentScreenBounds.Left,
                            Top = viewer.Capturer.CurrentScreenBounds.Top,
                            Width = viewer.Capturer.CurrentScreenBounds.Width,
                            Height = viewer.Capturer.CurrentScreenBounds.Height
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

                        viewer.ThrottleIfNeeded();

                        if (currentFrame != null)
                        {
                            previousFrame?.Dispose();
                            previousFrame = (Bitmap)currentFrame.Clone();
                        }

                        currentFrame?.Dispose();
                        currentFrame = viewer.Capturer.GetNextFrame();

                        var diffArea = ImageUtils.GetDiffArea(currentFrame, previousFrame, viewer.Capturer.CaptureFullscreen);

                        if (diffArea.IsEmpty)
                        {
                            continue;
                        }


                        viewer.Capturer.CaptureFullscreen = false;

                        var frameClone = (Bitmap)currentFrame.Clone();
                        await sendFramesLock.WaitAsync();
                        SendFrame(frameClone, diffArea, viewer, sendFramesLock);
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

        private void SendDiffFrame(Bitmap diffImage, Viewer viewer, SemaphoreSlim sendFramesLock)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var encodedImageBytes = ImageUtils.EncodeGif(diffImage);

                    if (encodedImageBytes?.Length > 0)
                    {
                        await viewer.SendScreenCapture(new CaptureFrame()
                        {
                            EncodedImageBytes = encodedImageBytes,
                            Top = 0,
                            Left = 0,
                            Width = diffImage.Width,
                            Height = diffImage.Height,
                        });
                    }
                }
                finally
                {
                    sendFramesLock.Release();
                    diffImage.Dispose();
                }
            });
        }

        private static void SendFrame(Bitmap currentFrame, Rectangle diffArea, Viewer viewer, SemaphoreSlim sendFramesLock)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    using var newImage = currentFrame.Clone(diffArea, PixelFormat.Format32bppArgb);

                    var encodedImageBytes = ImageUtils.EncodeBitmap(newImage, viewer.EncoderParams);

                    if (encodedImageBytes?.Length > 0)
                    {
                        await viewer.SendScreenCapture(new CaptureFrame()
                        {
                            EncodedImageBytes = encodedImageBytes,
                            Top = diffArea.Top,
                            Left = diffArea.Left,
                            Width = diffArea.Width,
                            Height = diffArea.Height,
                        });
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
