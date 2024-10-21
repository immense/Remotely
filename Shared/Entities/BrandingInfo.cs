#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Entities;

public class BrandingInfo
{
    public static BrandingInfo Default => new();

    public byte[] Icon { get; set; } = [];

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = null!;

    [JsonIgnore]
    public Organization? Organization { get; set; }

    public string OrganizationId { get; set; } = null!;

    [StringLength(25)]
    public string Product { get; set; } = "Remote Control";
}
