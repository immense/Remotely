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

namespace Remotely.ScreenCast.Core.Services
{
    public class ScreenCasterBase
    {
        public async Task BeginScreenCasting(string viewerID,
            string requesterName)
        {
            byte[] encodedImageBytes;
            var fpsQueue = new Queue<DateTimeOffset>();
            var conductor = ServiceContainer.Instance.GetRequiredService<Conductor>();
            var mode = conductor.Mode;
            var viewer = ServiceContainer.Instance.GetRequiredService<Viewer>();
            viewer.Name = requesterName;
            viewer.ViewerConnectionID = viewerID;
            var viewers = conductor.Viewers;

            Logger.Write($"Starting screen cast.  Requester: {requesterName}. Viewer ID: {viewerID}.  App Mode: {mode}");

            viewers.AddOrUpdate(viewerID, viewer, (id, v) => viewer);

            if (mode == Enums.AppMode.Normal)
            {
                conductor.InvokeViewerAdded(viewer);
            }

            if (EnvironmentHelper.IsWindows)
            {
                await viewer.InitializeWebRtc();
            }

            await viewer.SendMachineName(Environment.MachineName, viewerID);
            
            await viewer.SendScreenData(
                   viewer.Capturer.SelectedScreen,
                   viewer.Capturer.GetDisplayNames().ToArray(),
                   viewerID);

            await viewer.SendScreenSize(viewer.Capturer.CurrentScreenBounds.Width,
                viewer.Capturer.CurrentScreenBounds.Height, 
                viewerID);

            viewer.Capturer.ScreenChanged += async (sender, bounds) =>
            {
                await viewer.SendScreenSize(bounds.Width, bounds.Height, viewerID);
            };

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
                         
                            await viewer.SendScreenCapture(encodedImageBytes, viewerID, diffArea.Left, diffArea.Top, diffArea.Width, diffArea.Height);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }

            Logger.Write($"Ended screen cast.  Requester: {requesterName}. Viewer ID: {viewerID}.");
            viewers.TryRemove(viewerID, out _);

            try
            {
                viewer.Dispose();

            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                // Close if no one is viewing.
                if (viewers.Count == 0 && mode == Enums.AppMode.Unattended)
                {
                    Logger.Debug($"Exiting process ID {Process.GetCurrentProcess().Id}.");
                    Environment.Exit(0);
                }
            }
        }

       
    }
}
