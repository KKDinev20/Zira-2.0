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
        public DbSet<ReminderSettings> ReminderSettings { get; set; }
        public DbSet<SavingsGoal> SavingsGoals { get; set; }

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

            modelBuilder.Entity<ReminderSettings>()
                .HasOne(r => r.User)
                .WithMany(x => x.ReminderSettings)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<SavingsGoal>()
                .HasOne(s => s.User)
                .WithMany(x => x.SavingsGoals)
                .HasForeignKey(s => s.UserId);
        }
    }
}