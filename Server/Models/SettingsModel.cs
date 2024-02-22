using Remotely.Shared.Enums;

namespace Remotely.Server.Models;

public class SettingsModel
{
    public static Guid DbKey { get; } = Guid.Parse("a35d6212-c0b7-49b2-89e1-7ba497f94a35");

    public bool AllowApiLogin { get; set; }
    public List<string> BannedDevices { get; set; } = [];
    public double DataRetentionInDays { get; set; } = 90;
    public string DbProvider { get; set; } = "SQLite";
    public bool EnableRemoteControlRecording { get; set; }
    public bool EnableWindowsEventLog { get; set; }
    public bool EnforceAttendedAccess { get; set; }
    public bool ForceClientHttps { get; set; }
    public List<string> KnownProxies { get; set; } = [];
    public int MaxConcurrentUpdates { get; set; } = 10;
    public int MaxOrganizationCount { get; set; } = 1;
    public string MessageOfTheDay { get; set; } = string.Empty;
    public bool RedirectToHttps { get; set; } = true;
    public bool RemoteControlNotifyUser { get; set; } = true;
    public bool RemoteControlRequiresAuthentication { get; set; } = true;
    public int RemoteControlSessionLimit { get; set; } = 5;
    public bool Require2FA { get; set; }
    public string ServerUrl { get; set; } = string.Empty;
    public bool SmtpCheckCertificateRevocation { get; set; } = true;
    public string SmtpDisplayName { get; set; } = string.Empty;
    public string SmtpEmail { get; set; } = string.Empty;
    public string SmtpHost { get; set; } = string.Empty;
    public string SmtpLocalDomain { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUserName { get; set; } = string.Empty;
    public Theme Theme { get; set; } = Theme.Dark;
    public List<string> TrustedCorsOrigins { get; set; } = [];
    public bool UseHsts { get; set; }
    public bool UseHttpLogging { get; set; }
}
