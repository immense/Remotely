using System;
using System.Collections.Generic;
using System.Text;
using Remotely.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq;
using System.Text.Json;

namespace Remotely.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> context)
            : base(context)
        {
        }

        public DbSet<ApiToken> ApiTokens { get; set; }

        public DbSet<CommandResult> CommandResults { get; set; }

        public DbSet<Device> Devices { get; set; }

        public DbSet<Organization> Organizations { get; set; }

        public new DbSet<RemotelyUser> Users { get; set; }

        public DbSet<EventLog> EventLogs { get; set; }

        public DbSet<SharedFile> SharedFiles { get; set; }

        public DbSet<InviteLink> InviteLinks { get; set; }

        public DbSet<DeviceGroup> DeviceGroups { get; set; }

        public DbSet<UserDevicePermission> PermissionLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.Entity<IdentityUser>().ToTable("RemotelyUsers");

            builder.Entity<Organization>()
                .HasMany(x => x.Devices)
                .WithOne(x=>x.Organization);
            builder.Entity<Organization>()
                .HasMany(x => x.RemotelyUsers)
                .WithOne(x=> x.Organization);
            builder.Entity<Organization>()
                .HasMany(x => x.CommandContexts)
                .WithOne(x => x.Organization);
            builder.Entity<Organization>()
                .HasMany(x => x.EventLogs)
                .WithOne(x => x.Organization);
            builder.Entity<DeviceGroup>()
                .HasMany(x => x.Devices)
                .WithOne(x => x.DeviceGroup);
            builder.Entity<DeviceGroup>()
                .HasMany(x => x.PermissionLinks);
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


            builder.Entity<CommandResult>()
                .Property(x=>x.TargetDeviceIDs)
                .HasConversion(
                    x => JsonSerializer.Serialize(x, null),
                    x => JsonSerializer.Deserialize<string[]>(x, null));
            builder.Entity<CommandResult>()
               .Property(x => x.PSCoreResults)
               .HasConversion(
                   x => JsonSerializer.Serialize(x, null),
                   x => JsonSerializer.Deserialize<List<PSCoreCommandResult>>(x, null));
            builder.Entity<CommandResult>()
                .Property(x => x.CommandResults)
                .HasConversion(
                    x => JsonSerializer.Serialize(x, null),
                    x => JsonSerializer.Deserialize<List<GenericCommandResult>>(x, null));

            builder.Entity<GenericCommandResult>()
                .HasNoKey();

            builder.Entity<PSCoreCommandResult>()
               .HasNoKey();


            builder.Entity<RemotelyUser>()
               .HasOne(x => x.Organization)
               .WithMany(x => x.RemotelyUsers);

            builder.Entity<RemotelyUser>()
                .HasMany(x => x.PermissionLinks);

            builder.Entity<RemotelyUser>()
                .Property(x => x.UserOptions)
                .HasConversion(
                    x => JsonSerializer.Serialize(x, null),
                    x => JsonSerializer.Deserialize<RemotelyUserOptions>(x, null));

            builder.Entity<RemotelyUser>()
                .HasIndex(x => x.UserName);

            builder.Entity<Device>()
                .Property(x => x.Drives)
                .HasConversion(
                    x => JsonSerializer.Serialize(x, null),
                    x => JsonSerializer.Deserialize<List<Drive>>(x, null));

            builder.Entity<Device>()
                .HasIndex(x => x.DeviceName);

            builder.Entity<ApiToken>()
                .HasIndex(x => x.Token);

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
                    var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
                                                                                || p.PropertyType == typeof(DateTimeOffset?));
                    foreach (var property in properties)
                    {
                        builder
                             .Entity(entityType.Name)
                             .Property(property.Name)
                             .HasConversion(new DateTimeOffsetToBinaryConverter());
                    }
                }
            }

        }
    }
}
