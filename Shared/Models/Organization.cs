using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Remotely.Shared.Models
{
    public class Organization
    {
        public virtual ICollection<ApiToken> ApiTokens { get; set; }

        public virtual ICollection<CommandContext> CommandContexts { get; set; }

        public virtual ICollection<DeviceGroup> DeviceGroups { get; set; }

        public virtual ICollection<Device> Devices { get; set; }

        public virtual ICollection<EventLog> EventLogs { get; set; }

        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        public virtual ICollection<InviteLink> InviteLinks { get; set; }

        [StringLength(25)]
        public string OrganizationName { get; set; }
        public virtual ICollection<RemotelyUser> RemotelyUsers { get; set; }
        public virtual ICollection<SharedFile> SharedFiles { get; set; }
    }
}