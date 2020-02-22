using Remotely.Agent.Installer.Win.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Remotely.Agent.Installer.Win.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string headerMessage;

        private bool isServiceInstalled;

        private string organizationName;

        private int progress;

        private string serverUrl;

        private string statusMessage;

        private string subMessage;

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

        public ICommand InstallCommand => new Executor(Install);

        public bool IsProgressVisible => Progress > 0;

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
                        MessageBox.Show("Log file doesn't exist.", "No Logs", MessageBoxButton.OK, MessageBoxImage.Information);
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

        public string SubMessage
        {
            get
            {
                return subMessage;
            }
            set
            {
                subMessage = value;
                FirePropertyChanged(nameof(SubMessage));
            }
        }

        public ICommand UninstallCommand => new Executor(async (param) => { await Uninstall(param); });
        private InstallerService Installer { get; }
        public async Task Init()
        {
            IsServiceInstalled = ServiceController.GetServices().Any(x => x.ServiceName == "Remotely_Service");
            if (IsServiceInstalled)
            {
                HeaderMessage = "Install the Remotely service.";
                SubMessage = "Installing the Remotely service will allow remote access by the above service provider.";
            }
            else
            {
                HeaderMessage = "Uninstall the Remotely service.";
                SubMessage = "Uninstalling the Remotely service will remove all remote acess for the above service provider.";
            }

            var installerSettings = ReadInstallerSettings();

            OrganizationName = installerSettings?.OrganizationName;
            ServerUrl = installerSettings?.ServerUrl;
            OrganizationID = installerSettings?.OrganizationID;

            if (Environment.GetCommandLineArgs().Contains("-rununinstaller"))
            {
                await Uninstall(null);
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
                    using (var settingsStream = new MemoryStream(payloadBytes))
                    {
                        settingsStream.Seek(0, SeekOrigin.Begin);
                        var serializer = new DataContractJsonSerializer(typeof(InstallerSettings));
                        var installerSettings = (InstallerSettings)serializer.ReadObject(settingsStream);
                        return installerSettings;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                MessageBox.Show("There was an error reading the installer settings.  Try re-downloading the installer.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Elevated privileges are required.  Please restart the installer using 'Run as administrator'.", "Elevation Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return result;
        }

        private bool CheckParams()
        {
            var result = !string.IsNullOrWhiteSpace(OrganizationID) && !string.IsNullOrWhiteSpace(ServerUrl);
            if (!result)
            {
                MessageBox.Show("Required settings are missing.  Try re-downloading the installer.", "Invalid Installer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return result;
        }

        private void Install(object param)
        {
            try
            {
                if (!CheckParams())
                {
                    return;
                }
                
                
                if (Installer.Install())
                {
                    IsServiceInstalled = true;
                    Progress = 0;
                    HeaderMessage = "Installation completed.";
                    SubMessage = "Remotely has been installed.  You can now close this window.";
                }
                else
                {
                    Progress = 0;
                    HeaderMessage = "An error occurred during installation.";
                    SubMessage = "There was an error during installation.  Check the logs for details.";
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
           
        }
        private async Task Uninstall(object param)
        {
            try
            {
                if (!CheckParams())
                {
                    return;
                }
               
                if (await Installer.Uninstall())
                {
                    IsServiceInstalled = false;
                    Progress = 0;
                    HeaderMessage = "Uninstall completed.";
                    SubMessage = "Remotely has been uninstalled.  You can now close this window.";
                }
                else
                {
                    Progress = 0;
                    HeaderMessage = "An error occurred during uninstall.";
                    SubMessage = "There was an error during uninstall.  Check the logs for details.";
                }

            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
    }
}
