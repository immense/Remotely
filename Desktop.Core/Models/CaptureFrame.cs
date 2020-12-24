using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Desktop.Core.Models
{
    public class CaptureFrame
    {
        public byte[] EncodedImageBytes { get; init; }
        public int Top { get; init; }
        public int Left { get; init; }
        public int Height { get; init; }
        public int Width { get; init; }
    }
}
