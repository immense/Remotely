using Remotely.Shared.Services;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace Remotely.Agent.Services
{
    public class Updater
    {
        internal static void CheckForCoreUpdates()
        {
            try
            {
                var wc = new WebClient();
                var response = new HttpClient().GetAsync(Utilities.GetConnectionInfo().Host + $"/API/CoreVersion/").Result;
                var latestVersion = response.Content.ReadAsStringAsync().Result;
                var thisVersion = FileVersionInfo.GetVersionInfo("Remotely_Agent.dll").FileVersion.ToString().Trim();
                if (thisVersion != latestVersion)
                {
                    Logger.Write($"Service Updater: Downloading update.  Current Version: {thisVersion}.  Latest Version: {latestVersion}.");
                    var fileName = OSUtils.CoreZipFileName;
                    var tempFile = Path.Combine(Path.GetTempPath(), fileName);
                    var tempFolder = Path.Combine(Path.GetTempPath(), "Remotely_Update");
                    if (File.Exists(tempFile))
                    {
                        File.Delete(tempFile);
                    }
                    if (Directory.Exists(tempFolder))
                    {
                        Directory.Delete(tempFolder, true);
                    }
                    wc.DownloadFile(new Uri(Utilities.GetConnectionInfo().Host + $"/Downloads/{fileName}"), tempFile);

                    Logger.Write($"Service Updater: Extracting files.");

                    if (OSUtils.IsWindows)
                    {
                        ZipFile.ExtractToDirectory(tempFile, tempFolder, true);
                        Logger.Write($"Service Updater: Launching extracted process to perform update.");
                        var psi = new ProcessStartInfo()
                        {
                            FileName = Path.Combine(Path.GetTempPath(), "Remotely_Update", OSUtils.ClientExecutableFileName),
                            Arguments = "-update true",
                            Verb = "RunAs"
                        };
                        Process.Start(psi);
                    }
                    else if (OSUtils.IsLinux)
                    {
                        Process.Start("sudo", "apt-get install unzip").WaitForExit();
                        Process.Start("sudo", $"unzip -o {tempFile} -d /usr/local/bin/Remotely/").WaitForExit();
                        Process.Start("sudo", "chmod +x /usr/local/bin/Remotely/Remotely_Agent").WaitForExit();
                        Process.Start("sudo", "chmod +x /usr/local/bin/Remotely/ScreenCast/Remotely_ScreenCast.Linux").WaitForExit();
                        Logger.Write($"Service Updater: Update complete.  Restarting service.");
                        Process.Start("sudo", "systemctl restart remotely-agent");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
        internal static void CoreUpdate()
        {
            try
            {
                Logger.Write("Service Updater: Starting update.");
                var ps = PowerShell.Create();
                if (OSUtils.IsWindows)
                {
                    ps.AddScript(@"Get-Service | Where-Object {$_.Name -like ""Remotely_Service""} | Stop-Service -Force");
                    ps.Invoke();
                    ps.Commands.Clear();
                }
                else if (OSUtils.IsLinux)
                {
                    Process.Start("sudo", "systemctl stop remotely-agent");
                }


                foreach (var proc in Process.GetProcesses().Where(x =>
                                                (x.ProcessName.Contains("Remotely_Agent") ||
                                                    x.ProcessName.Contains("Remotely_ScreenCast") ||
                                                    x.ProcessName.Contains("Remotely_Desktop"))
                                                && x.Id != Process.GetCurrentProcess().Id))
                {
                    proc.Kill();
                }

                string targetDir = "";

                if (OSUtils.IsWindows)
                {
                    targetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Remotely");
                }
                else if (OSUtils.IsLinux)
                {
                    targetDir = "/usr/local/bin/Remotely";
                }
 
                Logger.Write("Service Updater: Copying new files.");


                var fileList = Directory.GetFiles(Path.Combine(Path.GetTempPath(), "Remotely_Update"));
                foreach (var file in fileList)
                {
                    try
                    {
                        var targetPath = Path.Combine(targetDir, Path.GetFileName(file));
                        File.Copy(file, targetPath, true);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex);
                        continue;
                    }
                }


                var subdirList = Directory.GetDirectories(Path.Combine(Path.GetTempPath(), "Remotely_Update"));

                foreach (var subdir in subdirList)
                {
                    try
                    {
                        var targetPath = Path.Combine(targetDir, Path.GetFileName(subdir.TrimEnd(Path.DirectorySeparatorChar)));
                        if (Directory.Exists(targetPath))
                        {
                            Directory.Delete(targetPath, true);
                        }
                        Directory.Move(subdir, targetPath);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex);
                        continue;
                    }
                    
                }
                Logger.Write("Service Updater: Update completed.");
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                Logger.Write("Service Updater: Starting service.");
                if (OSUtils.IsWindows)
                {
                    var ps = PowerShell.Create();
                    ps.AddScript("Start-Service -Name \"Remotely_Service\"");
                    ps.Invoke();
                }
                else if (OSUtils.IsLinux)
                {
                    Process.Start("sudo", "systemctl restart remotely-agent").WaitForExit();
                }
                Environment.Exit(0);
            }
        }
        internal static async Task<string> GetLatestScreenCastVersion()
        {
            var platform = "";
            if (OSUtils.IsWindows)
            {
                platform = "Windows";
            }
            else if (OSUtils.IsLinux)
            {
                platform = "Linux";
            }
            else
            {
                throw new Exception("Unsupported operating system.");
            }
            var response = await new HttpClient().GetAsync(Utilities.GetConnectionInfo().Host + $"/API/ScreenCastVersion/{platform}");
            return await response.Content.ReadAsStringAsync();
        }


    }
}
