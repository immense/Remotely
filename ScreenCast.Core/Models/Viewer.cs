using Remotely.ScreenCast.Core.Capture;
using Remotely.ScreenCast.Core.Communication;
using Remotely.ScreenCast.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.ScreenCast.Core.Models
{
    public class Viewer
    {
        private long imageQuality = 50;

        public Viewer()
        {
            ImageQuality = 50;
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
        public int OutputBuffer { get; set; }
        public WebRtcSession RtcSession { get; set; }
        public string ViewerConnectionID { get; set; }
    }
}
