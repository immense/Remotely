using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Models.RemoteControlDtos
{
    [DataContract]
    public class SelectScreenDto : BaseDto
    {
        [DataMember(Name = "DisplayName")]
        public string DisplayName { get; set; }

        [DataMember(Name = "DtoType")]
        public new BaseDtoType DtoType { get; } = BaseDtoType.SelectScreen;
    }
}
