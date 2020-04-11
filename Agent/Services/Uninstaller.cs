using Remotely.Shared.Utilities;
using System;
using System.Diagnostics;
using System.IO;

namespace Remotely.Agent.Services
{
    public class Uninstaller
    {
        public void UninstallAgent()
        {
            if (EnvironmentHelper.IsWindows)
            {
                Process.Start("cmd.exe", "/c sc delete Remotely_Service");
                var currentDir = AppDomain.CurrentDomain.BaseDirectory;
                Process.Start("cmd.exe", $"/c timeout 5 & rd /s /q \"{currentDir}\"");
            }
            else if (EnvironmentHelper.IsLinux)
            {
                Process.Start("sudo", "systemctl stop remotely-agent").WaitForExit();
                Directory.Delete("/usr/local/bin/Remotely", true);
                File.Delete("/etc/systemd/system/remotely-agent.service");
                Process.Start("sudo", "systemctl daemon-reload").WaitForExit();
            }
            Environment.Exit(0);
        }
    }
}
