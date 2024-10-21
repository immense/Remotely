using Microsoft.EntityFrameworkCore;

namespace Remotely.Server.Data;

public class SqlServerDbContext : AppDb
{
    private readonly IConfiguration _configuration;

    public SqlServerDbContext(IConfiguration configuration, IWebHostEnvironment hostEnv)
        : base(hostEnv)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(_configuration.GetConnectionString("SQLServer"));
        base.OnConfiguring(options);
    }
}
