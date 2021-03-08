using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Remotely.Server.Areas.Identity.Pages.Account.Manage
{
    public class ServerConfigModel : PageModel
    {
        private readonly IHubContext<AgentHub> _agentHubContext;
        private readonly IConfiguration _configuration;
        private readonly IDataService _dataService;
        private readonly IEmailSenderEx _emailSender;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly UserManager<RemotelyUser> _userManager;
               
        public ServerConfigModel(IConfiguration configuration,
            IWebHostEnvironment hostEnv,
            UserManager<RemotelyUser> userManager,
            IDataService dataService,
            IEmailSenderEx emailSender,
            IHubContext<AgentHub> agentHubContext)
        {
            _configuration = configuration;
            _hostEnv = hostEnv;
            _userManager = userManager;
            _dataService = dataService;
            _emailSender = emailSender;
            _agentHubContext = agentHubContext;
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

        public string Environment { get; set; }
        public bool IsServerAdmin { get; set; }
        public int TotalDevices { get; set; }
        public IEnumerable<string> OutdatedDevices { get; set; }

        [BindProperty]
        [Display(Name = "Server Admins")]
        public List<string> ServerAdmins { get; set; } = new List<string>();

        [TempData]
        public string StatusMessage { get; set; }
        public async Task<IActionResult> OnGet()
        {
            IsServerAdmin = (await _userManager.GetUserAsync(User)).IsServerAdmin;
            if (!IsServerAdmin)
            {
                return Unauthorized();
            }

            TotalDevices = _dataService.GetTotalDevices();
            OutdatedDevices = GetOutdatedDevices();

            Environment = _hostEnv.EnvironmentName;

            _configuration.Bind("ApplicationOptions", AppSettingsInput);
            _configuration.Bind("ConnectionStrings", ConnectionStrings);
            ServerAdmins = _dataService.GetServerAdmins();

            return Page();
        }
        public async Task<IActionResult> OnPostSaveAndTestSmtpAsync()
        {
            var user = _dataService.GetUserByName(User.Identity.Name);
            if (!user.IsServerAdmin)
            {
                return Unauthorized();
            }
            var result = await OnPostSaveAsync();
            var success = await _emailSender.SendEmailAsync(user.Email, "Remotely Test Email", "Congratulations! Your SMTP settings are working!", user.OrganizationID);
            if (success)
            {
                StatusMessage = "Test email sent.  Check your inbox (including spam folder).";
            }
            else
            {
                StatusMessage = "Error sending email.  Check the server logs for details.";
            }
            return result;
        }
        public async Task<IActionResult> OnPostSaveAsync()
        {
            IsServerAdmin = (await _userManager.GetUserAsync(User)).IsServerAdmin;
            if (!IsServerAdmin)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var configReloaded = false;
            _configuration.GetReloadToken().RegisterChangeCallback((e) =>
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

        public async Task<IActionResult> OnPostUpdateAllAsync()
        {
            var user = _dataService.GetUserByName(User.Identity.Name);
            if (!user.IsServerAdmin)
            {
                return Unauthorized();
            }
            var outdatedDevices = GetOutdatedDevices();
            if (!outdatedDevices.Any())
            {
                StatusMessage = "No agents need updating.";
                return RedirectToPage();
            }
            var agentConnections = AgentHub.ServiceConnections.Where(x => outdatedDevices.Contains(x.Value.ID));
            await _agentHubContext.Clients.Clients(agentConnections.Select(x => x.Key)).SendAsync("ReinstallAgent");
            StatusMessage = $"Update command sent to {agentConnections.Count()} agents.";
            return RedirectToPage();
        }

        private IEnumerable<string> GetOutdatedDevices()
        {
            var highestVersion = AgentHub.ServiceConnections.Values.Max(x => Version.TryParse(x.AgentVersion, out var result) ? result : default);
            return AgentHub.ServiceConnections.Values
                .Where(x => Version.TryParse(x.AgentVersion, out var result) && result != highestVersion)
                .Select(x => x.ID);
        }

        private async Task SaveAppSettings()
        {
            string savePath;
            var prodSettings = _hostEnv.ContentRootFileProvider.GetFileInfo("appsettings.Production.json");
            var stagingSettings = _hostEnv.ContentRootFileProvider.GetFileInfo("appsettings.Staging.json");
            var settings = _hostEnv.ContentRootFileProvider.GetFileInfo("appsettings.json");
            if (_hostEnv.IsProduction() && prodSettings.Exists)
            {
                savePath = prodSettings.PhysicalPath;
            }
            else if (_hostEnv.IsStaging() && stagingSettings.Exists)
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
            AppSettingsInput.IceServers = _configuration.GetSection("ApplicationOptions:IceServers").Get<IceServerModel[]>();
            settingsJson["ApplicationOptions"] = AppSettingsInput;
            settingsJson["ConnectionStrings"] = ConnectionStrings;

            await System.IO.File.WriteAllTextAsync(savePath, JsonSerializer.Serialize(settingsJson, new JsonSerializerOptions() { WriteIndented = true }));

            await _dataService.UpdateServerAdmins(ServerAdmins, User.Identity.Name);
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
            [Display(Name = "Message of the Day")]
            public string MessageOfTheDay { get; set; }

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