using MessagePack;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

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
