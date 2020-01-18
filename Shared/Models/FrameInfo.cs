using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Models
{
    [DataContract]
    public class FrameInfo : IDynamicDto
    {
        [DataMember(Name = "EndOfFrame")]
        public bool EndOfFrame { get; set; }

        [DataMember(Name = "Height")]
        public int Height { get; set; }

        [DataMember(Name = "ImageBytes")]
        public byte[] ImageBytes { get; set; }

        [DataMember(Name = "Left")]
        public int Left { get; set; }

        [DataMember(Name = "ModelType")]
        public DynamicDtoType DtoType { get; set; }

        [DataMember(Name = "Top")]
        public int Top { get; set; }
        [DataMember(Name = "Width")]
        public int Width { get; set; }
    }
}
