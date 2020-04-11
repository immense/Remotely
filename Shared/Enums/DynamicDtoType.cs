using System.Runtime.Serialization;

namespace Remotely.Shared.Enums
{
    [DataContract]
    public enum DynamicDtoType
    {
        [EnumMember(Value = "FrameInfo")]
        FrameInfo = 0
    }
}
