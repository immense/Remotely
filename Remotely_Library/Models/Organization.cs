using Remotely_Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Remotely_Library.Models
{
    public class Organization
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString().Replace("-", "");

        [StringLength(25)]
        public string OrganizationName { get; set; }
        public List<RemotelyUser> RemotelyUsers { get; set; }
        public List<Machine> Machines { get; set; }
        public List<CommandContext> CommandContexts { get; set; }
        public List<EventLog> EventLogs { get; set; }
        public List<PermissionGroup> PermissionGroups { get; set; } = new List<PermissionGroup>();
        public List<InviteLink> InviteLinks { get; set; }
        public List<SharedFile> SharedFiles { get; set; }
    }
}