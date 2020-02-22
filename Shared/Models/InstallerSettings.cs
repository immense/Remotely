using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Models
{
    public class InstallerSettings
    {
        public string OrganizationID { get; set; }

        public string OrganizationName { get; set; }

        public string ServerUrl { get; set; }
    }
}
