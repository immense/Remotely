using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotely.Agent.Interfaces;
using Remotely.Agent.Services;
using Remotely.Server.API;
using Remotely.Server.Data;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;

namespace Remotely.Tests
{
    [TestClass]
    public class IoCActivator
    {
        public static IServiceProvider ServiceProvider { get; set; }
        private static IWebHostBuilder builder;

        public static void Activate()
        {
            if (builder is null)
            {
                builder = WebHost.CreateDefaultBuilder()
                   .UseStartup<Startup>()
                   .CaptureStartupErrors(true);

                builder.Build();
            }
        }


        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Activate();
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextFactory<AppDb>(options =>
            {
                options.UseInMemoryDatabase("Remotely");
            });

            services.AddScoped(p =>
                p.GetRequiredService<IDbContextFactory<AppDb>>().CreateDbContext());

            services.AddIdentity<RemotelyUser, IdentityRole>(options => options.Stores.MaxLengthForKeys = 128)
             .AddEntityFrameworkStores<AppDb>()
             .AddDefaultUI()
             .AddDefaultTokenProviders();

            services.AddTransient<IDataService, DataService>();
            services.AddTransient<IApplicationConfig, ApplicationConfig>();
            services.AddTransient<IEmailSenderEx, EmailSenderEx>();

            if (EnvironmentHelper.IsWindows)
            {
                services.AddTransient<IDeviceInformationService, DeviceInformationServiceWin>();
            }
            else if (EnvironmentHelper.IsLinux)
            {
                services.AddTransient<IDeviceInformationService, DeviceInformationServiceLinux>();
            }

            IoCActivator.ServiceProvider = services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }


}
