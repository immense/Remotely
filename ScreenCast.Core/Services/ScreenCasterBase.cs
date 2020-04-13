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
        public ScreenCasterBase(Viewer viewer)
        {
            Viewer = viewer;
        }

        protected Viewer Viewer { get; }

        public async Task BeginScreenCasting(string viewerID,
            string requesterName)
        {
            var conductor = ServiceContainer.Instance.GetRequiredService<Conductor>();
            var viewers = conductor.Viewers;
            var mode = conductor.Mode;

            Logger.Write($"Starting screen cast.  Requester: {requesterName}. Viewer ID: {viewerID}.  App Mode: {mode}");

            byte[] encodedImageBytes;
            var fpsQueue = new Queue<DateTimeOffset>();

            Viewer.Name = requesterName;
            Viewer.ViewerConnectionID = viewerID;

            viewers.AddOrUpdate(viewerID, Viewer, (id, v) => Viewer);

            if (mode == Enums.AppMode.Normal)
            {
                conductor.InvokeViewerAdded(Viewer);
            }

            if (EnvironmentHelper.IsWindows)
            {
                await Viewer.InitializeWebRtc();
            }

            await Viewer.SendMachineName(Environment.MachineName, viewerID);
            
            await Viewer.SendScreenData(
                   Viewer.Capturer.SelectedScreen,
                   Viewer.Capturer.GetDisplayNames().ToArray(),
                   viewerID);

            await Viewer.SendScreenSize(Viewer.Capturer.CurrentScreenBounds.Width,
                Viewer.Capturer.CurrentScreenBounds.Height, 
                viewerID);

            Viewer.Capturer.ScreenChanged += async (sender, bounds) =>
            {
                await Viewer.SendScreenSize(bounds.Width, bounds.Height, viewerID);
            };

            while (!Viewer.DisconnectRequested && Viewer.IsConnected)
            {
                try
                {
                    if (Viewer.IsStalled())
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

                    await Viewer.ThrottleIfNeeded();

                    Viewer.Capturer.GetNextFrame();

                    var diffArea = ImageUtils.GetDiffArea(Viewer.Capturer.CurrentFrame, Viewer.Capturer.PreviousFrame, Viewer.Capturer.CaptureFullscreen);

                    if (diffArea.IsEmpty)
                    {
                        continue;
                    }

                    using (var newImage = Viewer.Capturer.CurrentFrame.Clone(diffArea, PixelFormat.Format32bppArgb))
                    {
                        if (Viewer.Capturer.CaptureFullscreen)
                        {
                            Viewer.Capturer.CaptureFullscreen = false;
                        }
                        
                        encodedImageBytes = ImageUtils.EncodeBitmap(newImage, Viewer.EncoderParams);

                        if (encodedImageBytes?.Length > 0)
                        {
                         
                            await Viewer.SendScreenCapture(encodedImageBytes, viewerID, diffArea.Left, diffArea.Top, diffArea.Width, diffArea.Height, Viewer.ImageQuality);
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
                Viewer.Dispose();

                Viewer.Capturer.Dispose();

                Viewer.Disconnect();

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
