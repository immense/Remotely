#nullable enable
using Immense.RemoteControl.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
