#nullable enable
using Remotely.Agent.Installer.Win.Models;
using Remotely.Agent.Installer.Win.Services;
using Remotely.Agent.Installer.Win.Utilities;
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
using System.Net;
using Remotely.Shared;
using Remotely.Agent.Installer.Models;

namespace Remotely.Agent.Installer.Win.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly EmbeddedServerDataReader _embeddedDataReader;
    private readonly InstallerService _installer;
    private BrandingInfo? _brandingInfo;
    private bool _createSupportShortcut;
    private string _headerMessage = "Install the service.";
    private bool _isReadyState = true;
    private bool _isServiceInstalled;

    private string? _organizationID;

    private int _progress;

    private string _serverUrl = string.Empty;

    private string? _statusMessage;

    public MainWindowViewModel()
    {
        _installer = new InstallerService();
        _embeddedDataReader = new EmbeddedServerDataReader();

        CopyCommandLineArgs();

        ExtractEmbeddedServerData().Wait();

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

    public BitmapImage? Icon { get; set; }
    public string InstallButtonText => IsServiceMissing ? "Install" : "Reinstall";

    public ICommand InstallCommand => new RelayCommand(async (param) => { await Install(); });

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
            return new RelayCommand(param =>
            {
                
                if (Directory.Exists(Logger.LogsDir))
                {
                    Process.Start(Logger.LogsDir);
                }
                else
                {
                    MessageBoxEx.Show("Log directory doesn't exist.", "No Logs", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });
        }
    }

    public string? OrganizationID
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

    public string ProductName { get; set; } = "Remotely";

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
            _serverUrl = value?.TrimEnd('/') ?? string.Empty;
            FirePropertyChanged();
        }
    }

    public string? StatusMessage
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

    public ICommand UninstallCommand => new RelayCommand(async (param) => { await Uninstall(); });
    private string? DeviceAlias { get; set; }
    private string? DeviceGroup { get; set; }
    private string? DeviceUuid { get; set; }

    public async Task Init()
    {
        _installer.ProgressMessageChanged += (sender, arg) =>
        {
            StatusMessage = arg;
        };

        _installer.ProgressValueChanged += (sender, arg) =>
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
                    ServerUrl = connectionInfo.Host ?? string.Empty;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Write(ex);
        }

    }

    private void ApplyBranding(BrandingInfo? brandingInfo)
    {
        try
        {
            if (brandingInfo is not null &&
                !string.IsNullOrWhiteSpace(brandingInfo.Product))
            {
                ProductName = brandingInfo.Product;
            }

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
    }

    private async Task ExtractEmbeddedServerData()
    {

        try
        {
            var filePath = Process.GetCurrentProcess()?.MainModule?.FileName;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                Logger.Write("Failed to retrieve executing file name.");
                return;
            }

            var embeddedData = await _embeddedDataReader.TryGetEmbeddedData(filePath!);

            if (embeddedData is null || embeddedData == EmbeddedServerData.Empty)
            {
                Logger.Write("Embedded server data is empty.  Aborting.");
                return;
            }

            if (embeddedData.ServerUrl is null)
            {
                Logger.Write("ServerUrl is empty.  Aborting.");
                return;
            }

            OrganizationID = embeddedData.OrganizationId;
            ServerUrl = embeddedData.ServerUrl.AbsoluteUri;

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
        catch (Exception ex)
        {
            Logger.Write(ex);
        }
        finally
        {
            ApplyBranding(_brandingInfo);
        }
    }
    private BitmapImage GetBitmapImageIcon(BrandingInfo? bi)
    {
        Stream imageStream;
        if (bi is not null &&
            !string.IsNullOrWhiteSpace(bi.Icon))
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

            if (await _installer.Install(
                ServerUrl, 
                OrganizationID!, 
                DeviceGroup, 
                DeviceAlias, 
                DeviceUuid, 
                CreateSupportShortcut))
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

            if (await _installer.Uninstall())
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
