using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Enums
{
    [DataContract]
    public enum DynamicDtoType
    {
        [EnumMember(Value = "FrameInfo")]
        FrameInfo = 0
    }
}
