using Remotely.Shared.Utilities;
using Server.Installer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            var latestBuild = await _githubApi.GetLatestBuildArtifact(cliParams);
            var existingBuildTimestamp = latestBuild?.created_at;

            if (cliParams.CreateNew == true)
            {
                var dispatchResult = await _githubApi.TriggerDispatch(cliParams);

                if (!dispatchResult)
                {
                    ConsoleHelper.WriteError("GitHub API call to trigger build action failed.  Please check your input parameters.");
                    return;
                }

                ConsoleHelper.WriteLine("Build action triggered successfully.  Waiting for build completion.");

                while (latestBuild is null ||
                    latestBuild.created_at <= existingBuildTimestamp.Value)
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

            var filePath = Path.Combine(Path.GetTempPath(), "Remotely_Artifact.zip");

            var downloadResult = await _githubApi.DownloadArtifact(cliParams, latestBuild.archive_download_url, filePath);

            if (!downloadResult)
            {
                ConsoleHelper.WriteError("Downloading the build artifact was not successful.");
                return;
            }


            ConsoleHelper.WriteLine("Extracting artifact files.");
            if (Directory.Exists(cliParams.InstallDirectory))
            {
                Directory.Delete(cliParams.InstallDirectory, true);
            }
            Directory.CreateDirectory(cliParams.InstallDirectory);
            ZipFile.ExtractToDirectory(filePath, cliParams.InstallDirectory);

            await LaunchExternalInstaller(cliParams);
        }

        private async Task LaunchExternalInstaller(CliParams cliParams)
        {
            ConsoleHelper.WriteLine("Launching install script for selected reverse proxy type.");
            var resourcePath = "Remotely.Server.Installer.Resources.";

            resourcePath += cliParams.WebServer.Value switch
            {
                WebServerType.UbuntuCaddy => "Ubuntu_Caddy_Install.sh",
                WebServerType.UbuntuNginx => "Ubuntu_Nginx_Install.sh",
                WebServerType.CentOsCaddy => "CentOS_Caddy_Install.sh",
                WebServerType.CentOsNginx => "CentOS_Nginx_Install.sh",
                WebServerType.IisWindows => "IIS_Windows_Install.ps1",
                _ => throw new Exception("Unrecognized reverse proxy type."),
            };
            var fileName = resourcePath.Split(".").Last();
            var filePath = Path.Combine(Path.GetTempPath(), fileName);

            using (var mrs = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath))
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
                        $"-SitePath \"{cliParams.InstallDirectory}\" -Quiet",
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
