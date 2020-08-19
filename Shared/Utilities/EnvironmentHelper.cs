using Remotely.Shared.Enums;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Remotely.Shared.Utilities
{
    public static class EnvironmentHelper
    {
        public static string AgentExecutableFileName
        {
            get
            {
                string fileExt = "";
                if (IsWindows)
                {
                    fileExt = "Remotely_Agent.exe";
                }
                else if (IsLinux)
                {
                    fileExt = "Remotely_Agent";
                }
                return fileExt;
            }
        }

        public static bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }
        public static bool IsLinux
        {
            get
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            }
        }

        public static bool IsWindows
        {
            get
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            }
        }
        public static Platform Platform
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Platform.Windows;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return Platform.Linux;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return Platform.OSX;
                }
                else
                {
                    return Platform.Unknown;
                }
            }
        }

        public static string DesktopExecutableFileName
        {
            get
            {
                if (IsWindows)
                {
                    return "Remotely_Desktop.exe";
                }
                else if (IsLinux)
                {
                    return "Remotely_Desktop";
                }
                else
                {
                    throw new Exception("Unsupported operating system.");
                }
            }
        }
        public static string StartProcessWithResults(string command, string arguments)
        {
            var psi = new ProcessStartInfo(command, arguments);
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.Verb = "RunAs";
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;

            var proc = new Process();
            proc.StartInfo = psi;

            proc.Start();
            proc.WaitForExit();

            return proc.StandardOutput.ReadToEnd();
        }

    }
}
