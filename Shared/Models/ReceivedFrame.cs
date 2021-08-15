using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Models
{
    public struct ReceivedFrame
    {
        public int FrameSize { get; init; }
        public TimeSpan TimeToSend { get; init; }
    }
}
