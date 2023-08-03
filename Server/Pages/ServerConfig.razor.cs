using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remotely.Server.Components;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using Remotely.Shared.Entities;
using Remotely.Shared.Enums;
using Remotely.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.Pages;

public class AppSettingsModel
{
    [Display(Name = "Allow API Login")]
    public bool AllowApiLogin { get; set; }

    [Display(Name = "Banned Devices")]
    public List<string> BannedDevices { get; set; } = new();

    [Display(Name = "Data Retention (days)")]
    public double DataRetentionInDays { get; set; }

    [Display(Name = "Database Provider")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DbProvider DBProvider { get; set; }

    [Display(Name = "Enable Remote Control Recording")]
    public bool EnableRemoteControlRecording { get; set; }

    [Display(Name = "Enable Windows Event Log")]
    public bool EnableWindowsEventLog { get; set; }

    [Display(Name = "Enforce Attended Access")]
    public bool EnforceAttendedAccess { get; set; }

    [Display(Name = "Force Client HTTPS")]
    public bool ForceClientHttps { get; set; }

    [Display(Name = "Known Proxies")]
    public List<string> KnownProxies { get; set; } = new();

    [Display(Name = "Max Concurrent Updates")]
    public int MaxConcurrentUpdates { get; set; }

    [Display(Name = "Max Organizations")]
    public int MaxOrganizationCount { get; set; }
    [Display(Name = "Message of the Day")]
    public string? MessageOfTheDay { get; set; }

    [Display(Name = "Redirect To HTTPS")]
    public bool RedirectToHttps { get; set; }

    [Display(Name = "Remote Control Notify User")]
    public bool RemoteControlNotifyUser { get; set; }

    [Display(Name = "Remote Control Requires Authentication")]
    public bool RemoteControlRequiresAuthentication { get; set; }

    [Display(Name = "Remote Control Session Limit")]
    public double RemoteControlSessionLimit { get; set; }

    [Display(Name = "Require 2FA")]
    public bool Require2FA { get; set; }

    [Display(Name = "SMTP Display Name")]
    public string? SmtpDisplayName { get; set; }

    [Display(Name = "SMTP Email")]
    [EmailAddress]
    public string? SmtpEmail { get; set; }

    [Display(Name = "SMTP Host")]
    public string? SmtpHost { get; set; }
    [Display(Name = "SMTP Local Domain")]
    public string? SmtpLocalDomain { get; set; }

    [Display(Name = "SMTP Check Certificate Revocation")]
    public bool SmtpCheckCertificateRevocation { get; set; }

    [Display(Name = "SMTP Password")]
    public string? SmtpPassword { get; set; }

    [Display(Name = "SMTP Port")]
    public int SmtpPort { get; set; }

    [Display(Name = "SMTP Username")]
    public string? SmtpUserName { get; set; }

    [Display(Name = "Theme")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Theme Theme { get; set; }

    [Display(Name = "Trusted CORS Origins")]
    public List<string> TrustedCorsOrigins { get; set; } = new();

    [Display(Name = "Use HSTS")]
    public bool UseHsts { get; set; }

    [Display(Name = "Use HTTP Logging")]
    public bool UseHttpLogging { get; set; }
}

public class ConnectionStringsModel
{
    [Display(Name = "PostgreSQL")]
    public string? PostgreSQL { get; set; }

    [Display(Name = "SQLite")]
    public string? SQLite { get; set; }

    [Display(Name = "SQL Server")]
    public string? SQLServer { get; set; }
}

public partial class ServerConfig : AuthComponentBase
{
    private string? _alertMessage;
    private string? _bannedDeviceSelected;
    private string? _bannedDeviceToAdd;

    private string? _knownProxySelected;
    private string? _knownProxyToAdd;

    private bool _showMyOrgAdminsOnly = true;
    private bool _showAdminsOnly;

    private string? _trustedCorsOriginSelected;
    private string? _trustedCorsOriginToAdd;

    private readonly List<RemotelyUser> _userList = new();


    [Inject]
    private IHubContext<AgentHub, IAgentHubClient> AgentHubContext { get; init; } = null!;

    [Inject]
    private IConfiguration Configuration { get; init; } = null!;

    private ConnectionStringsModel ConnectionStrings { get; } = new();

    [Inject]
    private IDataService DataService { get; init; } = null!;

    [Inject]
    private IEmailSenderEx EmailSender { get; init; } = null!;

    [Inject]
    private IWebHostEnvironment HostEnv { get; init; } = null!;

    [Inject]
    private ILogger<ServerConfig> Logger { get; init; } = null!;

    [Inject]
    private IAgentHubSessionCache ServiceSessionCache { get; init; } = null!;

    private AppSettingsModel Input { get; } = new();

    [Inject]
    private IModalService ModalService { get; init; } = null!;

    [Inject]
    private IUpgradeService UpgradeService { get; init; } = null!;

    [Inject]
    private ICircuitManager CircuitManager { get; init; } = null!;

    private IEnumerable<string> OutdatedDevices => GetOutdatedDevices();

    [Inject]
    private IToastService ToastService { get; init; } = null!;

    private int TotalDevices => DataService.GetTotalDevices();

    private IEnumerable<RemotelyUser> UserList
    {
        get
        {
            if (User is null)
            {
                return Enumerable.Empty<RemotelyUser>();
            }

            EnsureUserSet();

            return _userList.Where(x =>
                (!_showAdminsOnly || x.IsServerAdmin) &&
                (!_showMyOrgAdminsOnly || x.OrganizationID == User.OrganizationID));
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        EnsureUserSet();
        if (!User.IsServerAdmin)
        {
            return;
        }

        Configuration.Bind("ApplicationOptions", Input);
        Configuration.Bind("ConnectionStrings", ConnectionStrings);
        _userList.AddRange(DataService.GetAllUsersForServer().OrderBy(x => x.UserName));
    }

    private void AddBannedDevice()
    {
        if (string.IsNullOrWhiteSpace(_bannedDeviceToAdd))
        {
            return;
        }

        Input.BannedDevices.Add(_bannedDeviceToAdd);
        _bannedDeviceToAdd = string.Empty;
    }

    private void AddKnownProxy()
    {
        if (string.IsNullOrWhiteSpace(_knownProxyToAdd))
        {
            return;
        }

        Input.KnownProxies.Add(_knownProxyToAdd);
        _knownProxyToAdd = string.Empty;
    }

    private void AddTrustedCorsOrigin()
    {
        if (string.IsNullOrWhiteSpace(_trustedCorsOriginToAdd))
        {
            return;
        }

        Input.TrustedCorsOrigins.Add(_trustedCorsOriginToAdd);
        _trustedCorsOriginToAdd = null;
    }

    private IEnumerable<string> GetOutdatedDevices()
    {
        try
        {
            var currentVersion = UpgradeService.GetCurrentVersion();

            return ServiceSessionCache.GetAllDevices()
                .Where(x => Version.TryParse(x.AgentVersion, out var result) && result < currentVersion)
                .Select(x => x.ID);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error while getting outdated devices.");
        }

        return Enumerable.Empty<string>();
    }

    private void HandleBannedDeviceKeyDown(KeyboardEventArgs args)
    {
        if (args.Key.Equals("Enter", StringComparison.OrdinalIgnoreCase))
        {
            AddBannedDevice();
        }
    }

    private void HandleKnownProxyKeyDown(KeyboardEventArgs args)
    {
        if (args.Key.Equals("Enter", StringComparison.OrdinalIgnoreCase))
        {
            AddKnownProxy();
        }
    }

    private void HandleTrustedCorsKeyDown(KeyboardEventArgs args)
    {
        if (args.Key.Equals("Enter", StringComparison.OrdinalIgnoreCase))
        {
            AddTrustedCorsOrigin();
        }
    }
    private async Task HandleValidSubmit(EditContext context)
    {
        await Save();
    }

    private void RemoveBannedDevice()
    {
        if (string.IsNullOrWhiteSpace(_bannedDeviceSelected))
        {
            return;
        }

        Input.BannedDevices.Remove(_bannedDeviceSelected);
        _bannedDeviceSelected = null;
    }

    private void RemoveKnownProxy()
    {
        if (string.IsNullOrWhiteSpace(_knownProxySelected))
        {
            return;
        }
        Input.KnownProxies.Remove(_knownProxySelected);
        _knownProxySelected = null;
    }

    private void RemoveTrustedCorsOrigin()
    {
        if (string.IsNullOrWhiteSpace(_trustedCorsOriginSelected))
        {
            return;
        }

        Input.TrustedCorsOrigins.Remove(_trustedCorsOriginSelected);
        _trustedCorsOriginSelected = null;
    }

    private async Task Save()
    {
        var resetEvent = new ManualResetEventSlim();

        Configuration.GetReloadToken().RegisterChangeCallback((e) =>
        {
            resetEvent.Set();
        }, null);

        await SaveInputToAppSettings();

        resetEvent.Wait(5_000);

        ToastService.ShowToast("Configuration saved.");
        _alertMessage = "Configuration saved.";
    }

    private async Task SaveAndTestSmtpSettings()
    {
        EnsureUserSet();
        await SaveInputToAppSettings();
        if (string.IsNullOrWhiteSpace(User.Email))
        {
            ToastService.ShowToast2("User email is not set.", Enums.ToastType.Warning);
            return;
        }

        var success = await EmailSender.SendEmailAsync(User.Email, "Remotely Test Email", "Congratulations! Your SMTP settings are working!", User.OrganizationID);
        if (success)
        {
            ToastService.ShowToast($"Test email sent to {User.Email}.  Check your inbox (or spam folder).");
            _alertMessage = $"Test email sent to {User.Email}.  Check your inbox (or spam folder).";
        }
        else
        {
            ToastService.ShowToast("Error sending email.  Check the server logs for details.", classString: "bg-danger");
            _alertMessage = "Error sending email.  Check the server logs for details.";
        }
    }

    private async Task SaveInputToAppSettings()
    {
        string savePath;
        var prodSettings = HostEnv.ContentRootFileProvider.GetFileInfo("appsettings.Production.json");
        var stagingSettings = HostEnv.ContentRootFileProvider.GetFileInfo("appsettings.Staging.json");
        var devSettings = HostEnv.ContentRootFileProvider.GetFileInfo("appsettings.Development.json");
        var settings = HostEnv.ContentRootFileProvider.GetFileInfo("appsettings.json");

        if (HostEnv.IsProduction() 
            && prodSettings.Exists && 
            !string.IsNullOrWhiteSpace(prodSettings.PhysicalPath))
        {
            savePath = prodSettings.PhysicalPath;
        }
        else if (
            HostEnv.IsStaging() && 
            stagingSettings.Exists && 
            !string.IsNullOrWhiteSpace(stagingSettings.PhysicalPath))
        {
            savePath = stagingSettings.PhysicalPath;
        }
        else if (
            HostEnv.IsDevelopment() && 
            devSettings.Exists && 
            !string.IsNullOrWhiteSpace(devSettings.PhysicalPath))
        {
            savePath = devSettings.PhysicalPath;
        }
        else if (settings.Exists && !string.IsNullOrWhiteSpace(settings.PhysicalPath))
        {
            savePath = settings.PhysicalPath;
        }
        else
        {
            return;
        }

        var settingsJson = JsonSerializer.Deserialize<IDictionary<string, object>>(await System.IO.File.ReadAllTextAsync(savePath));
        if (settingsJson is null)
        {
            return;
        }
        settingsJson["ApplicationOptions"] = Input;
        settingsJson["ConnectionStrings"] = ConnectionStrings;

        await System.IO.File.WriteAllTextAsync(savePath, JsonSerializer.Serialize(settingsJson, new JsonSerializerOptions() { WriteIndented = true }));

        if (Configuration is IConfigurationRoot root)
        {
            root.Reload();
        }
    }
    private void SetIsServerAdmin(ChangeEventArgs ev, RemotelyUser user)
    {
        if (ev.Value is not bool isAdmin)
        {
            return;
        }

        EnsureUserSet();

        DataService.SetIsServerAdmin(user.Id, isAdmin, User.Id);
        ToastService.ShowToast("Server admins updated.");
    }

    private void ShowOutdatedDevices()
    {
        if (OutdatedDevices.Any())
        {
            var outdatedDeviceNames = DataService
                .GetDevices(OutdatedDevices)
                .Select(x => $"{x.DeviceName}");

            var body = (new[] { "Outdated Devices:" })
                .Concat(outdatedDeviceNames)
                .ToArray();

            ModalService.ShowModal("Outdated Devices", body);
        }
        else
        {
            ModalService.ShowModal("Outdated Devices", new[] { "There are no outdated devices currently online." });
        }
    }

    private async Task UpdateAllDevices()
    {
        EnsureUserSet();

        if (!User.IsServerAdmin)
        {
            return;
        }

        if (!OutdatedDevices.Any())
        {
            ToastService.ShowToast("No agents need updating.");
            return;
        }

        var agentConnections = ServiceSessionCache.GetConnectionIdsByDeviceIds(OutdatedDevices);

        await AgentHubContext.Clients.Clients(agentConnections).ReinstallAgent();
        ToastService.ShowToast("Update command sent.");
    }
}
