using System.Diagnostics;
using System.Reflection;

namespace Remotely.Shared.Utilities;

public static class AppVersionHelper
{
    public static string GetAppVersion(string defaultVersion = "1.0.0")
    {
        try
        {
            var filePath = Assembly.GetEntryAssembly()?.Location;
            if (TryGetFileVersion(filePath, out var asmVersion))
            {
                return asmVersion;
            }

            if (TryGetFileVersion(Environment.ProcessPath, out var exeVersion))
            {
                return exeVersion;
            }

            return defaultVersion;
        }
        catch
        {
            return defaultVersion;
        }
    }

    private static bool TryGetFileVersion(string? filePath, out string version)
    {
        try
        {
            if (File.Exists(filePath))
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(filePath);
                if (Version.TryParse(versionInfo.FileVersion, out _))
                {
                    version = versionInfo.FileVersion;
                    return true;
                }
            }
        }
        catch
        {
            // Ignored.
        }

        version = string.Empty;
        return false;
    }
}
