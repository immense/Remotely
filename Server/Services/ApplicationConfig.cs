using Microsoft.Extensions.Configuration;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;

namespace Remotely.Server.Services;

public interface IApplicationConfig
{
    bool AllowApiLogin { get; }
    string[] BannedDevices { get; }
    double DataRetentionInDays { get; }
    string DBProvider { get; }
    bool EnableRemoteControlRecording { get; }
    bool EnableWindowsEventLog { get; }
    bool EnforceAttendedAccess { get; }
    bool ForceClientHttps { get; }
    string[] KnownProxies { get; }
    int MaxConcurrentUpdates { get; }
    int MaxOrganizationCount { get; }
    string MessageOfTheDay { get; }
    bool RedirectToHttps { get; }
    bool RemoteControlNotifyUser { get; }
    bool RemoteControlRequiresAuthentication { get; }
    int RemoteControlSessionLimit { get; }
    bool Require2FA { get; }
    string ServerUrl { get; }
    bool SmtpCheckCertificateRevocation { get; }
    string SmtpDisplayName { get; }
    string SmtpEmail { get; }
    string SmtpHost { get; }
    string SmtpLocalDomain { get; }
    string SmtpPassword { get; }
    int SmtpPort { get; }
    string SmtpUserName { get; }
    Theme Theme { get; }
    string[] TrustedCorsOrigins { get; }
    bool UseHsts { get; }
    bool UseHttpLogging { get; }
}

public class ApplicationConfig : IApplicationConfig
{
    private readonly IConfiguration _config;

    public ApplicationConfig(IConfiguration config)
    {
        _config = config;
    }

    public bool AllowApiLogin => bool.TryParse(_config["ApplicationOptions:AllowApiLogin"], out var result) && result;
    public string[] BannedDevices => _config.GetSection("ApplicationOptions:BannedDevices").Get<string[]>() ?? System.Array.Empty<string>();
    public double DataRetentionInDays => double.TryParse(_config["ApplicationOptions:DataRetentionInDays"], out var result) ? result : 30;
    public string DBProvider => _config["ApplicationOptions:DBProvider"] ?? "SQLite";
    public bool EnableRemoteControlRecording => bool.TryParse(_config["ApplicationOptions:EnableRemoteControlRecording"], out var result) && result;
    public bool EnableWindowsEventLog => bool.TryParse(_config["ApplicationOptions:EnableWindowsEventLog"], out var result) && result;
    public bool EnforceAttendedAccess => bool.TryParse(_config["ApplicationOptions:EnforceAttendedAccess"], out var result) && result;
    public bool ForceClientHttps => bool.TryParse(_config["ApplicationOptions:ForceClientHttps"], out var result) && result;
    public string[] KnownProxies => _config.GetSection("ApplicationOptions:KnownProxies").Get<string[]>() ?? System.Array.Empty<string>();
    public int MaxConcurrentUpdates => int.TryParse(_config["ApplicationOptions:MaxConcurrentUpdates"], out var result) ? result : 10;
    public int MaxOrganizationCount => int.TryParse(_config["ApplicationOptions:MaxOrganizationCount"], out var result) ? result : 1;
    public string MessageOfTheDay => _config["ApplicationOptions:MessageOfTheDay"] ?? string.Empty;
    public bool RedirectToHttps => bool.TryParse(_config["ApplicationOptions:RedirectToHttps"], out var result) && result;
    public bool RemoteControlNotifyUser => bool.TryParse(_config["ApplicationOptions:RemoteControlNotifyUser"], out var result) && result;
    public bool RemoteControlRequiresAuthentication => bool.TryParse(_config["ApplicationOptions:RemoteControlRequiresAuthentication"], out var result) && result;
    public int RemoteControlSessionLimit => int.TryParse(_config["ApplicationOptions:RemoteControlSessionLimit"], out var result) ? result : 3;
    public bool Require2FA => bool.TryParse(_config["ApplicationOptions:Require2FA"], out var result) && result;
    public string ServerUrl => _config["ApplicationOptions:ServerUrl"] ?? string.Empty;
    public bool SmtpCheckCertificateRevocation => !bool.TryParse(_config["ApplicationOptions:SmtpCheckCertificateRevocation"], out var result) || result;
    public string SmtpDisplayName => _config["ApplicationOptions:SmtpDisplayName"] ?? string.Empty;
    public string SmtpEmail => _config["ApplicationOptions:SmtpEmail"] ?? string.Empty;
    public string SmtpHost => _config["ApplicationOptions:SmtpHost"] ?? string.Empty;
    public string SmtpLocalDomain => _config["ApplicationOptions:SmtpLocalDomain"] ?? string.Empty;
    public string SmtpPassword => _config["ApplicationOptions:SmtpPassword"] ?? string.Empty;
    public int SmtpPort => int.TryParse(_config["ApplicationOptions:SmtpPort"], out var result) ? result : 25;
    public string SmtpUserName => _config["ApplicationOptions:SmtpUserName"] ?? string.Empty;
    public Theme Theme => Enum.TryParse<Theme>(_config["ApplicationOptions:Theme"], out var result) ? result : Theme.Dark;
    public string[] TrustedCorsOrigins => _config.GetSection("ApplicationOptions:TrustedCorsOrigins").Get<string[]>() ?? System.Array.Empty<string>();
    public bool UseHsts => bool.TryParse(_config["ApplicationOptions:UseHsts"], out var result) && result;
    public bool UseHttpLogging => bool.TryParse(_config["ApplicationOptions:UseHttpLogging"], out var result) && result;
}
