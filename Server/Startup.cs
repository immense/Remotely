using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
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
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Npgsql;
using Remotely.Server.Data;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using Remotely.Server.Areas.Identity;
using System;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Authorization;
using Remotely.Server.Auth;
using Microsoft.AspNetCore.Http.Extensions;

namespace Remotely.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            IsDev = env.IsDevelopment();
        }

        public IConfiguration Configuration { get; }
        private bool IsDev { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            var dbProvider = Configuration["ApplicationOptions:DBProvider"].ToLower();
            if (dbProvider == "sqlite")
            {
                services.AddDbContextFactory<SqliteDbContext>(options =>
                {
                    options.UseSqlite(Configuration.GetConnectionString("SQLite"));
                });

                services.AddScoped<IDbContextFactory<AppDb>>(p =>
                    p.GetRequiredService<IDbContextFactory<SqliteDbContext>>());

                services.AddScoped<AppDb, SqliteDbContext>(p =>
                    p.GetRequiredService<IDbContextFactory<SqliteDbContext>>().CreateDbContext());

            }
            else if (dbProvider == "sqlserver")
            {
                services.AddDbContextFactory<SqlServerDbContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("SQLServer"));
                });

                services.AddScoped<IDbContextFactory<AppDb>>(p =>
                    p.GetRequiredService<IDbContextFactory<SqlServerDbContext>>());

                services.AddScoped<AppDb, SqlServerDbContext>(p =>
                    p.GetRequiredService<IDbContextFactory<SqlServerDbContext>>().CreateDbContext());
            }
            else if (dbProvider == "postgresql")
            {
                services.AddDbContextFactory<PostgreSqlDbContext>(options =>
                {
                    // Password should be set in User Secrets in dev environment.
                    // See https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1
                    if (!string.IsNullOrWhiteSpace(Configuration.GetValue<string>("PostgresPassword")))
                    {
                        var connectionBuilder = new NpgsqlConnectionStringBuilder(Configuration.GetConnectionString("PostgreSQL"))
                        {
                            Password = Configuration["PostgresPassword"]
                        };
                        options.UseNpgsql(connectionBuilder.ConnectionString);
                    }
                    else
                    {
                        options.UseNpgsql(Configuration.GetConnectionString("PostgreSQL"));
                    }
                });

                services.AddScoped<IDbContextFactory<AppDb>>(p =>
                    p.GetRequiredService<IDbContextFactory<PostgreSqlDbContext>>());

                services.AddScoped<AppDb, PostgreSqlDbContext>(p =>
                    p.GetRequiredService<IDbContextFactory<PostgreSqlDbContext>>().CreateDbContext());
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

            var trustedOrigins = Configuration.GetSection("ApplicationOptions:TrustedCorsOrigins").Get<string[]>();

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

            var knownProxies = Configuration.GetSection("ApplicationOptions:KnownProxies").Get<string[]>();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.ForwardLimit = null;

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
                options.EnableDetailedErrors = IsDev;
                options.MaximumReceiveMessageSize = 500_000;
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
            services.AddTransient<IDataService, DataService>();
            services.AddScoped<IApplicationConfig, ApplicationConfig>();
            services.AddScoped<ApiAuthorizationFilter>();
            services.AddScoped<ExpiringTokenFilter>();
            services.AddHostedService<CleanupService>();
            services.AddHostedService<ScriptScheduler>();
            services.AddScoped<RemoteControlFilterAttribute>();
            services.AddScoped<IUpgradeService, UpgradeService>();
            services.AddScoped<IToastService, ToastService>();
            services.AddScoped<IModalService, ModalService>();
            services.AddScoped<IJsInterop, JsInterop>();
            services.AddScoped<ICircuitConnection, CircuitConnection>();
            services.AddScoped(x => (CircuitHandler)x.GetRequiredService<ICircuitConnection>());
            services.AddSingleton<ICircuitManager, CircuitManager>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IClientAppState, ClientAppState>();
            services.AddScoped<IExpiringTokenService, ExpiringTokenService>();
            services.AddScoped<IScriptScheduleDispatcher, ScriptScheduleDispatcher>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            AppDb context,
            IDataService dataService,
            ILoggerFactory loggerFactory)
        {
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                if (bool.Parse(Configuration["ApplicationOptions:UseHsts"]))
                {
                    app.UseHsts();
                }
                if (bool.Parse(Configuration["ApplicationOptions:RedirectToHttps"]))
                {
                    app.UseHttpsRedirection();
                }
            }

            app.UseMiddleware<ClickOnceMiddleware>();

            ConfigureStaticFiles(app, env);

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Remotely API V1");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("TrustedOriginPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<AgentHub>("/AgentHub", options =>
                {
                    options.ApplicationMaxBufferSize = 500_000;
                    options.TransportMaxBufferSize = 500_000;
                });
                endpoints.MapHub<CasterHub>("/CasterHub", options =>
                {
                    options.ApplicationMaxBufferSize = 100_000;
                    options.TransportMaxBufferSize = 100_000;
                });
                endpoints.MapHub<ViewerHub>("/ViewerHub", options =>
                {
                    options.ApplicationMaxBufferSize = 100_000;
                    options.TransportMaxBufferSize = 100_000;
                });

                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            if (context.Database.IsRelational())
            {
                context.Database.Migrate();
            }

            loggerFactory.AddProvider(new DbLoggerProvider(env, app.ApplicationServices));
            dataService.SetAllDevicesNotOnline();
            dataService.CleanupOldRecords();
        }


        private static void ConfigureStaticFiles(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // TODO: This redirects downloads from the old location to the new one.
            // Remove after a few releases.
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.HasValue &&
                    context.Request.Path.Value.Contains("/Downloads/R", StringComparison.OrdinalIgnoreCase))
                {
                    var redirectUrl = context.Request.GetDisplayUrl().Replace("/Downloads/", "/Content/");
                    context.Response.Redirect(redirectUrl);
                    return;
                }
                await next();
            });

            var provider = new FileExtensionContentTypeProvider();
            // Add new mappings
            provider.Mappings[".ps1"] = "application/octet-stream";
            provider.Mappings[".exe"] = "application/octet-stream";
            provider.Mappings[".dll"] = "application/octet-stream";
            provider.Mappings[".appimage"] = "application/octet-stream";
            provider.Mappings[".zip"] = "application/octet-stream";
            provider.Mappings[".config"] = "application/octet-stream";
            app.UseStaticFiles();
            var contentPath = Path.Combine(env.WebRootPath, "Content");

            if (Directory.Exists(contentPath))
            {
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(env.WebRootPath, "Content")),
                    ServeUnknownFileTypes = true,
                    RequestPath = new PathString("/Content"),
                    ContentTypeProvider = provider,
                    DefaultContentType = "application/octet-stream"
                });
            }

            // Needed for Let's Encrypt.
            if (Directory.Exists(Path.Combine(env.ContentRootPath, ".well-known")))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, @".well-known")),
                    RequestPath = new PathString("/.well-known"),
                    ServeUnknownFileTypes = true
                });
            }
        }
    }
}
