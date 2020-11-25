using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Npgsql;
using Remotely.Server.Attributes;
using Remotely.Server.Data;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;

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
        private bool IsDev { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabaseDeveloperPageExceptionFilter();
            var dbProvider = Configuration["ApplicationOptions:DBProvider"].ToLower();
            if (dbProvider == "sqlite")
            {
                services.AddDbContext<ApplicationDbContext, SqliteDbContext>(options => 
                    options.UseSqlite(Configuration.GetConnectionString("SQLite")));
            }
            else if (dbProvider == "sqlserver")
            {
                services.AddDbContext<ApplicationDbContext, SqlServerDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("SQLServer")));
            }
            else if (dbProvider == "postgresql")
            {
                services.AddDbContext<ApplicationDbContext, PostgreSqlDbContext>(options =>
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
            }

            services.AddIdentity<RemotelyUser, IdentityRole>(options => options.Stores.MaxLengthForKeys = 128)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

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

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = new PascalCasePolicy();
                });

            services.AddSignalR(options =>
                {
                    options.EnableDetailedErrors = IsDev;
                    options.MaximumReceiveMessageSize = 500_000;
                })
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.PropertyNamingPolicy = new PascalCasePolicy();
                })
                .AddMessagePackProtocol();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Remotely API", Version = "v1" });
            });

            services.AddLogging();
            services.AddScoped<IEmailSenderEx, EmailSenderEx>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IDataService, DataService>();
            services.AddSingleton<IApplicationConfig, ApplicationConfig>();
            services.AddScoped<ApiAuthorizationFilter>();
            services.AddHostedService<CleanupService>();
            services.AddScoped<RemoteControlFilterAttribute>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            ApplicationDbContext context,
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

            ConfigureStaticFiles(app);

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Remotely API V1");
            });

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors("TrustedOriginPolicy");

            app.UseEndpoints(routeBuilder =>
            {
                routeBuilder.MapHub<BrowserHub>("/BrowserHub", options =>
                {
                    options.ApplicationMaxBufferSize = 500_000;
                    options.TransportMaxBufferSize = 500_000;
                });
                routeBuilder.MapHub<AgentHub>("/AgentHub", options =>
                {
                    options.ApplicationMaxBufferSize = 500_000;
                    options.TransportMaxBufferSize = 500_000;
                });
                routeBuilder.MapHub<CasterHub>("/CasterHub", options =>
                {
                    options.ApplicationMaxBufferSize = 100_000;
                    options.TransportMaxBufferSize = 100_000;
                });
                routeBuilder.MapHub<ViewerHub>("/ViewerHub", options =>
                {
                    options.ApplicationMaxBufferSize = 100_000;
                    options.TransportMaxBufferSize = 100_000;
                });

                routeBuilder.MapRazorPages();
                routeBuilder.MapControllers();

            });

            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                dataService.WriteEvent(ex, null);
            }

            loggerFactory.AddProvider(new DbLoggerProvider(env, app.ApplicationServices));
            dataService.SetAllDevicesNotOnline();
            dataService.CleanupOldRecords();
        }


        private void ConfigureStaticFiles(IApplicationBuilder app)
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
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Downloads")),
                ServeUnknownFileTypes = true,
                RequestPath = new PathString("/Downloads"),
                ContentTypeProvider = provider,
                DefaultContentType = "application/octet-stream"
            });
            // Needed for Let's Encrypt.
            if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), ".well-known")))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @".well-known")),
                    RequestPath = new PathString("/.well-known"),
                    ServeUnknownFileTypes = true
                });
            }
        }
    }
}
