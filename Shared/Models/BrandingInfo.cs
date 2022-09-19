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

namespace Remotely.Shared.Models
{
    public class BrandingInfo : BrandingInfoBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = string.Empty;

        public string? OrganizationId { get; set; }

        [JsonIgnore]
        public Organization? Organization { get; set; }
    }
}
