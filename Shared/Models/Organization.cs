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

        public ICollection<CommandResult> CommandResults { get; set; }

        public ICollection<DeviceGroup> DeviceGroups { get; set; }

        public ICollection<Device> Devices { get; set; }

        public ICollection<EventLog> EventLogs { get; set; }

        public string GithubUser { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }

        public ICollection<InviteLink> InviteLinks { get; set; }

        public bool IsDefaultOrganization { get; set; }

        public double SponsorAmount { get; set; }

        [NotMapped]
        public SponsorLevel SponsorLevel
        {
            get
            {
                if (SponsorAmount < 10)
                {
                    return SponsorLevel.None;
                }
                if (SponsorAmount < 20)
                {
                    return SponsorLevel.Relay;
                }

                return SponsorLevel.Branding;
            }
        }

        [StringLength(25)]
        public string OrganizationName { get; set; }

        public string RelayCode { get; set; }
        public ICollection<RemotelyUser> RemotelyUsers { get; set; }
        public ICollection<SharedFile> SharedFiles { get; set; }
        public string UnlockCode { get; set; }
    }
}