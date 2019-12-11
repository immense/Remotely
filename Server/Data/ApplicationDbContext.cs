using System;
using System.Collections.Generic;
using System.Text;
using Remotely.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Remotely.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> context)
            : base(context)
        {
        }

        public DbSet<CommandContext> CommandContexts { get; set; }

        public DbSet<Device> Devices { get; set; }

        public DbSet<Organization> Organizations { get; set; }

        public new DbSet<RemotelyUser> Users { get; set; }

        public DbSet<EventLog> EventLogs { get; set; }

        public DbSet<SharedFile> SharedFiles { get; set; }

        public DbSet<InviteLink> InviteLinks { get; set; }

        public DbSet<DeviceGroup> DeviceGroups { get; set; }

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
            builder.Entity<Organization>()
              .HasMany(x => x.InviteLinks)
              .WithOne(x => x.Organization);
            builder.Entity<Organization>()
              .HasMany(x => x.SharedFiles)
              .WithOne(x => x.Organization);


            builder.Entity<CommandContext>()
                .Property(x=>x.TargetDeviceIDs)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<string[]>(x));
            builder.Entity<CommandContext>()
               .Property(x => x.PSCoreResults)
               .HasConversion(
                   x => JsonConvert.SerializeObject(x),
                   x => JsonConvert.DeserializeObject<List<PSCoreCommandResult>>(x));
            builder.Entity<CommandContext>()
                .Property(x => x.CommandResults)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<List<GenericCommandResult>>(x));


			builder.Entity<RemotelyUser>()
               .HasOne(x => x.Organization);
            builder.Entity<RemotelyUser>()
                .Property(x => x.UserOptions)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<RemotelyUserOptions>(x));

            builder.Entity<Device>()
                .Property(x => x.Drives)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<List<Drive>>(x));

        }
    }
}
