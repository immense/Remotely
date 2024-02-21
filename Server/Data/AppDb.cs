using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Remotely.Server.Converters;
using Remotely.Shared.Entities;
using Remotely.Shared.Models;
using System.Text.Json;

namespace Remotely.Server.Data;

public class AppDb : IdentityDbContext
{
    private static readonly ValueComparer<string[]> _stringArrayComparer = new(
                (a, b) => (a ?? Array.Empty<string>()).SequenceEqual(b ?? Array.Empty<string>()),
                c => c.Aggregate(0, (a, b) => HashCode.Combine(a, b.GetHashCode())),
                c => c.ToArray());

    private readonly IWebHostEnvironment _hostEnv;

    public AppDb(IWebHostEnvironment hostEnvironment)
    {
        _hostEnv = hostEnvironment;
    }

    public DbSet<Alert> Alerts { get; set; }

    public DbSet<ApiToken> ApiTokens { get; set; }
    public DbSet<BrandingInfo> BrandingInfos { get; set; }
    public DbSet<DeviceGroup> DeviceGroups { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<InviteLink> InviteLinks { get; set; }
    public DbSet<KeyValueRecord> KeyValueRecords { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<SavedScript> SavedScripts { get; set; }
    public DbSet<ScriptResult> ScriptResults { get; set; }
    public DbSet<ScriptRun> ScriptRuns { get; set; }
    public DbSet<ScriptSchedule> ScriptSchedules { get; set; }
    public DbSet<SharedFile> SharedFiles { get; set; }
    public new DbSet<RemotelyUser> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.ConfigureWarnings(x => x.Ignore(RelationalEventId.MultipleCollectionIncludeWarning));
        options.LogTo((message) => System.Diagnostics.Debug.Write(message));

        if (_hostEnv.IsDevelopment())
        {
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var jsonOptions = JsonSerializerOptions.Default;

        base.OnModelCreating(builder);

        builder.Entity<IdentityUser>().ToTable("RemotelyUsers");

        builder.Entity<Organization>()
            .HasMany(x => x.Devices)
            .WithOne(x => x.Organization);
        builder.Entity<Organization>()
            .HasMany(x => x.RemotelyUsers)
            .WithOne(x => x.Organization);
        builder.Entity<Organization>()
            .HasMany(x => x.DeviceGroups)
            .WithOne(x => x.Organization)
            .OnDelete(DeleteBehavior.ClientCascade);
        builder.Entity<Organization>()
            .HasMany(x => x.InviteLinks)
            .WithOne(x => x.Organization);
        builder.Entity<Organization>()
            .HasMany(x => x.SharedFiles)
            .WithOne(x => x.Organization)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);
        builder.Entity<Organization>()
            .HasMany(x => x.ApiTokens)
            .WithOne(x => x.Organization);
        builder.Entity<Organization>()
            .HasMany(x => x.Alerts)
            .WithOne(x => x.Organization);
        builder.Entity<Organization>()
            .HasMany(x => x.ScriptRuns)
            .WithOne(x => x.Organization)
            .OnDelete(DeleteBehavior.ClientCascade);
        builder.Entity<Organization>()
            .HasMany(x => x.ScriptSchedules)
            .WithOne(x => x.Organization)
            .OnDelete(DeleteBehavior.ClientCascade);
        builder.Entity<Organization>()
            .HasMany(x => x.ScriptResults)
            .WithOne(x => x.Organization);
        builder.Entity<Organization>()
            .HasMany(x => x.SavedScripts)
            .WithOne(x => x.Organization);
        builder.Entity<Organization>()
            .HasOne(x => x.BrandingInfo)
            .WithOne(x => x.Organization)
            .HasForeignKey<BrandingInfo>(x => x.OrganizationId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);


        builder.Entity<RemotelyUser>()
            .HasMany(x => x.DeviceGroups)
            .WithMany(x => x.Users);

        builder.Entity<RemotelyUser>()
            .HasMany(x => x.Alerts)
            .WithOne(x => x.User)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.Entity<RemotelyUser>()
            .Property(x => x.UserOptions)
            .HasConversion(
                x => JsonSerializer.Serialize(x, jsonOptions),
                x => JsonSerializer.Deserialize<RemotelyUserOptions>(x, jsonOptions));

        builder.Entity<RemotelyUser>()
            .HasMany(x => x.SavedScripts)
            .WithOne(x => x.Creator)
            .HasForeignKey(x => x.CreatorId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.Entity<RemotelyUser>()
            .HasMany(x => x.ScriptSchedules)
            .WithOne(x => x.Creator)
            .HasForeignKey(x => x.CreatorId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.Entity<RemotelyUser>()
            .HasIndex(x => x.UserName);

        builder.Entity<Device>()
            .Property(x => x.Drives)
            .HasConversion(
                x => JsonSerializer.Serialize(x, jsonOptions),
                x => TryDeserializeProperty<List<Drive>>(x, jsonOptions));

        builder.Entity<Device>()
           .Property(x => x.Drives)
           .Metadata.SetValueComparer(new ValueComparer<List<Drive>>(true));

        builder.Entity<Device>()
            .HasIndex(x => x.DeviceName);

        builder.Entity<Device>()
            .HasMany(x => x.Alerts)
            .WithOne(x => x.Device)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.Entity<Device>()
            .HasMany(x => x.ScriptRuns)
            .WithMany(x => x.Devices);

        builder.Entity<Device>()
            .HasMany(x => x.ScriptSchedules)
            .WithMany(x => x.Devices);

        builder.Entity<Device>()
            .HasMany(x => x.ScriptResults)
            .WithOne(x => x.Device)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.Entity<Device>()
            .Property(x => x.MacAddresses)
            .HasConversion(
                x => JsonSerializer.Serialize(x, jsonOptions),
                x => DeserializeStringArray(x, jsonOptions),
                valueComparer: _stringArrayComparer);

        builder.Entity<DeviceGroup>()
            .HasMany(x => x.Devices)
            .WithOne(x => x.DeviceGroup)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.Entity<DeviceGroup>()
            .HasMany(x => x.ScriptSchedules)
            .WithMany(x => x.DeviceGroups);

        builder.Entity<ScriptRun>()
            .HasMany(x => x.Devices)
            .WithMany(x => x.ScriptRuns);

        builder.Entity<ScriptRun>()
            .HasMany(x => x.Results)
            .WithOne(x => x.ScriptRun)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);


        builder.Entity<ScriptResult>()
          .Property(x => x.ErrorOutput)
          .HasConversion(
              x => JsonSerializer.Serialize(x, jsonOptions),
              x => DeserializeStringArray(x, jsonOptions))
          .Metadata
          .SetValueComparer(_stringArrayComparer);

        builder.Entity<ScriptResult>()
          .Property(x => x.StandardOutput)
          .HasConversion(
              x => JsonSerializer.Serialize(x, jsonOptions),
              x => DeserializeStringArray(x, jsonOptions))
          .Metadata
          .SetValueComparer(_stringArrayComparer);

        builder.Entity<SavedScript>()
            .HasMany(x => x.ScriptRuns)
            .WithOne(x => x.SavedScript)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.Entity<SavedScript>()
            .HasMany(x => x.ScriptResults)
            .WithOne(x => x.SavedScript)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.Entity<ScriptSchedule>()
            .HasMany(x => x.ScriptRuns)
            .WithOne(x => x.Schedule)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);


        var isSqlite = Database.IsSqlite();
        var isPostgres = Database.IsNpgsql();
        
        if (isSqlite || isPostgres)
        {
            // SQLite and PostgreSQL don't support DateTimeOffset natively (or don't support
            // it correctly), so we need to use a converter.
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (entityType.IsKeyless)
                {
                    continue;
                }

                var properties = entityType.ClrType
                    .GetProperties()
                    .Where(p => 
                        p.PropertyType == typeof(DateTimeOffset) ||
                        p.PropertyType == typeof(DateTimeOffset?));

                foreach (var property in properties)
                {
                    if (isSqlite)
                    {
                        builder
                             .Entity(entityType.Name)
                             .Property(property.Name)
                             .HasConversion(new DateTimeOffsetToStringConverter());
                    }
                    else if (isPostgres)
                    {
                        builder
                             .Entity(entityType.Name)
                             .Property(property.Name)
                             .HasConversion(new PostgresDateTimeOffsetConverter());
                    }
                }
            }
        }

    }

    private static string[] DeserializeStringArray(string value, JsonSerializerOptions jsonOptions)
    {
        try
        {
            if (string.IsNullOrEmpty(value))
            {
                return [];
            }
            return JsonSerializer.Deserialize<string[]>(value, jsonOptions) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private static T TryDeserializeProperty<T>(string value, JsonSerializerOptions jsonOptions)
        where T: new()
    {
        try
        {
            if (string.IsNullOrEmpty(value))
            {
                return new();
            }
            return JsonSerializer.Deserialize<T>(value, jsonOptions) ?? new();
        }
        catch
        {
            return new();
        }
    }
}
