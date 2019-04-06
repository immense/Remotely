using System;
using System.Collections.Generic;
using System.Text;
using Remotely_Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Remotely_Server.Data
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

        public DbSet<PermissionGroup> PermissionGroups { get; set; }

        public DbSet<UserPermissionLink> UserPermissionLinks { get; set; }
        public DbSet<DevicePermissionLink> DevicePermissionLinks { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<IdentityUser>().ToTable("RemotelyUsers");
            builder.Entity<RemotelyUser>().ToTable("RemotelyUsers");

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
            builder.Entity<Organization>()
                .HasMany(x => x.PermissionGroups)
                .WithOne(x => x.Organization);
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


            builder.Entity<UserPermissionLink>()
                .HasKey(x => new { x.PermissionGroupID, x.RemotelyUserID });

            builder.Entity<UserPermissionLink>()
                .HasOne(x => x.RemotelyUser)
                .WithMany(y => y.UserPermissionLinks)
                .HasForeignKey(x => x.RemotelyUserID);

            builder.Entity<UserPermissionLink>()
               .HasOne(x => x.PermissionGroup)
               .WithMany(y => y.UserPermissionLinks)
               .HasForeignKey(x => x.PermissionGroupID);


            builder.Entity<DevicePermissionLink>()
                .HasKey(x => new { x.PermissionGroupID, x.DeviceID });

            builder.Entity<UserPermissionLink>()
                .HasOne(x => x.RemotelyUser)
                .WithMany(y => y.UserPermissionLinks)
                .HasForeignKey(x => x.RemotelyUserID);

            builder.Entity<UserPermissionLink>()
               .HasOne(x => x.PermissionGroup)
               .WithMany(y => y.UserPermissionLinks)
               .HasForeignKey(x => x.PermissionGroupID);

        }
    }
}
