using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Remotely.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Server.Data
{
    public class TestingDbContext : AppDb
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseInMemoryDatabase("Remotely");
            base.OnConfiguring(options);
        }
    }
}
