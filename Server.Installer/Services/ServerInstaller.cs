﻿using Remotely.Shared.Utilities;
using Server.Installer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Remotely.Shared.Extensions;

namespace Server.Installer.Services
{
    public interface IServerInstaller
    {
        Task PerformInstall(CliParams cliParams);
    }

    public class ServerInstaller : IServerInstaller
    {
        private readonly IGitHubApi _githubApi;

        public ServerInstaller(IGitHubApi githubApi)
        {
            _githubApi = githubApi;
        }

        public async Task PerformInstall(CliParams cliParams)
        {
            var zipPath = Path.Combine(Path.GetTempPath(), "Remotely_Server.zip");

            if (cliParams.UsePrebuiltPackage == true)
            {
                ConsoleHelper.WriteLine("Downloading pre-built server package.");

                var releaseFile = cliParams.WebServer == WebServerType.IisWindows ?
                    "https://github.com/immense/Remotely/releases/latest/download/Remotely_Server_Win-x64.zip" :
                    "https://github.com/immense/Remotely/releases/latest/download/Remotely_Server_Linux-x64.zip";

                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(releaseFile);
                var contentLength = response.Content.Headers.ContentLength;

                using var webStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(zipPath, FileMode.Create);

                var progress = 0;

                await webStream.CopyToAsync(fileStream, (int bytesRead) =>
                {
                    if (contentLength is null ||
                        contentLength <= 0)
                    {
                        return;

                    }

                    var newProgress = (double)bytesRead / contentLength * 100;

                    if (newProgress == 100 ||
                        newProgress - progress > 5)
                    {
                        progress = (int)newProgress;
                        ConsoleHelper.WriteLine($"Progress: {progress}%");
                    }
                });

            }
            else
            {
                var latestBuild = await _githubApi.GetLatestBuildArtifact(cliParams);
                var latestBuildId = latestBuild?.id;

                if (cliParams.CreateNew == true)
                {
                    var dispatchResult = await _githubApi.TriggerDispatch(cliParams);

                    if (!dispatchResult)
                    {
                        ConsoleHelper.WriteError("GitHub API call to trigger build action failed.  Do you have " +
                            "Actions enabled on your forked Remotely repo on the Actions tab?  If not, enable them and try again. " +
                            "Otherwise, please check your input parameters.");
                        return;
                    }

                    ConsoleHelper.WriteLine("Build action triggered successfully.  Waiting for build completion.");

                    while (latestBuild?.id == latestBuildId)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1));
                        ConsoleHelper.WriteLine("Waiting for GitHub build completion.");
                        latestBuild = await _githubApi.GetLatestBuildArtifact(cliParams);
                    }
                }
                else if (latestBuild is null)
                {
                    ConsoleHelper.WriteError("There are no existing build artifacts, and --create-new was not specified.  Exiting.");
                    return;
                }

                var downloadResult = await _githubApi.DownloadArtifact(cliParams, latestBuild.archive_download_url, zipPath);

                if (!downloadResult)
                {
                    ConsoleHelper.WriteError("Downloading the build artifact was not successful.");
                    return;
                }

            }

            // Files in use can't be overwritten in Windows.  Stop the
            // website process first.
            if (cliParams.WebServer == WebServerType.IisWindows)
            {
                var w3wpProcs = Process.GetProcessesByName("w3wp");
                if (w3wpProcs.Length > 0)
                {
                    Process.Start("powershell.exe", "-Command & \"{ Stop-WebAppPool -Name Remotely -ErrorAction SilentlyContinue }\"").WaitForExit();
                    Process.Start("powershell.exe", "-Command & \"{ Stop-Website -Name Remotely -ErrorAction SilentlyContinue }\"").WaitForExit();

                    ConsoleHelper.WriteLine("Waiting for w3wp processes to close...");
                    foreach (var proc in w3wpProcs)
                    {
                        try { proc.Kill(); } 
                        catch { }
                    }
                    TaskHelper.DelayUntil(() => Process.GetProcessesByName("w3wp").Length < w3wpProcs.Length, TimeSpan.FromMinutes(5), 100);
                }
            }

            ConsoleHelper.WriteLine("Extracting files.");
            Directory.CreateDirectory(cliParams.InstallDirectory);
            ZipFile.ExtractToDirectory(zipPath, cliParams.InstallDirectory, true);

            await LaunchExternalInstaller(cliParams);
        }


        private async Task LaunchExternalInstaller(CliParams cliParams)
        {
            ConsoleHelper.WriteLine("Launching install script for selected reverse proxy type.");
            var resourcesPath = "Remotely.Server.Installer.Resources.";

            var fileName = cliParams.WebServer.Value switch
            {
                WebServerType.UbuntuCaddy => "Ubuntu_Caddy_Install.sh",
                WebServerType.UbuntuNginx => "Ubuntu_Nginx_Install.sh",
                WebServerType.CentOsCaddy => "CentOS_Caddy_Install.sh",
                WebServerType.CentOsNginx => "CentOS_Nginx_Install.sh",
                WebServerType.IisWindows => "IIS_Windows_Install.ps1",
                _ => throw new Exception("Unrecognized reverse proxy type."),
            };

            var resourcesFile = resourcesPath + fileName;
            var filePath = Path.Combine(Path.GetTempPath(), fileName);

            using (var mrs = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcesFile))
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await mrs.CopyToAsync(fileStream);
            }

            var scriptResult = false;
            ProcessStartInfo psi;

            if (cliParams.WebServer.Value == WebServerType.IisWindows)
            {
                psi = new ProcessStartInfo("powershell.exe")
                {
                    Arguments = $"-f \"{filePath}\" -AppPoolName Remotely -SiteName Remotely " +
                        $"-SitePath \"{cliParams.InstallDirectory}\" -HostName {cliParams.ServerUrl.Authority} -Quiet",
                    WorkingDirectory = cliParams.InstallDirectory
                };
            }
            else
            {
                Process.Start("sudo", $"chmod +x {filePath}").WaitForExit();
                psi = new ProcessStartInfo("sudo")
                {
                    Arguments = $"\"{filePath}\" --host {cliParams.ServerUrl.Authority} --approot {cliParams.InstallDirectory}",
                    WorkingDirectory = cliParams.InstallDirectory
                };
            }

            var proc = Process.Start(psi);

            scriptResult = await Task.Run(() => proc.WaitForExit((int)TimeSpan.FromMinutes(30).TotalMilliseconds));

            if (!scriptResult)
            {
                ConsoleHelper.WriteError("Installer script is taking longer than expected.");
            }
        }
    }
}
