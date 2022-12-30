using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Models
{
    [DataContract]
    public class EmbeddedServerData
    {
        [SerializationConstructor]
        public EmbeddedServerData(Uri serverUrl, string organizationId)
        {
            ServerUrl = serverUrl;
            OrganizationId = organizationId;
        }

        [DataMember]
        public string OrganizationId { get; }

        [DataMember]
        public Uri ServerUrl { get; }
    }
}
