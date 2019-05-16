using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely_Server.Models
{
    public class RecordingSessionState
    {
        public Bitmap CumulativeFrame { get; set; }
        public Process FfmpegProcess { get; set; }
    }
}
