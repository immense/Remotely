using Remotely_Library.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely_Server.Services
{
    public class ApplicationConfig
    {
        public ApplicationConfig(IConfiguration config)
        {
            Config = config;
        }
        public string DefaultPrompt => Config["ApplicationOptions:DefaultPrompt"];
        public string DBProvider => Config["ApplicationOptions:DBProvider"];
        public bool AllowSelfRegistration => bool.Parse(Config["ApplicationOptions:AllowSelfRegistration"]);
        public bool UseDomainAuthentication => bool.Parse(Config["ApplicationOptions:UseDomainAuthentication"]);
        public bool ShowMessageOfTheDay => bool.Parse(Config["ApplicationOptions:ShowMessageOfTheDay"]);
        public double DataRetentionInDays => double.Parse(Config["ApplicationOptions:DataRetentionInDays"]);
        public double RemoteControlSessionLimit => double.Parse(Config["ApplicationOptions:RemoteControlSessionLimit"]);

        public string SmtpHost => Config["ApplicationOptions:SmtpHost"];
        public int SmtpPort => int.Parse(Config["ApplicationOptions:SmtpPort"]);
        public string SmtpUserName => Config["ApplicationOptions:SmtpUserName"];
        public string SmtpPassword => Config["ApplicationOptions:SmtpPassword"];
        public string SmtpEmail => Config["ApplicationOptions:SmtpEmail"];
        public string SmtpDisplayName => Config["ApplicationOptions:SmtpDisplayName"];

        private IConfiguration Config { get; set; }
    }
}
