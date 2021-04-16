using System;

namespace Remotely.Shared.Models
{
    public class ConnectionInfo
    {
        private string _host;
        public string DeviceID { get; set; } = Guid.NewGuid().ToString();
        public string Host
        {
            get
            {
                return _host?.Trim()?.TrimEnd('/');
            }
            set
            {
                _host = value?.Trim()?.TrimEnd('/');
            }
        }
        public string OrganizationID { get; set; }
        public string ServerVerificationToken { get; set; }

    }
}
