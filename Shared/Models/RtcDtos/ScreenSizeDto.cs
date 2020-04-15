using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Models.RtcDtos
{
    [DataContract]
    public class ScreenSizeDto : BinaryDtoBase
    {
        public ScreenSizeDto(int width, int height)
        {
            Width = width;
            Height = height;
        }

        [DataMember(Name = "Width")]
        public int Width { get; }

        [DataMember(Name = "Height")]
        public int Height { get; }

        [DataMember(Name = "DtoType")]
        public new BinaryDtoType DtoType { get; } = BinaryDtoType.ScreenSize;
    }
}
