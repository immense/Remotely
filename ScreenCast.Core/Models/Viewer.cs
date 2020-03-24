using Remotely.ScreenCast.Core.Capture;
using Remotely.ScreenCast.Core.Communication;
using Remotely.ScreenCast.Core.Interfaces;
using Remotely.ScreenCast.Core.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.ScreenCast.Core.Models
{
    public class Viewer : IDisposable
    {
        private long imageQuality;

        public Viewer()
        {
            ImageQuality = 75;
        }
        public bool AutoAdjustQuality { get; internal set; } = true;
        public ICapturer Capturer { get; set; }
        public bool DisconnectRequested { get; set; }
        public EncoderParameters EncoderParams { get; private set; }
        public bool FullScreenRefreshNeeded { get; internal set; }
        public bool HasControl { get; set; }
        public long ImageQuality
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

                EncoderParams = new EncoderParameters()
                {
                    Param = new[]
                    {
                        new EncoderParameter(Encoder.Quality, value)
                    }
                };
            }
        }

        public double Latency { get; set; } = 1;
        public string Name { get; set; }
        public int WebSocketBuffer { get; set; }
        public WebRtcSession RtcSession { get; set; }
        public string ViewerConnectionID { get; set; }

        public void Dispose()
        {
            RtcSession?.Dispose();
        }

        public bool IsStalled()
        {
            return RtcSession?.IsDataChannelOpen != true && Latency > 30000;
        }

        public async Task ThrottleIfNeeded()
        {
            if (IsUsingWebRtc() && RtcSession?.CurrentBuffer > 100_000)
            {
                var delay = (int)Math.Ceiling((RtcSession.CurrentBuffer - 100_000) * .0025);
                Logger.Debug($"Throttling output due to WebRTC buffer.  Size: {RtcSession.CurrentBuffer}.  Delay: {delay}");
                await Task.Delay(delay);
            }
            else if (!IsUsingWebRtc() && WebSocketBuffer > 150_000)
            {
                var delay = (int)Math.Ceiling((WebSocketBuffer - 150_000) * .0025);
                Logger.Debug($"Throttling output due to websocket buffer.  Size: {WebSocketBuffer}.  Delay: {delay}");
                await Task.Delay(delay);
            }
        }

        public bool ShouldAdjustQuality()
        {
            return !IsUsingWebRtc() && AutoAdjustQuality && Latency > 1000;
        }

        public bool IsUsingWebRtc()
        {
            return RtcSession?.IsPeerConnected == true && RtcSession?.IsDataChannelOpen == true;
        }
    }
}
