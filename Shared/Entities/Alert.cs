using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Entities;

public class Alert
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string ID { get; set; } = null!;

    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

    [JsonIgnore]
    public Device? Device { get; set; }
    public string DeviceID { get; set; } = null!;

    public string? Message { get; set; }

    [JsonIgnore]
    public Organization? Organization { get; set; }

    public string OrganizationID { get; set; } = null!;

    [JsonIgnore]
    public RemotelyUser? User { get; set; }
    public string UserID { get; set; } = null!;
    public string? Details { get; set; }
}
