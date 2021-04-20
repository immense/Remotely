using Microsoft.Extensions.Configuration;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;

namespace Remotely.Server.Services
{
    public interface IApplicationConfig
    {
        bool AllowApiLogin { get; }
        string[] BannedDevices { get; }
        double DataRetentionInDays { get; }
        string DBProvider { get; }
        bool EnableWindowsEventLog { get; }
        bool EnforceAttendedAccess { get; }
        IceServerModel[] IceServers { get; }
        string[] KnownProxies { get; }
        int MaxConcurrentUpdates { get; }
        int MaxOrganizationCount { get; }
        string MessageOfTheDay { get; }
        bool RedirectToHttps { get; }
        bool RemoteControlNotifyUser { get; }
        bool RemoteControlRequiresAuthentication { get; }
        double RemoteControlSessionLimit { get; }
        bool Require2FA { get; }
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
        bool UseWebRtc { get; }
    }

    public class ApplicationConfig : IApplicationConfig
    {
        private readonly IceServerModel[] fallbackIceServers = new IceServerModel[]
        {
            new IceServerModel() { Url = "stun:stun.l.google.com:19302"},
            new IceServerModel() { Url = "stun:stun4.l.google.com:19302"}
        };

        public ApplicationConfig(IConfiguration config)
        {
            Config = config;
        }

        public bool AllowApiLogin => bool.Parse(Config["ApplicationOptions:AllowApiLogin"] ?? "false");
        public string[] BannedDevices => Config.GetSection("ApplicationOptions:BannedDevices").Get<string[]>() ?? System.Array.Empty<string>();
        public double DataRetentionInDays => double.Parse(Config["ApplicationOptions:DataRetentionInDays"] ?? "30");
        public string DBProvider => Config["ApplicationOptions:DBProvider"] ?? "SQLite";
        public bool EnableWindowsEventLog => bool.Parse(Config["ApplicationOptions:EnableWindowsEventLog"]);
        public bool EnforceAttendedAccess => bool.Parse(Config["ApplicationOptions:EnforceAttendedAccess"] ?? "false");
        public IceServerModel[] IceServers => Config.GetSection("ApplicationOptions:IceServers").Get<IceServerModel[]>() ?? fallbackIceServers;
        public string[] KnownProxies => Config.GetSection("ApplicationOptions:KnownProxies").Get<string[]>() ?? System.Array.Empty<string>();
        public int MaxConcurrentUpdates => int.Parse(Config["ApplicationOptions:MaxConcurrentUpdates"] ?? "10");
        public int MaxOrganizationCount => int.Parse(Config["ApplicationOptions:MaxOrganizationCount"] ?? "1");
        public string MessageOfTheDay => Config["ApplicationOptions:MessageOfTheDay"];
        public bool RedirectToHttps => bool.Parse(Config["ApplicationOptions:RedirectToHttps"] ?? "false");
        public bool RemoteControlNotifyUser => bool.Parse(Config["ApplicationOptions:RemoteControlNotifyUser"] ?? "true");
        public bool RemoteControlRequiresAuthentication => bool.Parse(Config["ApplicationOptions:RemoteControlRequiresAuthentication"] ?? "true");
        public double RemoteControlSessionLimit => double.Parse(Config["ApplicationOptions:RemoteControlSessionLimit"] ?? "3");
        public bool Require2FA => bool.Parse(Config["ApplicationOptions:Require2FA"] ?? "false");
        public string SmtpDisplayName => Config["ApplicationOptions:SmtpDisplayName"];
        public string SmtpEmail => Config["ApplicationOptions:SmtpEmail"];
        public string SmtpHost => Config["ApplicationOptions:SmtpHost"];
        public string SmtpLocalDomain => Config["ApplicationOptions:SmtpLocalDomain"];
        public string SmtpPassword => Config["ApplicationOptions:SmtpPassword"];
        public int SmtpPort => int.Parse(Config["ApplicationOptions:SmtpPort"] ?? "25");
        public string SmtpUserName => Config["ApplicationOptions:SmtpUserName"];
        public Theme Theme => Enum.Parse<Theme>(Config["ApplicationOptions:Theme"] ?? "Dark", true);
        public string[] TrustedCorsOrigins => Config.GetSection("ApplicationOptions:TrustedCorsOrigins").Get<string[]>() ?? System.Array.Empty<string>();
        public bool UseHsts => bool.Parse(Config["ApplicationOptions:UseHsts"] ?? "false");
        public bool UseWebRtc => bool.Parse(Config["ApplicationOptions:UseWebRtc"] ?? "true");


        private IConfiguration Config { get; set; }
    }
}
