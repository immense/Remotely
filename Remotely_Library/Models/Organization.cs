using Remotely_Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Remotely_Library.Models
{
    public class Organization
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        [StringLength(25)]
        public string OrganizationName { get; set; }
        public virtual List<RemotelyUser> RemotelyUsers { get; set; }
        public virtual List<Device> Devices { get; set; }
        public virtual List<CommandContext> CommandContexts { get; set; }
        public virtual List<EventLog> EventLogs { get; set; }
        public virtual List<PermissionGroup> PermissionGroups { get; set; } = new List<PermissionGroup>();
        public virtual List<InviteLink> InviteLinks { get; set; }
        public virtual List<SharedFile> SharedFiles { get; set; }
    }
}