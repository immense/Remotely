using Remotely_Library.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Remotely_Agent.Services
{
    public class Uninstaller
    {
        public static void UninstallClient()
        {
            if (OSUtils.IsWindows)
            {
                Process.Start("cmd.exe", "/c sc delete Remotely_Service");
                var tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName()) + ".ps1";
                var currentDir = AppDomain.CurrentDomain.BaseDirectory;
                var ps = PowerShell.Create();
                ps.AddScript($@"
                            $Success = $false;
                            $Count = 0;
                            while ((Test-Path ""{currentDir}"") -eq $true -and $Count -lt 10) {{
                                try {{
                                    Get-Process -Name Agent | Stop-Process -Force; 
                                    Start-Sleep -Seconds 3;
                                    Remove-Item ""{currentDir}"" -Force -Recurse;
                                    $Count++;
                                    continue;
                                }}
                                catch{{
                                    continue;
                                }}
                            }}
                        ");
                ps.Invoke();
            }
            else if (OSUtils.IsLinux)
            {
                var users = OSUtils.StartProcessWithResults("users", "");
                var username = users?.Split()?.FirstOrDefault()?.Trim();
                Process.Start("systemctl", "stop remotely-client").WaitForExit();
                Directory.Delete("/usr/local/bin/Remotely", true);
                File.Delete("/etc/systemd/system/remotely-client.service");
                Process.Start("systemctl", "daemon-reload").WaitForExit();
            }
        }
    }
}
