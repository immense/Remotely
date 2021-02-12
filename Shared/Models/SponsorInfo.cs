using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Models
{
    public class SponsorInfo
    {
        public double Amount { get; set; }
        public string GithubUser { get; set; }
        public string HostName { get; set; }
        public string OrganizationId { get; set; }
        public string RelayCode { get; set; }
        public string UnlockCode { get; set; }
    }
}
