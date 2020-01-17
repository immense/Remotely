using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Remotely.ScreenCast.Core.Models
{
    [MessagePackObject]
    public class FrameInfo
    {
        [Key("Left")]
        public int Left { get; set; }
        [Key("Top")]
        public int Top { get; set; }
        [Key("Width")]
        public int Width { get; set; }
        [Key("Height")]
        public int Height { get; set; }
        [Key("ImageBytes")]
        public byte[] ImageBytes { get; set; }
    }
}
