using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace Remotely.Server.Models
{
    public class RecordingSessionState
    {
        public MemoryStream FrameBytes { get; set; }
        public Bitmap CumulativeFrame { get; set; }
        public Process FfmpegProcess { get; set; }
    }
}
