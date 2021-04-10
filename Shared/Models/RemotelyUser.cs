using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Models
{
    public class RemotelyUser : IdentityUser
    {
        public ICollection<Alert> Alerts { get; set; }

        public List<DeviceGroup> DeviceGroups { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsServerAdmin { get; set; }

        [JsonIgnore]
        public Organization Organization { get; set; }

        public string OrganizationID { get; set; }

        public List<SavedScript> SavedScripts { get; set; }
        public List<ScriptSchedule> ScriptSchedules { get; set; }

        public string TempPassword { get; set; }

        public RemotelyUserOptions UserOptions { get; set; }
    }
}
