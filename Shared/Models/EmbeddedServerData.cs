using MessagePack;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Models;

[DataContract]
public class EmbeddedServerData
{
    private static readonly HashSet<string> AllowedSchemes = new(StringComparer.OrdinalIgnoreCase)
    {
        "https",
        "wss"
    };

    [SerializationConstructor]
    [JsonConstructor]
    public EmbeddedServerData(Uri serverUrl, string? organizationId)
    {
        if (serverUrl != null && !AllowedSchemes.Contains(serverUrl.Scheme))
        {
            throw new ArgumentException($"URL scheme '{serverUrl.Scheme}' is not allowed. Only HTTPS and WSS are permitted.", nameof(serverUrl));
        }

        ServerUrl = serverUrl;
        OrganizationId = organizationId ?? string.Empty;
    }

    [DataMember]
    public string OrganizationId { get; }

    [DataMember]
    public Uri ServerUrl { get; }
}
