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
using SkiaSharp;

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
            _ = Task.Run(() => BeginScreenCastingImpl(screenCastRequest));
        }

        private async Task BeginScreenCastingImpl(ScreenCastRequest screenCastRequest)
        {
            try
            {
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

                await viewer.SendCursorChange(_cursorIconWatcher.GetCurrentCursor());

                await viewer.SendWindowsSessions();

                viewer.Capturer.ScreenChanged += async (sender, bounds) =>
                {
                    await viewer.SendScreenSize(bounds.Width, bounds.Height);
                };

                // This gets disposed internally in the Capturer on the next call.
                var result = viewer.Capturer.GetNextFrame();

                if (result.IsSuccess && result.Value is not null)
                {
                    await viewer.SendScreenCapture(new CaptureFrame()
                    {
                        EncodedImageBytes = ImageUtils.EncodeBitmap(result.Value, SKEncodedImageFormat.Jpeg, viewer.ImageQuality),
                        Left = screenBounds.Left,
                        Top = screenBounds.Top,
                        Width = screenBounds.Width,
                        Height = screenBounds.Height
                    });
                }

                // Wait until the first image is received.
                if (!TaskHelper.DelayUntil(() => !viewer.PendingSentFrames.Any(), TimeSpan.FromSeconds(30)))
                {
                    Logger.Write("Timed out while waiting for first frame receipt.");
                    _conductor.Viewers.TryRemove(viewer.ViewerConnectionID, out _);
                    viewer.Dispose();
                    return;
                }

                _ = Task.Run(() => CastScreen(screenCastRequest, viewer, 0));
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private async Task CastScreen(ScreenCastRequest screenCastRequest, Viewer viewer, int sequence)
        {
            try
            {

                while (!viewer.DisconnectRequested && viewer.IsConnected)
                {
                    try
                    {
                        if (viewer.IsStalled)
                        {
                            // Viewer isn't responding.  Abort sending.
                            Logger.Write("Viewer stalled.  Ending send loop.");
                            break;
                        }

                        viewer.CalculateFps();

                        viewer.ApplyAutoQuality();

                        var result = viewer.Capturer.GetNextFrame();

                        if (!result.IsSuccess || result.Value is null)
                        {
                            _ = Task.Run(() => CastScreen(screenCastRequest, viewer, sequence));
                            return;
                        }

                        var diffArea = viewer.Capturer.GetFrameDiffArea();

                        if (diffArea.IsEmpty)
                        {
                            continue;
                        }

                        viewer.Capturer.CaptureFullscreen = false;

                        using var croppedFrame = ImageUtils.CropBitmap(result.Value, diffArea);

                        var encodedImageBytes = ImageUtils.EncodeBitmap(croppedFrame, SKEncodedImageFormat.Jpeg, viewer.ImageQuality);

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

        private static async Task SendFrame(byte[] encodedImageBytes, SKRect diffArea, long sequence, Viewer viewer)
        {
            if (encodedImageBytes.Length == 0)
            {
                return;
            }

            await viewer.SendScreenCapture(new CaptureFrame()
            {
                EncodedImageBytes = encodedImageBytes,
                Top = (int)diffArea.Top,
                Left = (int)diffArea.Left,
                Width = (int)diffArea.Width,
                Height = (int)diffArea.Height,
                Sequence = sequence
            });
        }
    }
}
