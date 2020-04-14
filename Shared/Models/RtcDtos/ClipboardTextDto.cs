using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Models.RtcDtos
{
    [DataContract]
    public class ClipboardTextDto : IBinaryDto
    {
        public ClipboardTextDto(string clipboardText)
        {
            ClipboardText = clipboardText;
        }

        [DataMember(Name = "ClipboardText")]
        public string ClipboardText { get; }


        [DataMember(Name = "DtoType")]
        public BinaryDtoType DtoType { get; } = BinaryDtoType.ClipboardText;
    }
}
