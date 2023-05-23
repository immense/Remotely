using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using Npgsql;
using Remotely.Server.Areas.Identity;
using Remotely.Server.Auth;
using Remotely.Server.Data;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System.IO;
using System.Linq;
using System.Net;
using Remotely.Shared.Utilities;
using Immense.RemoteControl.Server.Extensions;
using Remotely.Server.Services.RcImplementations;
using Immense.RemoteControl.Server.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Remotely.Shared.Services;
using System;
using Immense.RemoteControl.Server.Services;
using Serilog;
using Nihs.SimpleMessenger;

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

var dbProvider = configuration["ApplicationOptions:DBProvider"].ToLower();
if (dbProvider == "sqlite")
{
    services.AddDbContext<AppDb, SqliteDbContext>(options =>
    {
        options.UseSqlite(configuration.GetConnectionString("SQLite"));
    });
}
else if (dbProvider == "sqlserver")
{
    services.AddDbContext<AppDb, SqlServerDbContext>(options =>
    {
        options.UseSqlServer(configuration.GetConnectionString("SQLServer"));
    });
}
else if (dbProvider == "postgresql")
{
    services.AddDbContext<AppDb, PostgreSqlDbContext>(options =>
    {
        // Password should be set in User Secrets in dev environment.
        // See https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1
        if (!string.IsNullOrWhiteSpace(configuration.GetValue<string>("PostgresPassword")))
        {
            var connectionBuilder = new NpgsqlConnectionStringBuilder(configuration.GetConnectionString("PostgreSQL"))
            {
                Password = configuration["PostgresPassword"]
            };
            options.UseNpgsql(connectionBuilder.ConnectionString);
        }
        else
        {
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL"));
        }
    });
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
services.AddAuthorization(options =>
{
    options.AddPolicy(TwoFactorRequiredRequirement.PolicyName, builder =>
    {
        builder.Requirements.Add(new TwoFactorRequiredRequirement());
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
services.AddScoped<IClientAppState, ClientAppState>();
services.AddScoped<IExpiringTokenService, ExpiringTokenService>();
services.AddScoped<IScriptScheduleDispatcher, ScriptScheduleDispatcher>();
services.AddSingleton<IOtpProvider, OtpProvider>();
services.AddSingleton<IEmbeddedServerDataSearcher, EmbeddedServerDataSearcher>();
services.AddSingleton<ILogsManager, LogsManager>();
services.AddSingleton(WeakReferenceMessenger.Default);

services.AddRemoteControlServer(config =>
{
    config.AddHubEventHandler<HubEventHandler>();
    config.AddViewerAuthorizer<ViewerAuthorizer>();
    config.AddViewerPageDataProvider<ViewerPageDataProvider>();
});

services.AddSingleton<IServiceHubSessionCache, ServiceHubSessionCache>();

var app = builder.Build();
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
    if (bool.Parse(app.Configuration["ApplicationOptions:UseHsts"]))
    {
        app.UseHsts();
    }
    if (bool.Parse(app.Configuration["ApplicationOptions:RedirectToHttps"]))
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
        context.Database.Migrate();
    }

    await dataService.SetAllDevicesNotOnline();
    dataService.CleanupOldRecords();
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
                .WriteTo.Console()
                .WriteTo.File($"{logPath}/Remotely_Server.log", 
                    rollingInterval: RollingInterval.Day, 
                    retainedFileTimeLimit: TimeSpan.FromDays(dataRetentionDays),
                    shared: true);
        }

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