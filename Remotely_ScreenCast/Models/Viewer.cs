using Remotely_ScreenCast.Capture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely_ScreenCapture.Models
{
    public class Viewer
    {
        public string ViewerConnectionID { get; set; }
        public string Name { get; set; }
        public ICapturer Capturer { get; set; }
        public int CurrentScreenIndex { get; set; }
        public bool DisconnectRequested { get; set; }
    }
}
