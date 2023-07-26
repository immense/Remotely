using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Remotely.Shared.Entities;

public class SharedFile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string ID { get; set; } = null!;
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public byte[] FileContents { get; set; } = Array.Empty<byte>();
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    public Organization? Organization { get; set; }
    public string? OrganizationID { get; set; }
}
