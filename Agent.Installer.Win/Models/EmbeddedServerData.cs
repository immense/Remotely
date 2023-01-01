using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Agent.Installer.Models
{
    [DataContract]
    public class EmbeddedServerData
    {
        public EmbeddedServerData(Uri serverUrl, string organizationId)
        {
            ServerUrl = serverUrl;
            OrganizationId = organizationId;
        }

        private EmbeddedServerData() { }

        public static EmbeddedServerData Empty { get; } = new EmbeddedServerData();

        [DataMember]
        public string OrganizationId { get; }

        [DataMember]
        public Uri ServerUrl { get; }
    }
}
