using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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

        public virtual Organization Organization { get; set; }
        public string OrganizationID { get; set; }

        public virtual ICollection<UserPermissionLink> UserPermissionLinks { get; set; } = new List<UserPermissionLink>();

        public bool IsAdministrator { get; set; } = true;
    }
}
