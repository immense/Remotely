using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using Remotely.Server.Areas.Identity;
using Remotely.Server.Auth;
using Remotely.Server.Data;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using System.IO;
using System.Linq;
using System.Net;
using Immense.RemoteControl.Server.Extensions;
using Remotely.Server.Services.RcImplementations;
using Remotely.Shared.Services;
using System;
using Serilog;
using Microsoft.AspNetCore.RateLimiting;
using RatePolicyNames = Remotely.Server.RateLimiting.PolicyNames;
using Remotely.Shared.Entities;
using Immense.SimpleMessenger;
using Remotely.Server.Services.Stores;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

ConfigureSerilog(builder);

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

if (OperatingSystem.IsWindows() &&
    bool.TryParse(builder.Configuration["ApplicationOptions:EnableWindowsEventLog"], out var enableEventLog) &&
    enableEventLog)
{
    builder.Logging.AddEventLog();
}

var dbProvider = configuration["ApplicationOptions:DBProvider"]?.ToLower();
if (string.IsNullOrWhiteSpace(dbProvider))
{
    throw new InvalidOperationException("DBProvider is missing from appsettings.json.");
}

if (dbProvider == "sqlite")
{
    services.AddDbContext<AppDb, SqliteDbContext>();
}
else if (dbProvider == "sqlserver")
{
    services.AddDbContext<AppDb, SqlServerDbContext>();
}
else if (dbProvider == "postgresql")
{
    services.AddDbContext<AppDb, PostgreSqlDbContext>();
}

services.AddIdentity<RemotelyUser, IdentityRole>(options =>
{
    options.Stores.MaxLengthForKeys = 128;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<AppDb>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();


services.AddScoped<IAuthorizationHandler, TwoFactorRequiredHandler>();
services.AddScoped<IAuthorizationHandler, OrganizationAdminRequirementHandler>();
services.AddScoped<IAuthorizationHandler, ServerAdminRequirementHandler>();

services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNames.TwoFactorRequired, builder =>
    {
        builder.Requirements.Add(new TwoFactorRequiredRequirement());
    });

    options.AddPolicy(PolicyNames.OrganizationAdminRequired, builder =>
    {
        builder.Requirements.Add(new OrganizationAdminRequirement());
    });

    options.AddPolicy(PolicyNames.ServerAdminRequired, builder =>
    {
        builder.Requirements.Add(new ServerAdminRequirement());
    });
});

services.AddRazorPages();
services.AddServerSideBlazor();
services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<RemotelyUser>>();
services.AddDatabaseDeveloperPageExceptionFilter();

if (bool.TryParse(configuration["ApplicationOptions:UseHttpLogging"], out var useHttpLogging) &&
    useHttpLogging)
{
    services.AddHttpLogging(options =>
    {
        options.RequestHeaders.Add("X-Forwarded-For");
        options.RequestHeaders.Add("X-Forwarded-Proto");
        options.RequestHeaders.Add("X-Forwarded-Host");
        options.RequestHeaders.Add("X-Original-For");
        options.RequestHeaders.Add("X-Original-Proto");
        options.RequestHeaders.Add("X-Original-Host");
        options.RequestHeaders.Add("Host");
    });
}

var trustedOrigins = configuration.GetSection("ApplicationOptions:TrustedCorsOrigins").Get<string[]>();

if (trustedOrigins != null)
{
    services.AddCors(options =>
    {
        options.AddPolicy("TrustedOriginPolicy", builder => builder
            .WithOrigins(trustedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
        );
    });
}

var knownProxies = configuration.GetSection("ApplicationOptions:KnownProxies").Get<string[]>();
services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.All;
    options.ForwardLimit = null;

    // Default Docker host. We want to allow forwarded headers from this address.
    options.KnownProxies.Add(IPAddress.Parse("172.17.0.1"));

    if (knownProxies?.Any() == true)
    {
        foreach (var proxy in knownProxies)
        {
            options.KnownProxies.Add(IPAddress.Parse(proxy));
        }
    }
});

services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.MaximumParallelInvocationsPerClient = 5;
    options.MaximumReceiveMessageSize = 100_000;
})
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
    })
    .AddMessagePackProtocol();

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Remotely API", Version = "v1" });
});

services.AddRateLimiter(options =>
{
    options.AddConcurrencyLimiter(RatePolicyNames.AgentUpdateDownloads, clOptions =>
    {
        clOptions.QueueLimit = int.MaxValue;

        var concurrentPermits = configuration.GetSection("ApplicationOptions:MaxConcurrentUpdates").Get<int>();
        if (concurrentPermits <= 0)
        {
            concurrentPermits = 10;
        }

        clOptions.PermitLimit = concurrentPermits;
    });
});
services.AddHttpClient();
services.AddLogging();
services.AddScoped<IEmailSenderEx, EmailSenderEx>();
services.AddScoped<IEmailSender, EmailSender>();
services.AddScoped<IAppDbFactory, AppDbFactory>();
services.AddTransient<IDataService, DataService>();
services.AddSingleton<IApplicationConfig, ApplicationConfig>();
services.AddScoped<ApiAuthorizationFilter>();
services.AddScoped<LocalOnlyFilter>();
services.AddScoped<ExpiringTokenFilter>();
services.AddHostedService<DataCleanupService>();
services.AddHostedService<ScriptScheduler>();
services.AddSingleton<IUpgradeService, UpgradeService>();
services.AddScoped<IToastService, ToastService>();
services.AddScoped<IModalService, ModalService>();
services.AddScoped<IJsInterop, JsInterop>();
services.AddScoped<ICircuitConnection, CircuitConnection>();
services.AddScoped<ILoaderService, LoaderService>();
services.AddScoped(x => (CircuitHandler)x.GetRequiredService<ICircuitConnection>());
services.AddSingleton<ICircuitManager, CircuitManager>();
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<ISelectedCardsStore, SelectedCardsStore>();
services.AddScoped<IExpiringTokenService, ExpiringTokenService>();
services.AddScoped<IScriptScheduleDispatcher, ScriptScheduleDispatcher>();
services.AddSingleton<IOtpProvider, OtpProvider>();
services.AddSingleton<IEmbeddedServerDataSearcher, EmbeddedServerDataSearcher>();
services.AddSingleton<ILogsManager, LogsManager>();
services.AddScoped<IThemeProvider, ThemeProvider>();
services.AddScoped<IChatSessionStore, ChatSessionStore>();
services.AddScoped<ITerminalStore, TerminalStore>();
services.AddSingleton(WeakReferenceMessenger.Default);

services.AddRemoteControlServer(config =>
{
    config.AddHubEventHandler<HubEventHandler>();
    config.AddViewerAuthorizer<ViewerAuthorizer>();
    config.AddViewerPageDataProvider<ViewerPageDataProvider>();
    config.AddViewerOptionsProvider<ViewerOptionsProvider>();
    config.AddSessionRecordingSink<SessionRecordingSink>();
});

services.AddSingleton<IAgentHubSessionCache, AgentHubSessionCache>();

var app = builder.Build();

app.UseRateLimiter();

var appConfig = app.Services.GetRequiredService<IApplicationConfig>();

if (appConfig.UseHttpLogging)
{
    app.UseHttpLogging();
}

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    if (bool.TryParse(app.Configuration["ApplicationOptions:UseHsts"], out var hsts) && hsts)
    {
        app.UseHsts();
    }
    if (bool.TryParse(app.Configuration["ApplicationOptions:RedirectToHttps"], out var redirect) && redirect)
    {
        app.UseHttpsRedirection();
    }
}

ConfigureStaticFiles();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Remotely API V1");
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("TrustedOriginPolicy");

app.UseRemoteControlServer();


app.MapHub<AgentHub>("/hubs/service");
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetRequiredService<AppDb>();
    var dataService = scope.ServiceProvider.GetRequiredService<IDataService>();

    if (context.Database.IsRelational())
    {
        await context.Database.MigrateAsync();
    }

    await dataService.SetAllDevicesNotOnline();
    await dataService.CleanupOldRecords();
}

await app.RunAsync();

void ConfigureStaticFiles()
{
    var provider = new FileExtensionContentTypeProvider();
    // Add new mappings
    provider.Mappings[".ps1"] = "application/octet-stream";
    provider.Mappings[".exe"] = "application/octet-stream";
    provider.Mappings[".dll"] = "application/octet-stream";
    provider.Mappings[".appimage"] = "application/octet-stream";
    provider.Mappings[".zip"] = "application/octet-stream";
    provider.Mappings[".config"] = "application/octet-stream";
    app.UseStaticFiles();
    var contentPath = Path.Combine(app.Environment.WebRootPath, "Content");

    if (Directory.Exists(contentPath))
    {
        app.UseStaticFiles(new StaticFileOptions()
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.WebRootPath, "Content")),
            ServeUnknownFileTypes = true,
            RequestPath = new PathString("/Content"),
            ContentTypeProvider = provider,
            DefaultContentType = "application/octet-stream"
        });
    }

    // Needed for Let's Encrypt.
    if (Directory.Exists(Path.Combine(app.Environment.ContentRootPath, ".well-known")))
    {
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, @".well-known")),
            RequestPath = new PathString("/.well-known"),
            ServeUnknownFileTypes = true
        });
    }
}

void ConfigureSerilog(WebApplicationBuilder webAppBuilder)
{
    try
    {
        var dataRetentionDays = 7;
        if (int.TryParse(webAppBuilder.Configuration["ApplicationOptions:DataRetentionInDays"], out var retentionSetting))
        {
            dataRetentionDays = retentionSetting;
        }

        var logPath = LogsManager.DefaultLogsDirectory;

        void ApplySharedLoggerConfig(LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}")
                .WriteTo.File($"{logPath}/Remotely_Server.log", 
                    rollingInterval: RollingInterval.Day, 
                    retainedFileTimeLimit: TimeSpan.FromDays(dataRetentionDays),
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}",
                    shared: true);
        }

        // https://github.com/serilog/serilog-aspnetcore#two-stage-initialization
        var loggerConfig = new LoggerConfiguration();
        ApplySharedLoggerConfig(loggerConfig);
        Log.Logger = loggerConfig.CreateBootstrapLogger();

        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services);

            ApplySharedLoggerConfig(configuration);
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to configure Serilog file logging.  Error: {ex.Message}");
    }
}