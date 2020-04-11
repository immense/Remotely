using System;

namespace Remotely.Shared.Models
{
    public class ConnectionInfo
    {
        public string DeviceID { get; set; } = Guid.NewGuid().ToString();
        private string host;
        public string Host
        {
            get
            {
                return host;
            }
            set
            {
                host = value.Trim().TrimEnd('/');
            }
        }
        public string OrganizationID { get; set; }
        public string ServerVerificationToken { get; set; }

    }
}
