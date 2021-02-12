using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Remotely.Shared.Models
{
    public class SharedFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] FileContents { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
        public Organization Organization { get; set; }
        public string OrganizationID { get; set; }
    }
}
