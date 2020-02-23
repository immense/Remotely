using Remotely.Agent.Installer.Win.Services;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Input;

namespace Remotely.Agent.Installer.Win.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string headerMessage;

        private bool isReadyState = true;
        private bool isServiceInstalled;

        private string organizationName;

        private int progress;

        private string serverUrl;

        private string statusMessage;

        public MainWindowViewModel()
        {
            Installer = new InstallerService();
        }
        public string HeaderMessage
        {
            get
            {
                return headerMessage;
            }
            set
            {
                headerMessage = value;
                FirePropertyChanged(nameof(HeaderMessage));
            }
        }

        public ICommand InstallCommand => new Executor(async (param) => { await Install(param); });

        public bool IsProgressVisible => Progress > 0;

        public bool IsReadyState
        {
            get
            {
                return isReadyState;
            }
            set
            {
                isReadyState = value;
                FirePropertyChanged(nameof(IsReadyState));
            }
        }

        public bool IsServiceInstalled
        {
            get
            {
                return isServiceInstalled;
            }
            set
            {
                isServiceInstalled = value;
                FirePropertyChanged(nameof(IsServiceInstalled));
                FirePropertyChanged(nameof(IsServiceMissing));
            }
        }

        public bool IsServiceMissing => !isServiceInstalled;

        public ICommand OpenLogsCommand
        {
            get
            {
                return new Executor(param =>
                {
                    var logPath = Path.Combine(Path.GetTempPath(), "Remotely_Installer.log");
                    if (File.Exists(logPath))
                    {
                        Process.Start(logPath);
                    }
                    else
                    { 
                        MessageBoxEx.Show("Log file doesn't exist.", "No Logs", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                });
            }
        }
        public string OrganizationID { get; set; }
        public string OrganizationName
        {
            get
            {
                return organizationName;
            }
            set
            {
                organizationName = value;
                FirePropertyChanged(nameof(OrganizationName));
            }
        }

        public int Progress
        {
            get
            {
                return progress;
            }
            set
            {
                progress = value;
                FirePropertyChanged(nameof(Progress));
                FirePropertyChanged(nameof(IsProgressVisible));
            }
        }

        public string ServerUrl
        {
            get
            {
                return serverUrl;
            }
            set
            {
                serverUrl = value;
                FirePropertyChanged(nameof(ServerUrl));
            }
        }

        public string StatusMessage
        {
            get
            {
                return statusMessage;
            }
            set
            {
                statusMessage = value;
                FirePropertyChanged(nameof(StatusMessage));
            }
        }

        public ICommand UninstallCommand => new Executor(async (param) => { await Uninstall(param); });

        private string DeviceAlias { get; set; }
        private string DeviceGroup { get; set; }
        private InstallerService Installer { get; }
        public async Task Init()
        {

            Installer.ProgressMessageChanged += (sender, arg) =>
            {
                StatusMessage = arg;
            };

            Installer.ProgressValueChanged += (sender, arg) =>
            {
                Progress = arg;
            };

            IsServiceInstalled = ServiceController.GetServices().Any(x => x.ServiceName == "Remotely_Service");
            if (IsServiceMissing)
            {
                HeaderMessage = "Install the Remotely service.";
                StatusMessage = "Installing the Remotely service will allow remote access by the above service provider.";
            }
            else
            {
                HeaderMessage = "Uninstall the Remotely service.";
                StatusMessage = "Uninstalling the Remotely service will remove all remote acess to this device.";
            }

            var installerSettings = ReadInstallerSettings();

            OrganizationName = installerSettings?.OrganizationName;
            ServerUrl = installerSettings?.ServerUrl;
            OrganizationID = installerSettings?.OrganizationID;

            CopyCommandLineArgs();

            if (CommandLineParser.CommandLineArgs.ContainsKey("install"))
            {
                await Install(null);
            }
            else if (CommandLineParser.CommandLineArgs.ContainsKey("uninstall"))
            {
                await Uninstall(null);
            }
            else
            {
                CheckParams();
            }

            if (CommandLineParser.CommandLineArgs.ContainsKey("quiet"))
            {
                App.Current.Shutdown();
            }
        }

        public InstallerSettings ReadInstallerSettings()
        {
            try
            {
                var fileBytes = File.ReadAllBytes(Assembly.GetExecutingAssembly().Location);
                using (var peStream = new MemoryStream(fileBytes))
                using (var br = new BinaryReader(peStream))
                {
                    peStream.Seek(-4, SeekOrigin.End);
                    var payloadSize = br.ReadInt32();
                    peStream.Seek(-4 - payloadSize, SeekOrigin.End);
                    var payloadBytes = br.ReadBytes(payloadSize);
                    var payloadJson = Encoding.UTF8.GetString(payloadBytes);
                    var serializer = new JavaScriptSerializer();
                    var installerSettings = serializer.Deserialize<InstallerSettings>(payloadJson);
                    return installerSettings;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                MessageBoxEx.Show("There was an error reading the installer settings.  Try re-downloading the installer.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
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

        private bool CheckParams()
        {
            var result = !string.IsNullOrWhiteSpace(OrganizationID) || !string.IsNullOrWhiteSpace(ServerUrl);
            if (!result)
            {
                MessageBoxEx.Show("Required settings are missing.  Try re-downloading the installer.", "Invalid Installer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return result;
        }

        private void CopyCommandLineArgs()
        {
            if (CommandLineParser.CommandLineArgs.TryGetValue("organization", out var orgName))
            {
                OrganizationName = orgName;
            }

            if (CommandLineParser.CommandLineArgs.TryGetValue("organizationid", out var orgID))
            {
                OrganizationID = orgID;
            }

            if (CommandLineParser.CommandLineArgs.TryGetValue("serverurl", out var serverUrl))
            {
                ServerUrl = serverUrl;
            }

            if (CommandLineParser.CommandLineArgs.TryGetValue("devicegroup", out var deviceGroup))
            {
                DeviceGroup = deviceGroup;
            }

            if (CommandLineParser.CommandLineArgs.TryGetValue("devicealias", out var deviceAlias))
            {
                DeviceAlias = deviceAlias;
            }

            if (ServerUrl?.EndsWith("/") == true)
            {
                ServerUrl = ServerUrl.Substring(0, ServerUrl.LastIndexOf("/"));
            }
        }
        private async Task Install(object param)
        {
            try
            {
                IsReadyState = false;
                if (!CheckParams())
                {
                    return;
                }

                HeaderMessage = "Installing Remotely...";
                
                if (await Installer.Install(ServerUrl, OrganizationID, DeviceGroup, DeviceAlias))
                {
                    IsServiceInstalled = true;
                    Progress = 0;
                    HeaderMessage = "Installation completed.";
                    StatusMessage = "Remotely has been installed.  You can now close this window.";
                }
                else
                {
                    Progress = 0;
                    HeaderMessage = "An error occurred during installation.";
                    StatusMessage = "There was an error during installation.  Check the logs for details.";
                }
                if (!CheckIsAdministrator())
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                IsReadyState = true;
            }
        }

       

        private async Task Uninstall(object param)
        {
            try
            {
                IsReadyState = false;

                HeaderMessage = "Uninstalling Remotely...";

                if (await Installer.Uninstall())
                {
                    IsServiceInstalled = false;
                    Progress = 0;
                    HeaderMessage = "Uninstall completed.";
                    StatusMessage = "Remotely has been uninstalled.  You can now close this window.";
                }
                else
                {
                    Progress = 0;
                    HeaderMessage = "An error occurred during uninstall.";
                    StatusMessage = "There was an error during uninstall.  Check the logs for details.";
                }

            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                IsReadyState = true;
            }
        }
    }
}
