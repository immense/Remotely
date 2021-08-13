using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Models
{
    public struct SentFrame
    {
        public SentFrame(int frameSize)
        {
            Timestamp = Time.Now;
            FrameSize = frameSize;
        }

        public DateTimeOffset Timestamp { get; }
        public int FrameSize { get; }
    }
}
