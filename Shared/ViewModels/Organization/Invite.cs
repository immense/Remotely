using System;
using System.Collections.Generic;
using System.Text;

namespace Remotely.Shared.ViewModels.Organization
{
    public class Invite
    {
        public string ID { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime DateSent { get; set; }
        public string InvitedUser { get; set; }
    }
}
