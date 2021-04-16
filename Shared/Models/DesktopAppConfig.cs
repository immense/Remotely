using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Models
{
    public class DesktopAppConfig
    {
        private string _host = "";

        public string Host
        {
            get => _host.TrimEnd('/');
            set
            {
                _host = value?.TrimEnd('/');
            }
        }
        public string OrganizationId { get; set; } = "";
    }
}
