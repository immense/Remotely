using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Immense.RemoteControl.Shared.Models.Dtos;

[DataContract]
public class FrameReceivedDto
{
    [DataMember]
    public long Timestamp { get; set; }
}
