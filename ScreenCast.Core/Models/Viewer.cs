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
        private int imageQuality;

        public Viewer()
        {
            ImageQuality = 60;
        }
        public bool AutoAdjustQuality { get; internal set; } = true;
        public IScreenCapturer Capturer { get; set; }
        public bool DisconnectRequested { get; set; }
        public EncoderParameters EncoderParams { get; private set; }
        public bool FullScreenRefreshNeeded { get; internal set; }
        public bool HasControl { get; set; }
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

                EncoderParams = new EncoderParameters()
                {
                    Param = new[]
                    {
                        new EncoderParameter(Encoder.Quality, value)
                    }
                };
            }
        }
        public string Name { get; set; }
        public WebRtcSession RtcSession { get; set; }
        public string ViewerConnectionID { get; set; }
        public int WebSocketBuffer { get; set; }
        public void Dispose()
        {
            RtcSession?.Dispose();
        }

        public bool IsStalled()
        {
            return RtcSession?.CurrentBuffer > 1_000_000 || WebSocketBuffer > 1_000_000;
        }

        public bool IsUsingWebRtc()
        {
            return RtcSession?.IsPeerConnected == true && RtcSession?.IsDataChannelOpen == true;
        }

        public async Task ThrottleIfNeeded()
        {
            var currentBuffer = IsUsingWebRtc() ? 
                RtcSession.CurrentBuffer :
                (ulong)WebSocketBuffer;

            if (currentBuffer > 150_000)
            {
                if (AutoAdjustQuality)
                {
                    ImageQuality = Math.Max(ImageQuality - 1, 0);
                    Logger.Debug($"Auto-adjusting image quality.  Quality: {ImageQuality}");
                }
               
                var delay = (int)Math.Ceiling((currentBuffer - 100_000) * .0025);
                Logger.Debug($"Throttling output due to buffer size.  Size: {currentBuffer}.  Delay: {delay}");
                await Task.Delay(delay);
            }
            else if (AutoAdjustQuality)
            {
                ImageQuality = Math.Min(ImageQuality + 1, 60);
            }
        }
    }
}
