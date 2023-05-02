using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Utilities
{
    public static class AppVersionHelper
    {
        public static string GetAppVersion(string defaultVersion = "1.0.0")
        {
            try
            {
                if (File.Exists(Environment.ProcessPath))
                {
                    var versionInfo = FileVersionInfo.GetVersionInfo(Environment.ProcessPath);
                    if (!string.IsNullOrEmpty(versionInfo.FileVersion))
                    {
                        return versionInfo.FileVersion;
                    }
                }

                var asmVersion = Assembly.GetEntryAssembly()?.GetName().Version;
                if (asmVersion is not null && asmVersion > new Version(1, 0, 0))
                {
                    return asmVersion.ToString();
                }

                return defaultVersion;
            }
            catch
            {
                return defaultVersion;
            }
        }
    }
}
