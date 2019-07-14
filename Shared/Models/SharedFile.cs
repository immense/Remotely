using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Remotely.Shared.Models
{
    public class SharedFile
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] FileContents { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public virtual Organization Organization { get; set; }
        public string OrganizationID { get; set; }
    }
}
