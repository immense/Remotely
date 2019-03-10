using Remotely_ScreenCast.Capture;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely_ScreenCast.Models
{
    public class Viewer
    {
        public string ViewerConnectionID { get; set; }
        public string Name { get; set; }
        public ICapturer Capturer { get; set; }
        public bool DisconnectRequested { get; set; }
        public bool HasControl { get; set; }
        public double Latency { get; set; } = 1;
        public int PendingFrames { get; set; }

        private long imageQuality = 1;
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
                imageQuality = value;
                EncoderParams = new EncoderParameters()
                                {
                                    Param = new EncoderParameter[]
                                    {
                                        new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, value)
                                    }
                                };
            }
        }
        public bool FullScreenRefreshNeeded { get; internal set; }

        public EncoderParameters EncoderParams { get; private set; } = new EncoderParameters()
        {
            Param = new EncoderParameter[]
            {
                new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L)
            }
        };

    }
}
