using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zira.Data.Models;

namespace Zira.Data
{
    public class EntityContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public DbSet<ApplicationUser> Users { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Reminder> Reminders { get; set; }

        public EntityContext(DbContextOptions<EntityContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>()
                .HasOne(i => i.User)
                .WithMany(x => x.Transactions)
                .HasForeignKey(i => i.UserId);

            modelBuilder.Entity<Budget>()
                .HasOne(b => b.User)
                .WithMany(x => x.Budgets)
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<Reminder>()
                .HasOne(r => r.User)
                .WithMany(x => x.Reminders)
                .HasForeignKey(r => r.UserId);
        }
    }
}