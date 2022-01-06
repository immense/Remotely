using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Data
{
    public class SqliteDbContext : AppDb
    {
        private readonly IConfiguration _configuration;

        public SqliteDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(_configuration.GetConnectionString("SQLite"));
            base.OnConfiguring(options);
        }
    }
}
