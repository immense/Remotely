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

        private double imageQuality = 1;
        public double ImageQuality
        {
            get
            {
                return imageQuality;
            }
            set
            {
                if (imageQuality > 1 || imageQuality < 0)
                {
                    return;
                }
                imageQuality = value;
            }
        }
        public bool FullScreenRefreshNeeded { get; internal set; }

    }
}
