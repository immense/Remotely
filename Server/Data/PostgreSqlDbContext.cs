using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Data
{
    public class PostgreSqlDbContext : AppDb
    {
        private readonly IConfiguration _configuration;

        public PostgreSqlDbContext(DbContextOptions context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // Password should be set in User Secrets in dev environment.
            // See https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1
            if (!string.IsNullOrWhiteSpace(_configuration.GetValue<string>("PostgresPassword")))
            {
                var connectionBuilder = new NpgsqlConnectionStringBuilder(_configuration.GetConnectionString("PostgreSQL"))
                {
                    Password = _configuration["PostgresPassword"]
                };
                options.UseNpgsql(connectionBuilder.ConnectionString);
            }
            else
            {
                options.UseNpgsql(_configuration.GetConnectionString("PostgreSQL"));
            }
            options.ConfigureWarnings(x => x.Ignore(RelationalEventId.MultipleCollectionIncludeWarning));

            options.LogTo((message) => System.Diagnostics.Debug.Write(message));
        }
    }
}
