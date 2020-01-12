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
        public RemotelyUserOptions UserOptions { get; set; } = new RemotelyUserOptions();

        [JsonIgnore]
        public virtual Organization Organization { get; set; } = new Organization();
        public string OrganizationID { get; set; }

        public bool IsAdministrator { get; set; } = true;
    }
}
