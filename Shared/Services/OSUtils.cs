using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Remotely.Shared.Services
{
    public static class OSUtils
    {
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
        public static string ClientExecutableFileName
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
        public static string ScreenCastExecutableFileName
        {
            get
            {
                if (IsWindows)
                {
                    return "Remotely_ScreenCast.exe";
                }
                else if (IsLinux)
                {
                    return "Remotely_ScreenCast.Linux";
                }
                else
                {
                    throw new Exception("Unsupported operating system.");
                }
            }
        }

        public static OSPlatform GetPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            }
            else
            {
                return OSPlatform.Create("Unknown");
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
