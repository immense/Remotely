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
        /// <summary>
        /// Parameterless constructor for JsonSerializer.
        /// </summary>
        public EmbeddedServerData() { }

        public EmbeddedServerData(Uri serverUrl, string organizationId)
        {
            ServerUrl = serverUrl;
            OrganizationId = organizationId;
        }

        public static EmbeddedServerData Empty { get; } = new EmbeddedServerData();

        [DataMember]
        public string OrganizationId { get; set; } = string.Empty;

        [DataMember]
        public Uri ServerUrl { get; set; }
    }
}
