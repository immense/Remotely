using Microsoft.Win32;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Agent.Utilities
{
    public static class RegistryHelper
    {
        private static readonly int _minimumRequiredFrameworkVersion = 528040;

        public static bool CheckNetFrameworkVersion()
        {
            try
            {
                var subkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full");
                var installedVersion = subkey.GetValue("Release", string.Empty)?.ToString();

                if (int.TryParse(installedVersion, out var result) &&
                    result < _minimumRequiredFrameworkVersion)
                {
                    Logger.Write($"Minimmum required .NET Framework version ({_minimumRequiredFrameworkVersion} " +
                        $"is greater than installed version ({result}).  Updates will not be installed.", 
                        Shared.Enums.EventType.Warning);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }
    }
}
