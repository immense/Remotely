using Remotely.ScreenCast.Core.Communication;
using Remotely.ScreenCast.Core.Interfaces;
using Remotely.ScreenCast.Core.Services;
using System;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Remotely.ScreenCast.Core.Models
{
    public class Viewer : IDisposable
    {
        private int imageQuality;

        public Viewer(IScreenCapturer screenCapturer,
            CasterSocket casterSocket)
        {
            Capturer = screenCapturer;
            CasterSocket = casterSocket;
            EncoderParams = new EncoderParameters();
            ImageQuality = 60;
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
                if (imageQuality > 100 || imageQuality < 0)
                {
                    return;
                }
                if (imageQuality == value)
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
        private CasterSocket CasterSocket { get; }
        public void Disconnect()
        {
            RtcSession.Dispose();
        }

        public void Dispose()
        {
            RtcSession?.Dispose();
        }

        public async Task InitializeWebRtc()
        {
            try
            {
                RtcSession = new WebRtcSession();
                RtcSession.LocalSdpReady += async (sender, sdp) =>
                {
                    await CasterSocket.SendRtcOfferToBrowser(sdp, ViewerConnectionID);
                };
                RtcSession.IceCandidateReady += async (sender, args) =>
                {
                    await CasterSocket.SendIceCandidateToBrowser(args.candidate, args.sdpMlineIndex, args.sdpMid, ViewerConnectionID);
                };
                await RtcSession.Init();
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

        public async Task SendClipboardText(string clipboardText)
        {
            await SendToViewer(() => RtcSession.SendClipboardText(clipboardText),
                () => CasterSocket.SendClipboardText(clipboardText, ViewerConnectionID));
        }

        public async Task SendMachineName(string machineName, string viewerID)
        {
            await SendToViewer(()=> RtcSession.SendMachineName(machineName),
                () => CasterSocket.SendMachineName(machineName, viewerID));
        }

        public async Task SendScreenCapture(byte[] encodedImageBytes, string viewerID, int left, int top, int width, int height, int imageQuality)
        {
            await SendToViewer(() =>
            {
                RtcSession.SendCaptureFrame(left, top, width, height, encodedImageBytes, ImageQuality);
                WebSocketBuffer = 0;
            }, async () =>
            {
                await CasterSocket.SendScreenCapture(encodedImageBytes, viewerID, left, top, width, height, imageQuality);
                WebSocketBuffer += encodedImageBytes.Length;
            });
        }

        public async Task SendScreenData(string selectedScreen, string[] displayNames, string viewerID)
        {
            await SendToViewer(() => RtcSession.SendScreenData(selectedScreen, displayNames),
                () => CasterSocket.SendScreenData(selectedScreen, displayNames, viewerID));
        }

        public async Task SendScreenSize(int width, int height, string viewerID)
        {
            await SendToViewer(() => RtcSession.SendScreenSize(width, height),
                 () => CasterSocket.SendScreenSize(width, height, viewerID));
        }

        public async Task ThrottleIfNeeded()
        {
            var currentBuffer = IsUsingWebRtc() ?
                (int)RtcSession.CurrentBuffer :
                WebSocketBuffer;

            if (currentBuffer > 150_000)
            {
                if (AutoAdjustQuality)
                {
                    ImageQuality = Math.Max(ImageQuality - (150_000 / currentBuffer), 0);
                    Logger.Debug($"Auto-adjusting image quality.  Quality: {ImageQuality}");
                }

                var delay = (int)Math.Ceiling((currentBuffer - 150_000) * .0025);
                Logger.Debug($"Throttling output due to buffer size.  Size: {currentBuffer}.  Delay: {delay}");
                await Task.Delay(delay);
            }
            else if (AutoAdjustQuality)
            {
                ImageQuality = Math.Min(ImageQuality + 1, 60);
            }
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
