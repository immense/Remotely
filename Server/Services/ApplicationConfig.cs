using Remotely.Shared.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Services
{
    public class ApplicationConfig
    {
        public ApplicationConfig(IConfiguration config)
        {
            Config = config;
        }
        public bool AllowApiLogin => bool.Parse(Config["ApplicationOptions:AllowApiLogin"]);
        public bool AllowSelfRegistration => bool.Parse(Config["ApplicationOptions:AllowSelfRegistration"]);
        public double DataRetentionInDays => double.Parse(Config["ApplicationOptions:DataRetentionInDays"]);
        public string DBProvider => Config["ApplicationOptions:DBProvider"];
        public string DefaultPrompt => Config["ApplicationOptions:DefaultPrompt"];
        public bool EnableWindowsEventLog => bool.Parse(Config["ApplicationOptions:EnableWindowsEventLog"]);
        public string[] KnownProxies => Config.GetSection("ApplicationOptions:KnownProxies").Get<string[]>();
        public bool RecordRemoteControlSessions => bool.Parse(Config["ApplicationOptions:RecordRemoteControlSessions"]);
        public bool RedirectToHTTPS => bool.Parse(Config["ApplicationOptions:RedirectToHTTPS"]);
        public bool RemoteControlRequiresAuthentication => bool.Parse(Config["ApplicationOptions:RemoteControlRequiresAuthentication"]);
        public double RemoteControlSessionLimit => double.Parse(Config["ApplicationOptions:RemoteControlSessionLimit"]);
        public string SmtpDisplayName => Config["ApplicationOptions:SmtpDisplayName"];
        public string SmtpEmail => Config["ApplicationOptions:SmtpEmail"];
        public bool SmtpEnableSsl => bool.Parse(Config["ApplicationOptions:SmtpEnableSsl"]);
        public string SmtpHost => Config["ApplicationOptions:SmtpHost"];
        public string SmtpPassword => Config["ApplicationOptions:SmtpPassword"];
        public int SmtpPort => int.Parse(Config["ApplicationOptions:SmtpPort"]);
        public string SmtpUserName => Config["ApplicationOptions:SmtpUserName"];
        public string Theme => Config["ApplicationOptions:Theme"];
        public string[] TrustedCorsOrigins => Config.GetSection("ApplicationOptions:TrustedCorsOrigins").Get<string[]>();
        public bool UseHSTS => bool.Parse(Config["ApplicationOptions:RedirectToHTTPS"]);
        private IConfiguration Config { get; set; }
    }
}
