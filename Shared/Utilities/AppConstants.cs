using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Utilities
{
    public class AppConstants
    {
        public const string CentOsUpgradeUrl = "https://api.remotely.one/api/upgrade/centos";

        public static string ClickOnceSetupUrl
        {
            get
            {
#if DEBUG
                return "http://localhost:7071/api/UpdateSetup";
#else
                return  "https://remotely-clickonce.azurewebsites.net/api/UpdateSetup";
#endif
            }
        }

        public const string DefaultProductName = "Remotely";
        public const string DefaultPublisherName = "Translucency Software";
        public const string DeviceInitUrl = "https://remotely-api.azurewebsites.net/api/sponsors/info";
        public const int RelayCodeLength = 4;
        public const string SponsorRegistrationUrl = "https://remotely-api.azurewebsites.net/api/sponsors/register";
        public const string UbuntuUpgradeUrl = "https://remotely-api.azurewebsites.net/api/upgrade/ubuntu";
        public const string WindowsUpgradeUrl = "https://remotely-api.azurewebsites.net/api/upgrade/windows";
    }
}
