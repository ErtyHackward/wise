using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WiseDomain;

namespace WiseApi
{
    public class WiseContext : DbContext
    {
        public DbSet<ReportExecution> Executions { get; set; }

        public DbSet<DataProviderConfiguration> DataProviderConfigurations { get; set; }

        public DbSet<ReportConfiguration> ReportConfigurations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => 
            optionsBuilder.UseMySql("Server=localhost;Database=wise;Uid=wise;Pwd=SuK1jo1Eb44e;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReportExecution>().Property(p => p.ExecutionStartedAt).ValueGeneratedOnAdd();

            modelBuilder.Entity<DataProviderConfiguration>().Property(p => p.CreatedAt).ValueGeneratedOnAdd();
            modelBuilder.Entity<DataProviderConfiguration>().Property(p => p.UpdatedAt).ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<ReportConfiguration>().Property(p => p.CreatedAt).ValueGeneratedOnAdd();
            modelBuilder.Entity<ReportConfiguration>().Property(p => p.UpdatedAt).ValueGeneratedOnAddOrUpdate();

            base.OnModelCreating(modelBuilder);
        }
    }
}
