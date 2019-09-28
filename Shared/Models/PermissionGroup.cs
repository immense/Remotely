using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Remotely.Shared.Models
{
    public class PermissionGroup
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        [StringLength(100)]
        public string Name { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<UserPermissionLink> UserPermissionLinks { get; set; } = new List<UserPermissionLink>();
        public virtual ICollection<DevicePermissionLink> DevicePermissionLinks { get; set; } = new List<DevicePermissionLink>();
	}
}
