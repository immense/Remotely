using Remotely.Shared.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Remotely.Agent.Services;

public interface IUninstaller
{
    void UninstallAgent();
}

public class Uninstaller : IUninstaller
{
    private static string EscapeCommandLineArgument(string argument)
    {
        if (string.IsNullOrEmpty(argument))
        {
            return argument;
        }
        
        var sb = new StringBuilder();
        var needsQuoting = false;
        
        foreach (var c in argument)
        {
            if (c == '"' || c == '&' || c == '|' || c == ';' || c == '<' || c == '>' || c == '^' || c == '%' || c == '!')
            {
                needsQuoting = true;
                sb.Append('^');
            }
            sb.Append(c);
        }
        
        if (needsQuoting)
        {
            return $"\"{sb.ToString().Replace("\"", "\\\"")}\"";
        }
        
        return argument.Contains(' ') ? $"\"{argument}\"" : argument;
    }

    public void UninstallAgent()
    {
        if (EnvironmentHelper.IsWindows)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c sc delete Remotely_Service",
                UseShellExecute = false,
                CreateNoWindow = true
            });

            var view = Environment.Is64BitOperatingSystem ?
                "/reg:64" :
                "/reg:32";

            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c REG DELETE HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Remotely /f {view}",
                UseShellExecute = false,
                CreateNoWindow = true
            });

            var currentDir = Path.GetDirectoryName(typeof(Uninstaller).Assembly.Location) ?? "";
            
            var safeDir = new string(currentDir.Where(c => 
                char.IsLetterOrDigit(c) || c == ':' || c == '\\' || c == '/' || c == '-' || c == '_' || c == '.').ToArray());
            
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c timeout 5 & rd /s /q {EscapeCommandLineArgument(safeDir)}",
                UseShellExecute = false,
                CreateNoWindow = true
            });
        }
        else if (EnvironmentHelper.IsLinux)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "sudo",
                Arguments = "systemctl stop remotely-agent",
                UseShellExecute = false,
                CreateNoWindow = true
            }).WaitForExit();
            
            if (Directory.Exists("/usr/local/bin/Remotely"))
            {
                Directory.Delete("/usr/local/bin/Remotely", true);
            }
            
            if (File.Exists("/etc/systemd/system/remotely-agent.service"))
            {
                File.Delete("/etc/systemd/system/remotely-agent.service");
            }
            
            Process.Start(new ProcessStartInfo
            {
                FileName = "sudo",
                Arguments = "systemctl daemon-reload",
                UseShellExecute = false,
                CreateNoWindow = true
            }).WaitForExit();
        }
        Environment.Exit(0);
    }
}
