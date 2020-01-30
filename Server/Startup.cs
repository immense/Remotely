using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Remotely.Server.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.IO;
using Remotely.Server.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.SignalR;
using Remotely.Shared.Models;
using Microsoft.AspNetCore.Http.Connections;
using Remotely.Shared.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Newtonsoft.Json;
using System.Net;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;

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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var dbProvider = Configuration["ApplicationOptions:DBProvider"].ToLower();
            if (dbProvider == "sqlite")
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("SQLite")));
            }
            else if (dbProvider == "sqlserver")
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        Configuration.GetConnectionString("SQLServer")));
            }
            else if (dbProvider == "postgresql")
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("PostgreSQL")));
            }

            services.AddIdentity<RemotelyUser, IdentityRole>(options => options.Stores.MaxLengthForKeys = 128)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            var remoteControlAuthentication = Configuration.GetSection("ApplicationOptions:RemoteControlRequiresAuthentication").Get<bool>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RemoteControlPolicy", policy =>
                {
                    if (remoteControlAuthentication)
                    {
                        policy.RequireAuthenticatedUser();
                    }
                    else
                    {
                        policy.RequireAssertion((context) => true);
                    }
                    policy.Build();
                });
            });       

            services.ConfigureApplicationCookie(cookieOptions =>
            {
                cookieOptions.Cookie.SameSite = SameSiteMode.None;
            });


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
            if (knownProxies != null)
            {
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    foreach (var proxy in knownProxies)
                    {
                        options.KnownProxies.Add(IPAddress.Parse(proxy));
                        options.ForwardLimit = 2;
                    }
                });
            }

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = new PascalCasePolicy();
                });

            services.AddSignalR(options =>
                {
                    options.EnableDetailedErrors = IsDev;
                    options.MaximumReceiveMessageSize = 20000000;
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
            services.AddScoped<IEmailSender, EmailSender>();;
            services.AddScoped<DataService>();
            services.AddScoped<RemoteControlSessionRecorder>();
            services.AddSingleton<ApplicationConfig>();
            services.AddSingleton<RandomGenerator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context, DataService dataService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            app.UseCookiePolicy();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

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
                routeBuilder.MapHub<BrowserSocketHub>("/BrowserHub", options =>
                {
                    options.ApplicationMaxBufferSize = 500000;
                    options.TransportMaxBufferSize = 500000;
                });
                routeBuilder.MapHub<DeviceSocketHub>("/DeviceHub", options =>
                {
                    options.ApplicationMaxBufferSize = 500000;
                    options.TransportMaxBufferSize = 500000;
                });
                routeBuilder.MapHub<RCDeviceSocketHub>("/RCDeviceHub", options =>
                {
                    options.ApplicationMaxBufferSize = 2000000;
                    options.TransportMaxBufferSize = 2000000;
                });
                routeBuilder.MapHub<RCBrowserSocketHub>("/RCBrowserHub", options =>
                {
                    options.ApplicationMaxBufferSize = 2000000;
                    options.TransportMaxBufferSize = 2000000;
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
                dataService.WriteEvent(ex);
            }
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
