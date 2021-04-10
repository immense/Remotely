using Remotely.Agent.Installer.Win.Models;
using Remotely.Agent.Installer.Win.Services;
using Remotely.Agent.Installer.Win.Utilities;
using Remotely.Shared.Utilities;
using Remotely.Shared.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Remotely.Agent.Installer.Win.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private BrandingInfo _brandingInfo;
        private bool _createSupportShortcut;
        private string _headerMessage = "Install the service.";

        private bool _isReadyState = true;
        private bool _isServiceInstalled;

        private string _organizationID;

        private int _progress;

        private string _serverUrl;

        private string _statusMessage;
        public MainWindowViewModel()
        {
            Installer = new InstallerService();

            CopyCommandLineArgs();

            ExtractDeviceInitInfo().Wait();

            AddExistingConnectionInfo();
        }

        public bool CreateSupportShortcut
        {
            get
            {
                return _createSupportShortcut;
            }
            set
            {
                _createSupportShortcut = value;
                FirePropertyChanged();
            }
        }

        public string HeaderMessage
        {
            get
            {
                return _headerMessage;
            }
            set
            {
                _headerMessage = value;
                FirePropertyChanged();
            }
        }

        public BitmapImage Icon { get; set; }
        public string InstallButtonText => IsServiceMissing ? "Install" : "Reinstall";

        public ICommand InstallCommand => new Executor(async (param) => { await Install(); });

        public bool IsProgressVisible => Progress > 0;

        public bool IsReadyState
        {
            get
            {
                return _isReadyState;
            }
            set
            {
                _isReadyState = value;
                FirePropertyChanged();
            }
        }

        public bool IsServiceInstalled
        {
            get
            {
                return _isServiceInstalled;
            }
            set
            {
                _isServiceInstalled = value;
                FirePropertyChanged();
                FirePropertyChanged(nameof(IsServiceMissing));
                FirePropertyChanged(nameof(InstallButtonText));
            }
        }

        public bool IsServiceMissing => !_isServiceInstalled;

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

        public string OrganizationID
        {
            get
            {
                return _organizationID;
            }
            set
            {
                _organizationID = value;
                FirePropertyChanged();
            }
        }

        public string ProductName { get; set; }

        public int Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                FirePropertyChanged();
                FirePropertyChanged(nameof(IsProgressVisible));
            }
        }

        public string ServerUrl
        {
            get
            {
                return _serverUrl;
            }
            set
            {
                _serverUrl = value;
                FirePropertyChanged();
            }
        }

        public string StatusMessage
        {
            get
            {
                return _statusMessage;
            }
            set
            {
                _statusMessage = value;
                FirePropertyChanged();
            }
        }

        public SolidColorBrush TitleBackgroundColor { get; set; }
        public SolidColorBrush TitleButtonForegroundColor { get; set; }
        public SolidColorBrush TitleForegroundColor { get; set; }
        public ICommand UninstallCommand => new Executor(async (param) => { await Uninstall(); });
        private string DeviceAlias { get; set; }
        private string DeviceGroup { get; set; }
        private string DeviceUuid { get; set; }
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
                HeaderMessage = $"Install the {ProductName} service.";
            }
            else
            {
                HeaderMessage = $"Modify the {ProductName} installation.";
            }

            CommandLineParser.VerifyArguments();

            if (CommandLineParser.CommandLineArgs.ContainsKey("install"))
            {
                await Install();
            }
            else if (CommandLineParser.CommandLineArgs.ContainsKey("uninstall"))
            {
                await Uninstall();
            }

            if (CommandLineParser.CommandLineArgs.ContainsKey("quiet"))
            {
                App.Current.Shutdown();
            }
        }

        private void AddExistingConnectionInfo()
        {
            try
            {
                var connectionInfoPath = Path.Combine(
               Path.GetPathRoot(Environment.SystemDirectory),
                   "Program Files",
                   "Remotely",
                   "ConnectionInfo.json");

                if (File.Exists(connectionInfoPath))
                {
                    var serializer = new JavaScriptSerializer();
                    var connectionInfo = serializer.Deserialize<ConnectionInfo>(File.ReadAllText(connectionInfoPath));

                    if (string.IsNullOrWhiteSpace(OrganizationID))
                    {
                        OrganizationID = connectionInfo.OrganizationID;
                    }

                    if (string.IsNullOrWhiteSpace(ServerUrl))
                    {
                        ServerUrl = connectionInfo.Host;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }

        }

        private void ApplyBranding(BrandingInfo brandingInfo)
        {
            try
            {
                ProductName = "Remotely";

                if (!string.IsNullOrWhiteSpace(brandingInfo?.Product))
                {
                    ProductName = brandingInfo.Product;
                }

                TitleBackgroundColor = new SolidColorBrush(Color.FromRgb(
                    brandingInfo?.TitleBackgroundRed ?? 70,
                    brandingInfo?.TitleBackgroundGreen ?? 70,
                    brandingInfo?.TitleBackgroundBlue ?? 70));

                TitleForegroundColor = new SolidColorBrush(Color.FromRgb(
                   brandingInfo?.TitleForegroundRed ?? 29,
                   brandingInfo?.TitleForegroundGreen ?? 144,
                   brandingInfo?.TitleForegroundBlue ?? 241));

                TitleButtonForegroundColor = new SolidColorBrush(Color.FromRgb(
                   brandingInfo?.ButtonForegroundRed ?? 255,
                   brandingInfo?.ButtonForegroundGreen ?? 255,
                   brandingInfo?.ButtonForegroundBlue ?? 255));

                TitleBackgroundColor.Freeze();
                TitleForegroundColor.Freeze();
                TitleButtonForegroundColor.Freeze();

                Icon = GetBitmapImageIcon(brandingInfo);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
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
            if (string.IsNullOrWhiteSpace(OrganizationID) || string.IsNullOrWhiteSpace(ServerUrl))
            {
                Logger.Write("ServerUrl or OrganizationID param is missing.  Unable to install.");
                MessageBoxEx.Show("Required settings are missing.  Please enter a server URL and organization ID.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Guid.TryParse(OrganizationID, out _))
            {
                Logger.Write("OrganizationID is not a valid GUID.");
                MessageBoxEx.Show("Organization ID must be a valid GUID.", "Invalid Organization ID", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Uri.TryCreate(ServerUrl, UriKind.Absolute, out var serverUri) ||
                (serverUri.Scheme != Uri.UriSchemeHttp && serverUri.Scheme != Uri.UriSchemeHttps))
            {
                Logger.Write("ServerUrl is not valid.");
                MessageBoxEx.Show("Server URL must be a valid Uri (e.g. https://app.remotely.one).", "Invalid Server URL", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void CopyCommandLineArgs()
        {
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

            if (CommandLineParser.CommandLineArgs.TryGetValue("deviceuuid", out var deviceUuid))
            {
                DeviceUuid = deviceUuid;
            }

            if (CommandLineParser.CommandLineArgs.ContainsKey("supportshortcut"))
            {
                CreateSupportShortcut = true;
            }

            if (ServerUrl?.EndsWith("/") == true)
            {
                ServerUrl = ServerUrl.Substring(0, ServerUrl.LastIndexOf("/"));
            }
        }

        private async Task ExtractDeviceInitInfo()
        {

            try
            {
                var fileName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
                var codeLength = AppConstants.RelayCodeLength + 2;

                for (var i = 0; i < fileName.Length; i++)
                {
                    var guid = string.Join("", fileName.Skip(i).Take(36));
                    if (Guid.TryParse(guid, out _))
                    {
                        OrganizationID = guid;
                        return;
                    }


                    var codeSection = string.Join("", fileName.Skip(i).Take(codeLength));

                    if (codeSection.StartsWith("[") &&
                        codeSection.EndsWith("]") && 
                        !string.IsNullOrWhiteSpace(ServerUrl))
                    {
                        var relayCode = codeSection.Substring(1, 4);
                        using (var httpClient = new HttpClient())
                        {
                            var response = await httpClient.GetAsync($"{ServerUrl}/api/relay/{relayCode}").ConfigureAwait(false);
                            if (response.IsSuccessStatusCode)
                            {
                                var serializer = new JavaScriptSerializer();
                                var organizationId = await response.Content.ReadAsStringAsync();
                                OrganizationID = organizationId;
                                break;
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(OrganizationID) &&
                    !string.IsNullOrWhiteSpace(ServerUrl))
                {
                    using (var httpClient = new HttpClient())
                    {
                        var serializer = new JavaScriptSerializer();
                        var brandingUrl = $"{ServerUrl.TrimEnd('/')}/api/branding/{OrganizationID}";
                        using (var response = await httpClient.GetAsync(brandingUrl).ConfigureAwait(false))
                        {
                            var responseString = await response.Content.ReadAsStringAsync();
                            _brandingInfo = serializer.Deserialize<BrandingInfo>(responseString);
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                ApplyBranding(_brandingInfo);
            }
        }
        private BitmapImage GetBitmapImageIcon(BrandingInfo bi)
        {
            Stream imageStream;
            if (!string.IsNullOrWhiteSpace(bi?.Icon))
            {
                imageStream = new MemoryStream(Convert.FromBase64String(bi.Icon));
            }
            else
            {
                imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Remotely.Agent.Installer.Win.Assets.Remotely_Icon.png");
            }

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = imageStream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            imageStream.Close();

            return bitmap;
        }
        private async Task Install()
        {
            try
            {
                IsReadyState = false;
                if (!CheckParams())
                {
                    return;
                }

                HeaderMessage = "Installing Remotely...";

                if (await Installer.Install(ServerUrl, OrganizationID, DeviceGroup, DeviceAlias, DeviceUuid, CreateSupportShortcut))
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

        private async Task Uninstall()
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
