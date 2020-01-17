using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WiseDomain;

namespace WiseApi
{
    public class WiseContext : DbContext
    {
        public DbSet<ReportRun> Runs { get; set; }

        public DbSet<DataProviderConfiguration> Providers { get; set; }

        public DbSet<ReportConfiguration> Reports { get; set; }

        public DbSet<ReportCustomParameter> Parameters { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserGroup> Groups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReportRun>().Property(p => p.StartedAt).ValueGeneratedOnAdd();
            modelBuilder.Entity<ReportRun>().Property(p => p.CustomParameterValues).HasConversion(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<List<ParameterValue>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
                );

            modelBuilder.Entity<DataProviderConfiguration>().Property(p => p.CreatedAt).ValueGeneratedOnAdd();
            modelBuilder.Entity<DataProviderConfiguration>().Property(p => p.UpdatedAt).ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<ReportConfiguration>().Property(p => p.CreatedAt).ValueGeneratedOnAdd();
            modelBuilder.Entity<ReportConfiguration>().Property(p => p.UpdatedAt).ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<User>().HasIndex(b => b.Login).IsUnique();

            // many-to-many User <-> Groups
            modelBuilder.Entity<UserGroupJoin>()
                .HasKey(t => new { t.UserId, t.GroupId });

            modelBuilder.Entity<UserGroupJoin>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.UserGroups)
                .HasForeignKey(pt => pt.UserId);

            modelBuilder.Entity<UserGroupJoin>()
                .HasOne(pt => pt.Group)
                .WithMany(t => t.UserGroups)
                .HasForeignKey(pt => pt.GroupId);

            // many-to-many Report <-> Groups
            modelBuilder.Entity<ReportGroupJoin>()
                .HasKey(t => new { t.ReportId, t.GroupId });

            modelBuilder.Entity<ReportGroupJoin>()
                .HasOne(pt => pt.ReportConfiguration)
                .WithMany(p => p.ReportGroups)
                .HasForeignKey(pt => pt.ReportId);

            modelBuilder.Entity<ReportGroupJoin>()
                .HasOne(pt => pt.ReportGroup)
                .WithMany(t => t.ReportGroups)
                .HasForeignKey(pt => pt.GroupId);

            // many-to-many ReportGroup <-> UserGroup
            modelBuilder.Entity<ReportGroupUserGroupJoin>()
                .HasKey(t => new { t.ReportGroupId, t.UserGroupId });

            modelBuilder.Entity<ReportGroupUserGroupJoin>()
                .HasOne(pt => pt.ReportGroup)
                .WithMany(p => p.AllowedUserGroups)
                .HasForeignKey(pt => pt.ReportGroupId);
            
            // initial data
            modelBuilder.Entity<UserGroup>().HasData(new UserGroup() {Id = 1, IsAdmin = false, Title = "Пользователь"},
                new UserGroup() {Id = 2, IsAdmin = true, Title = "Администратор"});

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<WiseDomain.ReportGroup> ReportGroup { get; set; }
    }
}
