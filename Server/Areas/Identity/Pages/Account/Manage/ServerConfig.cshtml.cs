using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Remotely.Server.Services;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Remotely.Server.Areas.Identity.Pages.Account.Manage
{
    public class ServerConfigModel : PageModel
    {
        public ServerConfigModel(IConfiguration configuration,
            IWebHostEnvironment hostEnv,
            UserManager<RemotelyUser> userManager,
            IDataService dataService)
        {
            Configuration = configuration;
            HostEnv = hostEnv;
            UserManager = userManager;
            DataService = dataService;
        }

        public enum DBProviders
        {
            PostgreSQL,
            SQLite,
            SQLServer
        }

        [BindProperty]
        public AppSettingsModel AppSettingsInput { get; set; } = new AppSettingsModel();

        [BindProperty]
        public ConnectionStringsModel ConnectionStrings { get; set; } = new ConnectionStringsModel();

        public bool IsServerAdmin { get; set; }
        public string Environment { get; set; }

        [BindProperty]
        [Display(Name = "Server Admins")]
        public List<string> ServerAdmins { get; set; } = new List<string>();

        [TempData]
        public string StatusMessage { get; set; }

        private IConfiguration Configuration { get; }
        private IDataService DataService { get; }
        private IWebHostEnvironment HostEnv { get; }
        private UserManager<RemotelyUser> UserManager { get; }

        public async Task<IActionResult> OnGet()
        {
            IsServerAdmin = (await UserManager.GetUserAsync(User)).IsServerAdmin;
            Environment = HostEnv.EnvironmentName;
            if (!IsServerAdmin)
            {
                return Unauthorized();
            }

            Configuration.Bind("ApplicationOptions", AppSettingsInput);
            Configuration.Bind("ConnectionStrings", ConnectionStrings);
            ServerAdmins = DataService.GetServerAdmins();

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            IsServerAdmin = (await UserManager.GetUserAsync(User)).IsServerAdmin;
            if (!IsServerAdmin)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var configReloaded = false;
            Configuration.GetReloadToken().RegisterChangeCallback((e) =>
            {
                configReloaded = true;
            }, null);

            await SaveAppSettings();

            while (!configReloaded)
            {
                await Task.Delay(10);
            }

            StatusMessage = "Configuration saved.";

            return RedirectToPage();
        }

        private async Task SaveAppSettings()
        {
            string savePath;
            var prodSettings = HostEnv.ContentRootFileProvider.GetFileInfo("appsettings.Production.json");
            var stagingSettings = HostEnv.ContentRootFileProvider.GetFileInfo("appsettings.Staging.json");
            var settings = HostEnv.ContentRootFileProvider.GetFileInfo("appsettings.json");
            if (HostEnv.IsProduction() && prodSettings.Exists)
            {
                savePath = prodSettings.PhysicalPath;
            }
            else if (HostEnv.IsStaging() && stagingSettings.Exists)
            {
                savePath = stagingSettings.PhysicalPath;
            }
            else if (settings.Exists)
            {
                savePath = settings.PhysicalPath;
            }
            else
            {
                return;
            }

            var settingsJson = JsonSerializer.Deserialize<IDictionary<string, object>>(await System.IO.File.ReadAllTextAsync(savePath));
            AppSettingsInput.IceServers = Configuration.GetSection("ApplicationOptions:IceServers").Get<IceServerModel[]>();
            settingsJson["ApplicationOptions"] = AppSettingsInput;
            settingsJson["ConnectionStrings"] = ConnectionStrings;

            await System.IO.File.WriteAllTextAsync(savePath, JsonSerializer.Serialize(settingsJson, new JsonSerializerOptions() { WriteIndented = true }));

            await DataService.UpdateServerAdmins(ServerAdmins, User.Identity.Name);
        }

        public class AppSettingsModel
        {
            [Display(Name = "Allow API Login")]
            public bool AllowApiLogin { get; set; }

            [Display(Name = "Banned Devices")]
            public string[] BannedDevices { get; set; }

            [Display(Name = "Data Retention (days)")]
            public double DataRetentionInDays { get; set; }

            [Display(Name = "Database Provider")]
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public DBProviders DBProvider { get; set; }

            [Display(Name = "Default Prompt")]
            public string DefaultPrompt { get; set; }

            [Display(Name = "Enable Windows Event Log")]
            public bool EnableWindowsEventLog { get; set; }

            [Display(Name = "Enforce Attended Access")]
            public bool EnforceAttendedAccess { get; set; }

            [Display(Name = "Ice Servers")]
            public IceServerModel[] IceServers { get; set; }

            [Display(Name = "Known Proxies")]
            public string[] KnownProxies { get; set; }

            [Display(Name = "Max Concurrent Updates")]
            public int MaxConcurrentUpdates { get; set; }

            [Display(Name = "Max Organizations")]
            public int MaxOrganizationCount { get; set; }

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
            public string SmtpDisplayName { get; set; }

            [Display(Name = "SMTP Email")]
            [EmailAddress]
            public string SmtpEmail { get; set; }

            [Display(Name = "SMTP Enable SSL")]
            public bool SmtpEnableSsl { get; set; }

            [Display(Name = "SMTP Host")]
            public string SmtpHost { get; set; }

            [Display(Name = "SMTP Password")]
            public string SmtpPassword { get; set; }

            [Display(Name = "SMTP Port")]
            public int SmtpPort { get; set; }

            [Display(Name = "SMTP Username")]
            public string SmtpUserName { get; set; }

            [Display(Name = "Theme")]
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public Theme Theme { get; set; }

            [Display(Name = "Trusted CORS Origins")]
            public string[] TrustedCorsOrigins { get; set; }

            [Display(Name = "Use HSTS")]
            public bool UseHsts { get; set; }

            [Display(Name = "Use WebRTC")]
            public bool UseWebRtc { get; set; }
        }

        public class ConnectionStringsModel
        {
            [Display(Name = "PostgreSQL")]
            public string PostgreSQL { get; set; }

            [Display(Name = "SQLite")]
            public string SQLite { get; set; }

            [Display(Name = "SQL Server")]
            public string SQLServer { get; set; }
        }
    }
}