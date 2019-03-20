using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Remotely_Library.Models
{
    public class PermissionGroup
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        [StringLength(100)]
        public string Name { get; set; }
        public virtual Organization Organization { get; set; }
		public virtual List<Device> Devices { get; set; }
		public virtual List<RemotelyUser> RemotelyUsers { get; set; }
	}
}
