using Microsoft.Extensions.Configuration;
using Remotely.Server.Services;
using System;

namespace Remotely.Server.Data
{
    public interface IAppDbFactory
    {
        AppDb GetContext();
    }

    public class AppDbFactory : IAppDbFactory
    {
        private readonly IApplicationConfig _appConfig;
        private readonly IConfiguration _configuration;

        public AppDbFactory(IApplicationConfig appConfig, IConfiguration configuration)
        {
            _appConfig = appConfig;
            _configuration = configuration;
        }

        public AppDb GetContext()
        {
            return _appConfig.DBProvider.ToLower() switch
            {
                "sqlite" => new SqliteDbContext(_configuration),
                "sqlserver" => new SqlServerDbContext(_configuration),
                "postgresql" => new PostgreSqlDbContext(_configuration),
                "inmemory" => new TestingDbContext(),
                _ => throw new ArgumentException("Unknown DB provider."),
            };
        }
    }
}
