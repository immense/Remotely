using Microsoft.AspNetCore.SignalR.Client;
using Remotely.ScreenCast.Core.Models;
using Remotely.ScreenCast.Core.Communication;
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
using System.Drawing.Imaging;
using Microsoft.Extensions.DependencyInjection;

namespace Remotely.ScreenCast.Core.Capture
{
    public class ScreenCasterBase
    {
        public async Task BeginScreenCasting(string viewerID,
                                                   string requesterName,
                                                   ICapturer capturer)
        {
            var conductor = ServiceContainer.Instance.GetRequiredService<Conductor>();
            var viewers = conductor.Viewers;
            var mode = conductor.Mode;
            var casterSocket = ServiceContainer.Instance.GetRequiredService<CasterSocket>();

            Logger.Write($"Starting screen cast.  Requester: {requesterName}. Viewer ID: {viewerID}. Capturer: {capturer.GetType().ToString()}.  App Mode: {mode}");

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

            viewers.AddOrUpdate(viewerID, viewer, (id, v) => viewer);

            if (mode == Enums.AppMode.Normal)
            {
                conductor.InvokeViewerAdded(viewer);
            }

            if (OSUtils.IsWindows)
            {
                await InitializeWebRtc(viewer, casterSocket);
            }

            await casterSocket.SendMachineName(Environment.MachineName, viewerID);

            await casterSocket.SendScreenCount(
                   capturer.SelectedScreen,
                   capturer.GetScreenCount(),
                   viewerID);

            await casterSocket.SendScreenSize(capturer.CurrentScreenBounds.Width, capturer.CurrentScreenBounds.Height, viewerID);

            capturer.ScreenChanged += async (sender, bounds) =>
            {
                await casterSocket.SendScreenSize(bounds.Width, bounds.Height, viewerID);
            };

            while (!viewer.DisconnectRequested)
            {
                try
                {
                    if (viewer.IsStalled())
                    {
                        // Viewer isn't responding.  Abort sending.
                        break;
                    }

                    if (conductor.IsDebug)
                    {
                        while (fpsQueue.Any() && DateTime.Now - fpsQueue.Peek() > TimeSpan.FromSeconds(1))
                        {
                            fpsQueue.Dequeue();
                        }
                        fpsQueue.Enqueue(DateTime.Now);
                        Debug.WriteLine($"Capture FPS: {fpsQueue.Count}");
                    }

                    await viewer.ThrottleIfNeeded();
                  

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

                        if (viewer.ShouldAdjustQuality())
                        {
                            var quality = (int)(viewer.ImageQuality * 1000 / viewer.Latency);
                            Logger.Debug($"Auto-adjusting image quality. Latency: {viewer.Latency}. Quality: {quality}");
                            encodedImageBytes = ImageUtils.EncodeBitmap(newImage, new EncoderParameters()
                            {
                                Param = new[]
                                {
                                    new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality)
                                }
                            });
                        }
                        else
                        {
                            encodedImageBytes = ImageUtils.EncodeBitmap(newImage, viewer.EncoderParams);
                        }

                        if (encodedImageBytes?.Length > 0)
                        {
                            if (viewer.IsUsingWebRtc())
                            {
                                viewer.RtcSession.SendCaptureFrame(diffArea.Left, diffArea.Top, diffArea.Width, diffArea.Height, encodedImageBytes);
                                viewer.WebSocketBuffer = 0;
                                viewer.Latency = 0;
                            }
                            else
                            {
                                await casterSocket.SendScreenCapture(encodedImageBytes, viewerID, diffArea.Left, diffArea.Top, diffArea.Width, diffArea.Height, DateTime.UtcNow);
                                viewer.Latency += 300;
                                // Shave some off so it doesn't get deadlocked by dropped frames.
                                viewer.WebSocketBuffer += (int)(encodedImageBytes.Length * .9);
                            }
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
            viewers.TryRemove(viewerID, out _);
            var shouldExit = viewers.Count == 0 && mode == Enums.AppMode.Unattended;

            try
            {
                viewer.Dispose();

                if (shouldExit)
                {
                    capturer.Dispose();

                    await casterSocket.Disconnect();
                }

            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                // Close if no one is viewing.
                if (shouldExit)
                {
                    Logger.Debug($"Exiting process ID {Process.GetCurrentProcess().Id}.");
                    Environment.Exit(0);
                }
            }
        }

        private async Task InitializeWebRtc(Viewer viewer, CasterSocket casterSocket)
        {
            try
            {
                viewer.RtcSession = new WebRtcSession();
                viewer.RtcSession.LocalSdpReady += async (sender, sdp) =>
                {
                    await casterSocket.SendRtcOfferToBrowser(sdp, viewer.ViewerConnectionID);
                };
                viewer.RtcSession.IceCandidateReady += async (sender, args) =>
                {
                    await casterSocket.SendIceCandidateToBrowser(args.candidate, args.sdpMlineIndex, args.sdpMid, viewer.ViewerConnectionID);
                };
                await viewer.RtcSession.Init();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
    }
}
