using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Remotely.Shared.ViewModels
{
    public class Permission
    {
        public string ID { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
    }
    public class OrganizationUser
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
        public List<Permission> Permissions { get; set; }
    }
    public class Invite
    {
        public string ID { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime DateSent { get; set; }
        public string InvitedUser { get; set; }
        public string ResetUrl { get; set; }
    }
}
