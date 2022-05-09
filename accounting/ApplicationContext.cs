using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace accounting
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Worker> Workers { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Position> Positions { get; set; } = null!;
        public DbSet<AdvanceReport> AdvanceReports { get; set; } = null!;
        public DbSet<AccountingRecord> AccountingRecords { get; set; } = null!;

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=accounting;Username=postgres;Password=postgres");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Worker>()
                    .HasMany(c => c.Positions)
                    .WithMany(s => s.Workers);

            modelBuilder.Entity<AdvanceReport>()
                    .HasMany(c => c.AccountingRecords);
        }
    }
}
