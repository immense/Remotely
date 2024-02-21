using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace Remotely.Server.Data;

public class SqliteDbContextDesignTime : IDesignTimeDbContextFactory<SqliteDbContext>
{
    public SqliteDbContext CreateDbContext(string[] args)
    {
        var appSettings = new Dictionary<string, string?>
        {
            ["ConnectionStrings:SQLite"] = "Data Source=remotely.db",
            ["ApplicationOptions:DbProvider"] = "sqlite"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(appSettings)
            .Build();

        return new SqliteDbContext(configuration, new DesignTimeWebHostEnvironment());
    }

}
public class SqlServerDbContextDesignTime : IDesignTimeDbContextFactory<SqlServerDbContext>
{
    public SqlServerDbContext CreateDbContext(string[] args)
    {
        var appSettings = new Dictionary<string, string?>
        {
            ["ConnectionStrings:SqlServer"] = "Server=(localdb)\\mssqllocaldb;Database=Remotely-Server-53bc9b9d-9d6a-45d4-8429-2a2761773502;Trusted_Connection=True;MultipleActiveResultSets=true",
            ["ApplicationOptions:DbProvider"] = "SqlServer"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(appSettings)
            .Build();

        return new SqlServerDbContext(configuration, new DesignTimeWebHostEnvironment());
    }
}

public class PostgreSqlDbContextDesignTime : IDesignTimeDbContextFactory<PostgreSqlDbContext>
{
    public PostgreSqlDbContext CreateDbContext(string[] args)
    {
        var appSettings = new Dictionary<string, string?>
        {
            ["ConnectionStrings:PostgreSql"] = "Host=localhost;Database=Remotely;Username=postgres;",
            ["ApplicationOptions:DbProvider"] = "PostgreSql"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(appSettings)
            .Build();

        return new PostgreSqlDbContext(configuration, new DesignTimeWebHostEnvironment());
    }
}


public class DesignTimeWebHostEnvironment : IWebHostEnvironment
{
    public string WebRootPath { get; set; } = string.Empty;
    public IFileProvider WebRootFileProvider { get; set; } = default!;
    public string ApplicationName { get; set; } = string.Empty;
    public IFileProvider ContentRootFileProvider { get; set; } = default!;
    public string ContentRootPath { get; set; } = string.Empty;
    public string EnvironmentName { get; set; } = "Development";
}