using Remotely.Shared.Enums;
using System.Runtime.Serialization;

namespace Remotely.Shared.Models.RemoteControlDtos
{
    [DataContract]
    public class ScreenSizeDto : BaseDto
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
        public new BaseDtoType DtoType { get; } = BaseDtoType.ScreenSize;
    }
}
