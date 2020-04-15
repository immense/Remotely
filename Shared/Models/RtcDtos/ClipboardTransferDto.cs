using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Models.RtcDtos
{
    [DataContract]
    public class ClipboardTransferDto : BinaryDtoBase
    {

        [DataMember(Name = "Text")]
        public string Text { get; set; }

        [DataMember(Name = "TypeText")]
        public bool TypeText { get; set; }


        [DataMember(Name = "DtoType")]
        public new BinaryDtoType DtoType { get; } = BinaryDtoType.ClipboardTransfer;
    }
}
