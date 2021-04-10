using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Remotely.Shared.Models
{
    public class Organization
    {
        public ICollection<Alert> Alerts { get; set; }

        public ICollection<ApiToken> ApiTokens { get; set; }

        public BrandingInfo BrandingInfo { get; set; }

        public ICollection<ScriptResult> ScriptResults { get; set; }

        public ICollection<ScriptRun> ScriptRuns { get; set; }
        public ICollection<SavedScript> SavedScripts { get; set; }

        public ICollection<ScriptSchedule> ScriptSchedules { get; set; }

        public ICollection<DeviceGroup> DeviceGroups { get; set; }

        public ICollection<Device> Devices { get; set; }

        public ICollection<EventLog> EventLogs { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }

        public ICollection<InviteLink> InviteLinks { get; set; }

        public bool IsDefaultOrganization { get; set; }

        [StringLength(25)]
        public string OrganizationName { get; set; }

        public string RelayCode { get; set; }

        public ICollection<RemotelyUser> RemotelyUsers { get; set; }
        public ICollection<SharedFile> SharedFiles { get; set; }
    }
}