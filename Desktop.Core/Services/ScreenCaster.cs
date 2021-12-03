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
using System.ComponentModel.DataAnnotations;

namespace Remotely.Desktop.Core.Services
{
    public interface IScreenCaster
    {
        void BeginScreenCasting(ScreenCastRequest screenCastRequest);
    }

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


        public void BeginScreenCasting(ScreenCastRequest screenCastRequest)
        {
            _ = Task.Run(async () => await CastScreen(screenCastRequest));
        }

        private async Task CastScreen(ScreenCastRequest screenCastRequest)
        {
            try
            {
                Bitmap currentFrame = null;
                Bitmap previousFrame = null;
                long sequence = 0;

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

                await viewer.SendScreenData(
                    viewer.Capturer.SelectedScreen,
                    viewer.Capturer.GetDisplayNames(),
                    screenBounds.Width,
                    screenBounds.Height);

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
                            EncodedImageBytes = ImageUtils.EncodeJpeg(initialFrame),
                            Left = screenBounds.Left,
                            Top = screenBounds.Top,
                            Width = screenBounds.Width,
                            Height = screenBounds.Height,
                            Sequence = sequence++
                        });
                    }
                }


                if (EnvironmentHelper.IsWindows && screenCastRequest.UseWebRtc)
                {
                    await viewer.InitializeWebRtc();
                }

                // Wait until the first image is received.
                if (!TaskHelper.DelayUntil(() => !viewer.PendingSentFrames.Any(), TimeSpan.FromSeconds(30)))
                {
                    Logger.Write("Timed out while waiting for first frame receipt.");
                    _conductor.Viewers.TryRemove(viewer.ViewerConnectionID, out _);
                    viewer.Dispose();
                    return;
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
                            Logger.Write("Viewer stalled.  Ending send loop.");
                            break;
                        }

                        viewer.CalculateFps();

                        viewer.ApplyAutoQuality();

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

                        var diffArea = ImageUtils.GetDiffArea(currentFrame, previousFrame, viewer.Capturer.CaptureFullscreen);

                        if (diffArea.IsEmpty)
                        {
                            continue;
                        }

                        viewer.Capturer.CaptureFullscreen = false;

                        using var croppedFrame = currentFrame.Clone(diffArea, currentFrame.PixelFormat);

                        byte[] encodedImageBytes;

                        if (viewer.ImageQuality == Viewer.DefaultQuality)
                        {
                            encodedImageBytes = ImageUtils.EncodeJpeg(croppedFrame);
                        }
                        else
                        {
                            encodedImageBytes = ImageUtils.EncodeJpeg(croppedFrame, viewer.ImageQuality);
                        }

                        await SendFrame(encodedImageBytes, diffArea, sequence++, viewer);

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

        private static async Task SendFrame(byte[] encodedImageBytes, Rectangle diffArea, long sequence, Viewer viewer)
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
                Sequence = sequence
            });
        }
    }
}
