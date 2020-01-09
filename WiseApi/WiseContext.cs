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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => 
            optionsBuilder.UseMySql("Server=localhost;Database=wise;Uid=wise;Pwd=SuK1jo1Eb44e;");

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

            base.OnModelCreating(modelBuilder);
        }
    }
}
