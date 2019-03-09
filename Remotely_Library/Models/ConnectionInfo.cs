using System;
using System.Collections.Generic;
using System.Text;

namespace Remotely_Library.Models
{
    public class ConnectionInfo
    {
        public string DeviceID { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
        private string host;
        public string Host
        {
            get
            {
                return host;
            }
            set
            {
                host = value.Trim();
                if (host.EndsWith("/"))
                {
                    host = host.Substring(0, host.LastIndexOf("/"));
                }
            }
        }
        public string OrganizationID { get; set; }
        public string ServerVerificationToken { get; set; }
    }
}
