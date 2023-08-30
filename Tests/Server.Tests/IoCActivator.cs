using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotely.Server.API;
using Remotely.Server.Data;
using Remotely.Server.Services;
using Remotely.Shared.Entities;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Remotely.Server.Tests;

[TestClass]
public class IoCActivator
{
    public static IServiceProvider ServiceProvider { get; set; } = null!;
    private static IWebHostBuilder? _builder;

    public static void Activate()
    {
        if (_builder is null)
        {
            _builder = WebHost.CreateDefaultBuilder()
               .UseStartup<Startup>()
               .CaptureStartupErrors(true)
               .ConfigureAppConfiguration(config =>
               {
                   config.AddInMemoryCollection(new Dictionary<string, string?>()
                   {
                       ["ApplicationOptions:DBProvider"] = "InMemory"
                   });
               });

            _builder.Build();
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
        services.AddDbContext<AppDb, TestingDbContext>();

        services.AddIdentity<RemotelyUser, IdentityRole>(options => options.Stores.MaxLengthForKeys = 128)
         .AddEntityFrameworkStores<AppDb>()
         .AddDefaultUI()
         .AddDefaultTokenProviders();

        services.AddTransient<IAppDbFactory, AppDbFactory>();
        services.AddTransient<IDataService, DataService>();
        services.AddTransient<IApplicationConfig, ApplicationConfig>();
        services.AddTransient<IEmailSenderEx, EmailSenderEx>();

        IoCActivator.ServiceProvider = services.BuildServiceProvider();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}
