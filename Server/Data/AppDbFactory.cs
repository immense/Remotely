using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Remotely.Server.Services;
using System;

namespace Remotely.Server.Data;

public interface IAppDbFactory
{
    AppDb GetContext();
}

public class AppDbFactory : IAppDbFactory
{
    private readonly IApplicationConfig _appConfig;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _hostEnv;

    public AppDbFactory(
        IApplicationConfig appConfig, 
        IConfiguration configuration,
        IWebHostEnvironment hostEnv)
    {
        _appConfig = appConfig;
        _configuration = configuration;
        _hostEnv = hostEnv;
    }

    public AppDb GetContext()
    {
        return _appConfig.DBProvider.ToLower() switch
        {
            "sqlite" => new SqliteDbContext(_configuration, _hostEnv),
            "sqlserver" => new SqlServerDbContext(_configuration, _hostEnv),
            "postgresql" => new PostgreSqlDbContext(_configuration, _hostEnv),
            "inmemory" => new TestingDbContext(_hostEnv),
            _ => throw new ArgumentException("Unknown DB provider."),
        };
    }
}
