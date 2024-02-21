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
    private static WebApplicationBuilder? _builder;
    private static WebApplication? _webApp;

    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        _builder = WebApplication.CreateBuilder();

        _builder.Services.AddDbContext<AppDb, TestingDbContext>(
            contextLifetime: ServiceLifetime.Transient,
            optionsLifetime: ServiceLifetime.Transient);

        _builder.Services.
            AddIdentity<RemotelyUser, IdentityRole>(options => options.Stores.MaxLengthForKeys = 128)
                .AddEntityFrameworkStores<AppDb>()
                .AddDefaultTokenProviders();

        _builder.Services.AddTransient<IAppDbFactory, AppDbFactory>();
        _builder.Services.AddTransient<IDataService, DataService>();
        _builder.Services.AddTransient<IEmailSenderEx, EmailSenderEx>();

        _webApp = _builder.Build();
        ServiceProvider = _webApp.Services;
    }
}

