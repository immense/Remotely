using Remotely.ScreenCast.Core.Capture;
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
        public Viewer()
        {
            ImageQuality = 50;
        }
        public string ViewerConnectionID { get; set; }
        public string Name { get; set; }
        public ICapturer Capturer { get; set; }
        public bool DisconnectRequested { get; set; }
        public EncoderParameters EncoderParams { get; private set; }
        public bool HasControl { get; set; }
        public double Latency { get; set; } = 1;
        public int PendingFrames { get; set; }

        private long imageQuality = 50;
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
                    Param = new []
                    {
                        new EncoderParameter(Encoder.Quality, value)
                    }
                };
            }
        }
        public bool FullScreenRefreshNeeded { get; internal set; }

    }
}
