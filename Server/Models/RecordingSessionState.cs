using System.Diagnostics;
using System.Drawing;

namespace Remotely.Server.Models
{
    public class RecordingSessionState
    {
        public Bitmap CumulativeFrame { get; set; }
        public Process FfmpegProcess { get; set; }
    }
}
