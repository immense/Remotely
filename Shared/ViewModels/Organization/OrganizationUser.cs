using System;
using System.Collections.Generic;
using System.Text;

namespace Remotely.Shared.ViewModels.Organization
{
    public class OrganizationUser
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
    }
}
