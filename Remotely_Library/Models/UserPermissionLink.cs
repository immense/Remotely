using System;
using System.Collections.Generic;
using System.Text;

namespace Remotely_Library.Models
{
    public class UserPermissionLink
    {
        public string RemotelyUserID { get; set; }
        public RemotelyUser RemotelyUser { get; set; }
        public string PermissionGroupID { get; set; }
        public PermissionGroup PermissionGroup { get; set; }
    }
}
