using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Services;
using Remotely.Shared.Helpers;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using Remotely.Shared.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Remotely.Desktop.Core.Models
{
    public class Viewer : IDisposable
    {
        private readonly int defaultImageQuality = 60;
        private int imageQuality;
        private DateTimeOffset lastQualityAdjustment;
        public Viewer(CasterSocket casterSocket,
            IScreenCapturer screenCapturer,
            IClipboardService clipboardService,
            IWebRtcSessionFactory webRtcSessionFactory,
            IAudioCapturer audioCapturer)
        {
            Capturer = screenCapturer;
            CasterSocket = casterSocket;
            WebRtcSessionFactory = webRtcSessionFactory;
            EncoderParams = new EncoderParameters();
            ImageQuality = defaultImageQuality;
            ClipboardService = clipboardService;
            ClipboardService.ClipboardTextChanged += ClipboardService_ClipboardTextChanged;
            AudioCapturer = audioCapturer;
            AudioCapturer.AudioSampleReady += AudioCapturer_AudioSampleReady;
        }

        public bool AutoAdjustQuality { get; set; } = true;

        public IScreenCapturer Capturer { get; }

        public bool DisconnectRequested { get; set; }
        public EncoderParameters EncoderParams { get; private set; }
        public bool HasControl { get; set; } = true;
        public int ImageQuality
        {
            get
            {
                return imageQuality;
            }
            set
            {
                if (imageQuality == value)
                {
                    return;
                }

                if (imageQuality > 100 || imageQuality < 0)
                {
                    return;
                }

                imageQuality = value;

                EncoderParams.Param[0] = new EncoderParameter(Encoder.Quality, value);
            }
        }

        public bool IsConnected => CasterSocket.IsConnected;
        public bool IsStalled
        {
            get
            {
                return PendingSentFrames.TryPeek(out var result) && DateTimeOffset.Now - result > TimeSpan.FromSeconds(15);
            }
        }

        public bool IsUsingWebRtc
        {
            get
            {
                return RtcSession?.IsPeerConnected == true && RtcSession?.IsDataChannelOpen == true;
            }
        }

        public bool IsUsingWebRtcVideo
        {
            get
            {
                return RtcSession?.IsPeerConnected == true && RtcSession?.IsVideoTrackConnected == true;
            }
        }

        public string Name { get; set; }
        public ConcurrentQueue<DateTimeOffset> PendingSentFrames { get; } = new ConcurrentQueue<DateTimeOffset>();
        public WebRtcSession RtcSession { get; set; }
        public string ViewerConnectionID { get; set; }
        private IAudioCapturer AudioCapturer { get; }
        private CasterSocket CasterSocket { get; }
        private IClipboardService ClipboardService { get; }

        private IWebRtcSessionFactory WebRtcSessionFactory { get; }
        public void Dispose()
        {
            DisconnectRequested = true;
            Disposer.TryDisposeAll(new IDisposable[]
            {
                RtcSession,
                Capturer
            });
        }

        public async Task InitializeWebRtc()
        {
            try
            {
                var iceServers = await CasterSocket.GetIceServers();

                RtcSession = WebRtcSessionFactory.GetNewSession(this);
                RtcSession.LocalSdpReady += async (sender, sdp) =>
                {
                    await CasterSocket.SendRtcOfferToBrowser(sdp.Content, ViewerConnectionID, iceServers);
                };
                RtcSession.IceCandidateReady += async (sender, candidate) =>
                {
                    await CasterSocket.SendIceCandidateToBrowser(candidate.Content, candidate.SdpMlineIndex, candidate.SdpMid, ViewerConnectionID);
                };

                await RtcSession.Init(iceServers);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
        public async Task SendAudioSample(byte[] audioSample)
        {
            await SendToViewer(() => RtcSession.SendAudioSample(audioSample),
                () => CasterSocket.SendAudioSample(audioSample, ViewerConnectionID));
        }

        public async Task SendClipboardText(string clipboardText)
        {
            await SendToViewer(() => RtcSession.SendClipboardText(clipboardText),
                () => CasterSocket.SendClipboardText(clipboardText, ViewerConnectionID));
        }
        public async Task SendCursorChange(CursorInfo cursorInfo)
        {
            await SendToViewer(() => RtcSession.SendCursorChange(cursorInfo),
                () => CasterSocket.SendCursorChange(cursorInfo, ViewerConnectionID));
        }

        public async Task SendMachineName(string machineName)
        {
            await SendToViewer(() => RtcSession.SendMachineName(machineName),
                () => CasterSocket.SendMachineName(machineName, ViewerConnectionID));
        }

        public async Task SendScreenCapture(byte[] encodedImageBytes, int left, int top, int width, int height)
        {
            PendingSentFrames.Enqueue(DateTimeOffset.Now);

            await SendToViewer(() =>
            {
                RtcSession.SendCaptureFrame(left, top, width, height, encodedImageBytes, ImageQuality);
            }, async () =>
            {
                await CasterSocket.SendScreenCapture(encodedImageBytes, ViewerConnectionID, left, top, width, height, ImageQuality);
            });
        }

        public async Task SendScreenData(string selectedScreen, string[] displayNames)
        {
            await SendToViewer(() => RtcSession.SendScreenData(selectedScreen, displayNames),
                () => CasterSocket.SendScreenData(selectedScreen, displayNames, ViewerConnectionID));
        }

        public async Task SendScreenSize(int width, int height)
        {
            await SendToViewer(() => RtcSession.SendScreenSize(width, height),
                 () => CasterSocket.SendScreenSize(width, height, ViewerConnectionID));
        }

        public async Task SendWindowsSessions()
        {
            if (EnvironmentHelper.IsWindows)
            {
                await SendToViewer(() => RtcSession.SendWindowsSessions(Win32Interop.GetActiveSessions()),
                    () => CasterSocket.SendWindowsSessions(Win32Interop.GetActiveSessions(), ViewerConnectionID));
            }
        }

        public void ThrottleIfNeeded()
        {
            if (AutoAdjustQuality && DateTimeOffset.Now - lastQualityAdjustment > TimeSpan.FromSeconds(2))
            {
                lastQualityAdjustment = DateTimeOffset.Now;
                if (PendingSentFrames.TryPeek(out var result) && DateTimeOffset.Now - result > TimeSpan.FromMilliseconds(200))
                {
                    var latency = (DateTimeOffset.Now - result).TotalMilliseconds;
                    ImageQuality = (int)(200 / latency * defaultImageQuality);
                }
                else
                {
                    ImageQuality = defaultImageQuality;
                }
            }

            TaskHelper.DelayUntil(() => PendingSentFrames.Count < 5 &&
                (
                    !PendingSentFrames.TryPeek(out var result) || DateTimeOffset.Now - result < TimeSpan.FromSeconds(1)
                ),
                TimeSpan.MaxValue);
        }

        public void ToggleWebRtcVideo(bool toggleOn)
        {
            RtcSession.ToggleWebRtcVideo(toggleOn);
        }
        private async void AudioCapturer_AudioSampleReady(object sender, byte[] sample)
        {
            await SendAudioSample(sample);
        }
        private async void ClipboardService_ClipboardTextChanged(object sender, string clipboardText)
        {
            await SendClipboardText(clipboardText);
        }
        private Task SendToViewer(Action webRtcSend, Func<Task> websocketSend)
        {
            if (IsUsingWebRtc)
            {
                webRtcSend();
                return Task.CompletedTask;
            }
            else
            {
                return websocketSend();
            }
        }
    }
}
