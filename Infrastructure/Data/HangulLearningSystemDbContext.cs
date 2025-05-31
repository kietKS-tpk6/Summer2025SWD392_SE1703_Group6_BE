using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
namespace Infrastructure.Data
{
    public class HangulLearningSystemDbContext : DbContext
    {
        public HangulLearningSystemDbContext(DbContextOptions<HangulLearningSystemDbContext> options)
      : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(a => a.Role)
                .HasConversion<string>();
            modelBuilder.Entity<Account>()
                .Property(a => a.Status)
                .HasConversion<string>();
            modelBuilder.Entity<Account>()
                .Property(a => a.Gender)
                .HasConversion<string>();
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Account> Accounts { get; set; }
    }
}
