using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Remotely.Server.Data
{
    public class AppDb : IdentityDbContext
    {
        private static ValueComparer<string[]> _stringArrayComparer = new(
                    (a, b) => a.SequenceEqual(b),
                    c => c.Aggregate(0, (a, b) => HashCode.Combine(a, b.GetHashCode())),
                    c => c.ToArray());

        public AppDb(DbContextOptions context)
                    : base(context)
        {
        }

        public DbSet<Alert> Alerts { get; set; }

        public DbSet<ApiToken> ApiTokens { get; set; }

        public DbSet<DeviceGroup> DeviceGroups { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<EventLog> EventLogs { get; set; }
        public DbSet<InviteLink> InviteLinks { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<ScriptRun> ScriptRuns { get; set; }
        public DbSet<SavedScript> SavedScripts { get; set; }
        public DbSet<ScriptSchedule> ScriptSchedules { get; set; }
        public DbSet<ScriptResult> ScriptResults { get; set; }
        public DbSet<SharedFile> SharedFiles { get; set; }
        public new DbSet<RemotelyUser> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.Entity<IdentityUser>().ToTable("RemotelyUsers");

            builder.Entity<Organization>()
                .HasMany(x => x.Devices)
                .WithOne(x => x.Organization);
            builder.Entity<Organization>()
                .HasMany(x => x.RemotelyUsers)
                .WithOne(x => x.Organization);
            builder.Entity<Organization>()
                .HasMany(x => x.EventLogs)
                .WithOne(x => x.Organization);
            builder.Entity<Organization>()
                .HasMany(x => x.DeviceGroups)
                .WithOne(x => x.Organization);
            builder.Entity<Organization>()
              .HasMany(x => x.InviteLinks)
              .WithOne(x => x.Organization);
            builder.Entity<Organization>()
              .HasMany(x => x.SharedFiles)
              .WithOne(x => x.Organization);
            builder.Entity<Organization>()
              .HasMany(x => x.ApiTokens)
              .WithOne(x => x.Organization);
            builder.Entity<Organization>()
                .HasMany(x => x.Alerts)
                .WithOne(x => x.Organization);
            builder.Entity<Organization>()
                .HasMany(x => x.ScriptRuns)
                .WithOne(x => x.Organization);
            builder.Entity<Organization>()
                .HasMany(x => x.ScriptSchedules)
                .WithOne(x => x.Organization);
            builder.Entity<Organization>()
                .HasMany(x => x.ScriptResults)
                .WithOne(x => x.Organization);
            builder.Entity<Organization>()
                .HasMany(x => x.SavedScripts)
                .WithOne(x => x.Organization);

            builder.Entity<RemotelyUser>()
               .HasOne(x => x.Organization)
               .WithMany(x => x.RemotelyUsers);

            builder.Entity<RemotelyUser>()
                .HasMany(x => x.DeviceGroups)
                .WithMany(x => x.Users);
            builder.Entity<RemotelyUser>()
                .HasMany(x => x.Alerts)
                .WithOne(x => x.User);
            builder.Entity<RemotelyUser>()
                .Property(x => x.UserOptions)
                .HasConversion(
                    x => JsonSerializer.Serialize(x, null),
                    x => JsonSerializer.Deserialize<RemotelyUserOptions>(x, null));
            builder.Entity<RemotelyUser>()
                .HasMany(x => x.SavedScripts)
                .WithOne(x => x.Creator);
            builder.Entity<RemotelyUser>()
                .HasMany(x => x.ScriptSchedules)
                .WithOne(x => x.Creator);

            builder.Entity<RemotelyUser>()
                .HasIndex(x => x.UserName);

            builder.Entity<Device>()
                .Property(x => x.Drives)
                .HasConversion(
                    x => JsonSerializer.Serialize(x, null),
                    x => JsonSerializer.Deserialize<List<Drive>>(x, null));
            builder.Entity<Device>()
               .Property(x => x.Drives)
               .Metadata.SetValueComparer(new ValueComparer<List<Drive>>(true));
            builder.Entity<Device>()
                .HasIndex(x => x.DeviceName);
            builder.Entity<Device>()
                .HasMany(x => x.Alerts)
                .WithOne(x => x.Device);
            builder.Entity<Device>()
                .HasMany(x => x.ScriptRuns)
                .WithMany(x => x.Devices);
            builder.Entity<Device>()
                .HasMany(x => x.ScriptRunsCompleted)
                .WithMany(x => x.DevicesCompleted);
            builder.Entity<Device>()
                .HasMany(x => x.ScriptSchedules)
                .WithMany(x => x.Devices);

            builder.Entity<DeviceGroup>()
                .HasMany(x => x.Devices);
            builder.Entity<DeviceGroup>()
                .HasMany(x => x.ScriptSchedules)
                .WithMany(x => x.DeviceGroups);

            builder.Entity<ScriptRun>()
                .HasMany(x => x.Devices)
                .WithMany(x => x.ScriptRuns);
            builder.Entity<ScriptRun>()
                .HasMany(x => x.DevicesCompleted)
                .WithMany(x => x.ScriptRunsCompleted);

            builder.Entity<ScriptResult>()
              .Property(x => x.ErrorOutput)
              .HasConversion(
                  x => JsonSerializer.Serialize(x, null),
                  x => JsonSerializer.Deserialize<string[]>(x, null))
              .Metadata
              .SetValueComparer(_stringArrayComparer);

            builder.Entity<ScriptResult>()
              .Property(x => x.StandardOutput)
              .HasConversion(
                  x => JsonSerializer.Serialize(x, null),
                  x => JsonSerializer.Deserialize<string[]>(x, null))
              .Metadata
              .SetValueComparer(_stringArrayComparer);

            builder.Entity<Alert>()
                .HasOne(x => x.User)
                .WithMany(x => x.Alerts);


            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
                // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
                // To work around this, when the SQLite database provider is used, all model properties of type DateTimeOffset
                // use the DateTimeOffsetToBinaryConverter
                // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
                // This only supports millisecond precision, but should be sufficient for most use cases.
                foreach (var entityType in builder.Model.GetEntityTypes())
                {
                    if (entityType.IsKeyless)
                    {
                        continue;
                    }
                    var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
                                                                                || p.PropertyType == typeof(DateTimeOffset?));
                    foreach (var property in properties)
                    {
                        builder
                             .Entity(entityType.Name)
                             .Property(property.Name)
                             .HasConversion(new DateTimeOffsetToStringConverter());
                    }
                }
            }

        }
    }
}
