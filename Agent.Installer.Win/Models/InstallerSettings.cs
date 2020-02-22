using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Agent.Installer.Win
{
    [DataContract]
    public class InstallerSettings
    {
        [DataMember]
        public string OrganizationID { get; set; }

        [DataMember]
        public string OrganizationName { get; set; }

        [DataMember]
        public string ServerUrl { get; set; }
    }
}
