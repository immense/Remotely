using Remotely.ScreenCast.Core.Communication;
using Remotely.ScreenCast.Core.Interfaces;
using Remotely.ScreenCast.Core.Services;
using Remotely.Shared.Helpers;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using Remotely.Shared.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Remotely.ScreenCast.Core.Models
{
    public class Viewer : IDisposable
    {
        private int imageQuality;

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
            ImageQuality = 60;
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
        public string Name { get; set; }
        public WebRtcSession RtcSession { get; set; }
        public string ViewerConnectionID { get; set; }
        public int WebSocketBuffer { get; set; }
        private IAudioCapturer AudioCapturer { get; }
        private CasterSocket CasterSocket { get; }
        private IClipboardService ClipboardService { get; }
        private int CurrentBuffer
        {
            get
            {
                return IsUsingWebRtc() ?
                    (int)RtcSession.CurrentBuffer :
                    WebSocketBuffer;
            }
        }
        private IWebRtcSessionFactory WebRtcSessionFactory { get; }
        public void Dispose()
        {
            RtcSession?.Dispose();
            Capturer?.Dispose();
        }

        public async Task InitializeWebRtc()
        {
            try
            {
                var iceServers = await CasterSocket.GetIceServers();

                RtcSession = WebRtcSessionFactory.GetNewSession(this);
                RtcSession.LocalSdpReady += async (sender, sdp) =>
                {
                    await CasterSocket.SendRtcOfferToBrowser(sdp, ViewerConnectionID, iceServers);
                };
                RtcSession.IceCandidateReady += async (sender, args) =>
                {
                    await CasterSocket.SendIceCandidateToBrowser(args.candidate, args.sdpMlineIndex, args.sdpMid, ViewerConnectionID);
                };

                await RtcSession.Init(iceServers);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public bool IsStalled()
        {
            return RtcSession?.CurrentBuffer > 1_000_000 || WebSocketBuffer > 1_000_000;
        }

        public bool IsUsingWebRtc()
        {
            return RtcSession?.IsPeerConnected == true && RtcSession?.IsDataChannelOpen == true;
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
            await SendToViewer(() =>
            {
                RtcSession.SendCaptureFrame(left, top, width, height, encodedImageBytes, ImageQuality);
                WebSocketBuffer = 0;
            }, async () =>
            {
                await CasterSocket.SendScreenCapture(encodedImageBytes, ViewerConnectionID, left, top, width, height, ImageQuality);
                WebSocketBuffer += encodedImageBytes.Length;
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

        public async Task ThrottleIfNeeded()
        {
            if (CurrentBuffer > 200_000)
            {
                if (AutoAdjustQuality)
                {
                    var imageAdjust = (double)CurrentBuffer / 200_000 * 5;
                    ImageQuality = (int)Math.Max(ImageQuality - imageAdjust, 0);
                    Logger.Write($"Auto-adjusting image quality.  Quality: {ImageQuality}");
                }

                Logger.Write($"Throttling output due to buffer size.  Size: {CurrentBuffer}.");
                await TaskHelper.DelayUntil(() => CurrentBuffer < 200_000, TimeSpan.FromSeconds(1));
            }
            else if (AutoAdjustQuality)
            {
                ImageQuality = Math.Min(ImageQuality + 1, 60);
            }
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
            if (IsUsingWebRtc())
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
