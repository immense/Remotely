using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Models.RemoteControlDtos
{
    [DataContract]
    public class ClipboardTransferDto : BaseDto
    {

        [DataMember(Name = "Text")]
        public string Text { get; set; }

        [DataMember(Name = "TypeText")]
        public bool TypeText { get; set; }


        [DataMember(Name = "DtoType")]
        public new BaseDtoType DtoType { get; } = BaseDtoType.ClipboardTransfer;
    }
}
