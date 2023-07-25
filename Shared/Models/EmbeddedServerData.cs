using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Remotely.Shared.Models;

[DataContract]
public class EmbeddedServerData
{
    [SerializationConstructor]
    [JsonConstructor]
    public EmbeddedServerData(Uri serverUrl, string? organizationId)
    {
        ServerUrl = serverUrl;
        OrganizationId = organizationId ?? string.Empty;
    }

    [DataMember]
    public string OrganizationId { get; }

    [DataMember]
    public Uri ServerUrl { get; }
}
