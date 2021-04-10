using IWshRuntimeLibrary;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using Remotely.Agent.Installer.Win.Utilities;
using Remotely.Shared.Models;
using System;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using FileIO = System.IO.File;

namespace Remotely.Agent.Installer.Win.Services
{
    public class InstallerService
    {
        public event EventHandler<string> ProgressMessageChanged;
        public event EventHandler<int> ProgressValueChanged;

        private string InstallPath => Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), "Program Files", "Remotely");
        private string Platform => Environment.Is64BitOperatingSystem ? "x64" : "x86";
        private JavaScriptSerializer Serializer { get; } = new JavaScriptSerializer();
        public async Task<bool> Install(string serverUrl,
            string organizationId,
            string deviceGroup,
            string deviceAlias,
            string deviceUuid,
            bool createSupportShortcut)
        {
            try
            {
                Logger.Write("Install started.");
                if (!CheckIsAdministrator())
                {
                    return false;
                }

                StopService();

                await StopProcesses();

                BackupDirectory();

                var connectionInfo = GetConnectionInfo(organizationId, serverUrl, deviceUuid);

                ClearInstallDirectory();

                await DownloadRemotelyAgent(serverUrl);

                FileIO.WriteAllText(Path.Combine(InstallPath, "ConnectionInfo.json"), Serializer.Serialize(connectionInfo));

                FileIO.Copy(Assembly.GetExecutingAssembly().Location, Path.Combine(InstallPath, "Remotely_Installer.exe"));

                await CreateDeviceOnServer(connectionInfo.DeviceID, serverUrl, deviceGroup, deviceAlias, organizationId);

                AddFirewallRule();

                InstallService();

                CreateUninstallKey();

                CreateSupportShortcut(serverUrl, connectionInfo.DeviceID, createSupportShortcut);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                RestoreBackup();
                return false;
            }

        }

        public async Task<bool> Uninstall()
        {
            try
            {
                if (!CheckIsAdministrator())
                {
                    return false;
                }

                StopService();

                ProcessEx.StartHidden("cmd.exe", "/c sc delete Remotely_Service").WaitForExit();

                await StopProcesses();

                ProgressMessageChanged?.Invoke(this, "Deleting files.");
                ClearInstallDirectory();
                ProcessEx.StartHidden("cmd.exe", $"/c timeout 5 & rd /s /q \"{InstallPath}\"");

                ProcessEx.StartHidden("netsh", "advfirewall firewall delete rule name=\"Remotely Desktop Unattended\"").WaitForExit();

                GetRegistryBaseKey().DeleteSubKeyTree(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Remotely", false);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        private void AddFirewallRule()
        {
            var desktopExePath = Path.Combine(InstallPath, "Desktop", "Remotely_Desktop.exe");
            ProcessEx.StartHidden("netsh", "advfirewall firewall delete rule name=\"Remotely Desktop Unattended\"").WaitForExit();
            ProcessEx.StartHidden("netsh", $"advfirewall firewall add rule name=\"Remotely Desktop Unattended\" program=\"{desktopExePath}\" protocol=any dir=in enable=yes action=allow description=\"The agent that allows screen sharing and remote control for Remotely.\"").WaitForExit();
        }

        private void BackupDirectory()
        {
            if (Directory.Exists(InstallPath))
            {
                Logger.Write("Backing up current installation.");
                ProgressMessageChanged?.Invoke(this, "Backing up current installation.");
                var backupPath = Path.Combine(Path.GetTempPath(), "Remotely_Backup.zip");
                if (FileIO.Exists(backupPath))
                {
                    FileIO.Delete(backupPath);
                }
                ZipFile.CreateFromDirectory(InstallPath, backupPath, CompressionLevel.Fastest, false);
            }
        }

        private bool CheckIsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            var result = principal.IsInRole(WindowsBuiltInRole.Administrator);
            if (!result)
            {
                MessageBoxEx.Show("Elevated privileges are required.  Please restart the installer using 'Run as administrator'.", "Elevation Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return result;
        }

        private void ClearInstallDirectory()
        {
            if (Directory.Exists(InstallPath))
            {
                foreach (var entry in Directory.GetFileSystemEntries(InstallPath))
                {
                    try
                    {
                        if (FileIO.Exists(entry))
                        {
                            FileIO.Delete(entry);
                        }
                        else if (Directory.Exists(entry))
                        {
                            Directory.Delete(entry, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex);
                    }
                }
            }
        }

        private async Task CreateDeviceOnServer(string deviceUuid,
            string serverUrl,
            string deviceGroup,
            string deviceAlias,
            string organizationId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(deviceGroup) ||
                    !string.IsNullOrWhiteSpace(deviceAlias))
                {
                    var setupOptions = new DeviceSetupOptions()
                    {
                        DeviceID = deviceUuid,
                        DeviceGroupName = deviceGroup,
                        DeviceAlias = deviceAlias,
                        OrganizationID = organizationId
                    };

                    var wr = WebRequest.CreateHttp(serverUrl.TrimEnd('/') + "/api/devices");
                    wr.Method = "POST";
                    wr.ContentType = "application/json";
                    using (var rs = await wr.GetRequestStreamAsync())
                    using (var sw = new StreamWriter(rs))
                    {
                        await sw.WriteAsync(Serializer.Serialize(setupOptions));
                    }
                    using (var response = await wr.GetResponseAsync() as HttpWebResponse)
                    { 
                        Logger.Write($"Create device response: {response.StatusCode}");
                    }
                }
            }
            catch (WebException ex) when ((ex.Response is HttpWebResponse response) && response.StatusCode == HttpStatusCode.BadRequest)
            {
                Logger.Write("Bad request when creating device.  The device ID may already be created.");
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }

        }

        private void CreateSupportShortcut(string serverUrl, string deviceUuid, bool createSupportShortcut)
        {
            var shell = new WshShell();
            var shortcutLocation = Path.Combine(InstallPath, "Get Support.lnk");
            var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);
            shortcut.Description = "Get IT support";
            shortcut.IconLocation = Path.Combine(InstallPath, "Remotely_Agent.exe");
            shortcut.TargetPath = serverUrl.TrimEnd('/') + $"/GetSupport?deviceID={deviceUuid}";
            shortcut.Save();

            if (createSupportShortcut)
            {
                var systemRoot = Path.GetPathRoot(Environment.SystemDirectory);
                var publicDesktop = Path.Combine(systemRoot, "Users", "Public", "Desktop", "Get Support.lnk");
                FileIO.Copy(shortcutLocation, publicDesktop, true);
            }
        }
        private void CreateUninstallKey()
        {
            var version = FileVersionInfo.GetVersionInfo(Path.Combine(InstallPath, "Remotely_Agent.exe"));
            var baseKey = GetRegistryBaseKey();

            var remotelyKey = baseKey.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Remotely", true);
            remotelyKey.SetValue("DisplayIcon", Path.Combine(InstallPath, "Remotely_Agent.exe"));
            remotelyKey.SetValue("DisplayName", "Remotely");
            remotelyKey.SetValue("DisplayVersion", version.FileVersion);
            remotelyKey.SetValue("InstallDate", DateTime.Now.ToShortDateString());
            remotelyKey.SetValue("Publisher", "Translucency Software");
            remotelyKey.SetValue("VersionMajor", version.FileMajorPart.ToString(), RegistryValueKind.DWord);
            remotelyKey.SetValue("VersionMinor", version.FileMinorPart.ToString(), RegistryValueKind.DWord);
            remotelyKey.SetValue("UninstallString", Path.Combine(InstallPath, "Remotely_Installer.exe -uninstall -quiet"));
            remotelyKey.SetValue("QuietUninstallString", Path.Combine(InstallPath, "Remotely_Installer.exe -uninstall -quiet"));
        }

        private async Task DownloadRemotelyAgent(string serverUrl)
        {
            var targetFile = Path.Combine(Path.GetTempPath(), $"Remotely-Agent.zip");

            if (CommandLineParser.CommandLineArgs.TryGetValue("path", out var result) &&
                FileIO.Exists(result))
            {
                targetFile = result;
            }
            else
            {
                ProgressMessageChanged.Invoke(this, "Downloading Remotely agent.");
                using (var client = new WebClient())
                {
                    client.DownloadProgressChanged += (sender, args) =>
                    {
                        ProgressValueChanged?.Invoke(this, args.ProgressPercentage);
                    };

                    await client.DownloadFileTaskAsync($"{serverUrl}/Content/Remotely-Win10-{Platform}.zip", targetFile);
                }
            }

            ProgressMessageChanged.Invoke(this, "Extracting Remotely files.");
            ProgressValueChanged?.Invoke(this, 0);

            var tempDir = Path.Combine(Path.GetTempPath(), "RemotelyUpdate");
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }

            Directory.CreateDirectory(InstallPath);
            while (!Directory.Exists(InstallPath))
            {
                await Task.Delay(10);
            }

            var wr = WebRequest.CreateHttp($"{serverUrl}/Content/Remotely-Win10-{Platform}.zip");
            wr.Method = "Head";
            using (var response = (HttpWebResponse)await wr.GetResponseAsync())
            {
                FileIO.WriteAllText(Path.Combine(InstallPath, "etag.txt"), response.Headers["ETag"]);
            }

            ZipFile.ExtractToDirectory(targetFile, tempDir);
            var fileSystemEntries = Directory.GetFileSystemEntries(tempDir);
            for (var i = 0; i < fileSystemEntries.Length; i++)
            {
                try
                {
                    ProgressValueChanged?.Invoke(this, (int)((double)i / (double)fileSystemEntries.Length * 100d));
                    var entry = fileSystemEntries[i];
                    if (FileIO.Exists(entry))
                    {
                        FileIO.Copy(entry, Path.Combine(InstallPath, Path.GetFileName(entry)), true);
                    }
                    else if (Directory.Exists(entry))
                    {
                        FileSystem.CopyDirectory(entry, Path.Combine(InstallPath, new DirectoryInfo(entry).Name), true);
                    }
                    await Task.Delay(1);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }
            ProgressValueChanged?.Invoke(this, 0);
        }

        private ConnectionInfo GetConnectionInfo(string organizationId, string serverUrl, string deviceUuid)
        {
            ConnectionInfo connectionInfo;
            var connectionInfoPath = Path.Combine(InstallPath, "ConnectionInfo.json");
            if (FileIO.Exists(connectionInfoPath))
            {
                connectionInfo = Serializer.Deserialize<ConnectionInfo>(FileIO.ReadAllText(connectionInfoPath));
                connectionInfo.ServerVerificationToken = null;
            }
            else
            {
                connectionInfo = new ConnectionInfo()
                {
                    DeviceID = Guid.NewGuid().ToString()
                };
            }

            if (!string.IsNullOrWhiteSpace(deviceUuid))
            {
                // Clear the server verification token if we're installing this as a new device.
                if (connectionInfo.DeviceID != deviceUuid)
                {
                    connectionInfo.ServerVerificationToken = null;
                }
                connectionInfo.DeviceID = deviceUuid;
            }
            connectionInfo.OrganizationID = organizationId;
            connectionInfo.Host = serverUrl;
            return connectionInfo;
        }

        private RegistryKey GetRegistryBaseKey()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            }
            else
            {
                return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            }
        }

        private void InstallService()
        {
            Logger.Write("Installing service.");
            ProgressMessageChanged?.Invoke(this, "Installing Remotely service.");
            var serv = ServiceController.GetServices().FirstOrDefault(ser => ser.ServiceName == "Remotely_Service");
            if (serv == null)
            {
                var command = new string[] { "/assemblypath=" + Path.Combine(InstallPath, "Remotely_Agent.exe") };
                var context = new InstallContext("", command);
                var serviceInstaller = new ServiceInstaller()
                {
                    Context = context,
                    DisplayName = "Remotely Service",
                    Description = "Background service that maintains a connection to the Remotely server.  The service is used for remote support and maintenance by this computer's administrators.",
                    ServiceName = "Remotely_Service",
                    StartType = ServiceStartMode.Automatic,
                    DelayedAutoStart = true,
                    Parent = new ServiceProcessInstaller()
                };

                var state = new System.Collections.Specialized.ListDictionary();
                serviceInstaller.Install(state);
                Logger.Write("Service installed.");
                serv = ServiceController.GetServices().FirstOrDefault(ser => ser.ServiceName == "Remotely_Service");

                ProcessEx.StartHidden("cmd.exe", "/c sc.exe failure \"Remotely_Service\" reset= 5 actions= restart/5000");
            }
            if (serv.Status != ServiceControllerStatus.Running)
            {
                serv.Start();
            }
            Logger.Write("Service started.");
        }

        private void RestoreBackup()
        {
            try
            {
                var backupPath = Path.Combine(Path.GetTempPath(), "Remotely_Backup.zip");
                if (FileIO.Exists(backupPath))
                {
                    Logger.Write("Restoring backup.");
                    ClearInstallDirectory();
                    ZipFile.ExtractToDirectory(backupPath, InstallPath);
                    var serv = ServiceController.GetServices().FirstOrDefault(ser => ser.ServiceName == "Remotely_Service");
                    if (serv?.Status != ServiceControllerStatus.Running)
                    {
                        serv?.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private async Task StopProcesses()
        {
            ProgressMessageChanged?.Invoke(this, "Stopping Remotely processes.");
            var procs = Process.GetProcessesByName("Remotely_Agent").Concat(Process.GetProcessesByName("Remotely_Desktop"));

            foreach (var proc in procs)
            {
                proc.Kill();
            }

            await Task.Delay(500);
        }
        private void StopService()
        {
            try
            {
                var remotelyService = ServiceController.GetServices().FirstOrDefault(x => x.ServiceName == "Remotely_Service");
                if (remotelyService != null)
                {
                    Logger.Write("Stopping existing Remotely service.");
                    ProgressMessageChanged?.Invoke(this, "Stopping existing Remotely service.");
                    remotelyService.Stop();
                    remotelyService.WaitForStatus(ServiceControllerStatus.Stopped);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
    }
}
