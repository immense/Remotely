using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Models
{
    public struct SentFrame
    {
        public SentFrame(DateTimeOffset timestamp, int frameSize)
        {
            Timestamp = timestamp;
            FrameSize = frameSize;
        }

        public DateTimeOffset Timestamp { get; }
        public double FrameSize { get; }
    }
}
