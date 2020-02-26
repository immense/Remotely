using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Remotely.Shared.Models
{
    public class RemotelyUser : IdentityUser
    {
        public RemotelyUser()
        {
            UserOptions = new RemotelyUserOptions();
            Organization = new Organization();
        }
        public RemotelyUserOptions UserOptions { get; set; }

        [JsonIgnore]
        public Organization Organization { get; set; }
        public string OrganizationID { get; set; }

        public bool IsAdministrator { get; set; } = true;

        public List<UserDevicePermission> PermissionLinks { get; set; }
    }
}
