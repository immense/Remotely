using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Installer.Models
{
    public class CliParams
    {
        public bool? CreateNew { get; set; }
        public string GitHubPat { get; set; }
        public string GitHubUsername { get; set; }
        public string InstallDirectory { get; set; }
        public string Reference { get; set; }
        public Uri ServerUrl { get; set; }
        public bool? UsePrebuiltPackage { get; set; }
        public WebServerType? WebServer { get; set; }
    }
}
