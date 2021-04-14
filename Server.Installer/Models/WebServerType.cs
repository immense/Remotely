using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Server.Installer.Models
{
    public enum WebServerType
    {
        UbuntuCaddy,
        UbuntuNginx,
        CentOsCaddy,
        CentOsNginx,
        IisWindows
    }
}
