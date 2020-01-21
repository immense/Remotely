using Remotely.Shared.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Remotely.Agent.Services
{
    public class Uninstaller
    {
        public void UninstallAgent()
        {
            if (OSUtils.IsWindows)
            {
                Process.Start("cmd.exe", "/c sc delete Remotely_Service");
                var deleteTime = DateTime.Now.AddMinutes(2).ToString("HH:mm");
                var currentDir = AppDomain.CurrentDomain.BaseDirectory;
                Process.Start("cmd.exe", $"/c timeout 5 & rd /s /q \"{currentDir}\"");
            }
            else if (OSUtils.IsLinux)
            {
                var users = OSUtils.StartProcessWithResults("users", "");
                var username = users?.Split()?.FirstOrDefault()?.Trim();
                Process.Start("sudo", "systemctl stop remotely-agent").WaitForExit();
                Directory.Delete("/usr/local/bin/Remotely", true);
                File.Delete("/etc/systemd/system/remotely-agent.service");
                Process.Start("sudo", "systemctl daemon-reload").WaitForExit();
            }
            Environment.Exit(0);
        }
    }
}
