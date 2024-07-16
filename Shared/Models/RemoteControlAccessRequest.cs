using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Immense.RemoteControl.Shared.Models;

[DataContract]
public class RemoteControlAccessRequest
{
    [JsonConstructor]
    [SerializationConstructor]
    public RemoteControlAccessRequest(string viewerConnectionId, string requesterDisplayName, string organizationName)
    {
        ViewerConnectionId = viewerConnectionId;
        RequesterDisplayName = requesterDisplayName;
        OrganizationName = organizationName;
    }

    [DataMember]
    public string OrganizationName { get; }

    [DataMember]
    public string RequesterDisplayName { get; }

    [DataMember]
    public string ViewerConnectionId { get; }
}
