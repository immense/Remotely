using Remotely_ScreenCast.Capture;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        public double Latency { get; set; }
        public int PendingFrames { get; set; }
    }
}
