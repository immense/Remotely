using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Remotely.Shared.Models
{
    public class Alert
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public Device Device { get; set; }
        public string DeviceID { get; set; }
        public string Message { get; set; }
        public ICollection<RemotelyUser> SeenBy { get; set; }
    }
}
