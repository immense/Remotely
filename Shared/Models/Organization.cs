using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Remotely.Shared.Models
{
    public class Organization
    {
        public ICollection<ApiToken> ApiTokens { get; set; }

        public ICollection<CommandContext> CommandContexts { get; set; }

        public ICollection<DeviceGroup> DeviceGroups { get; set; }

        public ICollection<Device> Devices { get; set; }

        public ICollection<EventLog> EventLogs { get; set; }

        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        public ICollection<InviteLink> InviteLinks { get; set; }

        [StringLength(25)]
        public string OrganizationName { get; set; }
        public ICollection<RemotelyUser> RemotelyUsers { get; set; }
        public ICollection<SharedFile> SharedFiles { get; set; }
    }
}