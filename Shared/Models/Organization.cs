using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Remotely.Shared.Models
{
    public class Organization
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        [StringLength(25)]
        public string OrganizationName { get; set; }
        public virtual ICollection<RemotelyUser> RemotelyUsers { get; set; }
        public virtual ICollection<Device> Devices { get; set; }
        public virtual ICollection<CommandContext> CommandContexts { get; set; }
        public virtual ICollection<EventLog> EventLogs { get; set; }
        public virtual ICollection<PermissionGroup> PermissionGroups { get; set; } = new List<PermissionGroup>();
        public virtual ICollection<InviteLink> InviteLinks { get; set; }
        public virtual ICollection<SharedFile> SharedFiles { get; set; }
    }
}