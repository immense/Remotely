using Remotely.Shared.Utilities;
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
            var filePath = Path.Combine(Path.GetTempPath(), "Remotely_Server.zip");

            if (cliParams.UsePrebuiltPackage == true)
            {
                ConsoleHelper.WriteLine("Downloading pre-built server package.");

                int progress = 0;

                var releaseFile = cliParams.WebServer == WebServerType.IisWindows ?
                    "https://github.com/lucent-sea/Remotely/releases/latest/download/Remotely_Server_Win-x64.zip" :
                    "https://github.com/lucent-sea/Remotely/releases/latest/download/Remotely_Server_Linux-x64.zip";

                using var webClient = new WebClient();
                webClient.DownloadProgressChanged += (sender, args) =>
                {
                    var newProgress = args.ProgressPercentage / 5 * 5;
                    if (newProgress != progress)
                    {
                        progress = newProgress;
                        ConsoleHelper.WriteLine($"Progress: {progress}%");
                    }
                };
                await webClient.DownloadFileTaskAsync(releaseFile, filePath);
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

                var downloadResult = await _githubApi.DownloadArtifact(cliParams, latestBuild.archive_download_url, filePath);

                if (!downloadResult)
                {
                    ConsoleHelper.WriteError("Downloading the build artifact was not successful.");
                    return;
                }

            }


            ConsoleHelper.WriteLine("Extracting files.");
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
