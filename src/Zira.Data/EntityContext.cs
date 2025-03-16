using System;
using System.Collections.Generic;
using System.Linq;
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

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        public EntityContext(DbContextOptions<EntityContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(x => x.Transactions)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Currency)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CurrencyCode)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Budget>()
                .HasOne(b => b.User)
                .WithMany(x => x.Budgets)
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<Budget>()
                .HasOne(t => t.Currency)
                .WithMany(c => c.Budgets)
                .HasForeignKey(t => t.CurrencyCode)
                .OnDelete(DeleteBehavior.NoAction);

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

            modelBuilder.Entity<SavingsGoal>()
                .HasOne(t => t.Currency)
                .WithMany(c => c.SavingsGoals)
                .HasForeignKey(t => t.CurrencyCode)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Currency>()
                .HasKey(c => c.Code);

            modelBuilder.Entity<ExchangeRate>()
                .HasOne(e => e.FromCurrency)
                .WithMany()
                .HasForeignKey(e => e.FromCurrencyCode)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExchangeRate>()
                .HasOne(e => e.ToCurrency)
                .WithMany()
                .HasForeignKey(e => e.ToCurrencyCode)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Currency>().HasData(
                new Currency { Code = "BGN", Name = "Bulgarian Lev", Symbol = "лв." },
                new Currency { Code = "USD", Name = "US Dollar", Symbol = "$" },
                new Currency { Code = "EUR", Name = "Euro", Symbol = "€" },
                new Currency { Code = "GBP", Name = "British Pound", Symbol = "£" },
                new Currency { Code = "JPY", Name = "Japanese Yen", Symbol = "¥" },
                new Currency { Code = "CAD", Name = "Canadian Dollar", Symbol = "C$" },
                new Currency { Code = "AUD", Name = "Australian Dollar", Symbol = "A$" });

            var directRates = new List<ExchangeRate>
            {
                new ExchangeRate
                {
                    Id = 1, FromCurrencyCode = "BGN", ToCurrencyCode = "USD", Rate = 0.5539m,
                    LastUpdated = DateTime.UtcNow,
                },
                new ExchangeRate
                {
                    Id = 2, FromCurrencyCode = "BGN", ToCurrencyCode = "EUR", Rate = 0.5094m,
                    LastUpdated = DateTime.UtcNow,
                },
                new ExchangeRate
                {
                    Id = 3, FromCurrencyCode = "BGN", ToCurrencyCode = "GBP", Rate = 0.4276m,
                    LastUpdated = DateTime.UtcNow,
                },
                new ExchangeRate
                {
                    Id = 4, FromCurrencyCode = "BGN", ToCurrencyCode = "JPY", Rate = 74.76m,
                    LastUpdated = DateTime.UtcNow,
                },
                new ExchangeRate
                {
                    Id = 5, FromCurrencyCode = "BGN", ToCurrencyCode = "CAD", Rate = 0.7456m,
                    LastUpdated = DateTime.UtcNow,
                },
                new ExchangeRate
                {
                    Id = 6, FromCurrencyCode = "BGN", ToCurrencyCode = "AUD", Rate = 0.8284m,
                    LastUpdated = DateTime.UtcNow,
                },
            };

            var inverseRates = new List<ExchangeRate>();
            int nextId = directRates.Max(r => r.Id) + 1;

            foreach (var rate in directRates)
            {
                inverseRates.Add(
                    new ExchangeRate
                    {
                        Id = nextId++,
                        FromCurrencyCode = rate.ToCurrencyCode,
                        ToCurrencyCode = rate.FromCurrencyCode,
                        Rate = 1 / rate.Rate,
                        LastUpdated = DateTime.UtcNow,
                    });
            }

            modelBuilder.Entity<ExchangeRate>().HasData(directRates);
            modelBuilder.Entity<ExchangeRate>().HasData(inverseRates);

            this.SeedCrossRates(modelBuilder, directRates, inverseRates, nextId);
        }

        private void SeedCrossRates(
            ModelBuilder modelBuilder,
            List<ExchangeRate> directRates,
            List<ExchangeRate> inverseRates,
            int startId)
        {
            var allRates = directRates.Concat(inverseRates).ToList();
            var allCurrencies = directRates.Select(r => r.ToCurrencyCode)
                .Distinct()
                .Where(c => c != "BGN")
                .ToList();

            var crossRates = new List<ExchangeRate>();
            int nextId = startId;

            for (int i = 0; i < allCurrencies.Count; i++)
            {
                for (int j = i + 1; j < allCurrencies.Count; j++)
                {
                    var fromCurrency = allCurrencies[i];
                    var toCurrency = allCurrencies[j];

                    var rateFromBGN = allRates.First(
                        r => r.FromCurrencyCode == "BGN" && r.ToCurrencyCode == fromCurrency);
                    var rateToBGN = allRates.First(r => r.FromCurrencyCode == "BGN" && r.ToCurrencyCode == toCurrency);

                    decimal crossRate = rateToBGN.Rate / rateFromBGN.Rate;
                    crossRates.Add(
                        new ExchangeRate
                        {
                            Id = nextId++,
                            FromCurrencyCode = fromCurrency,
                            ToCurrencyCode = toCurrency,
                            Rate = crossRate,
                            LastUpdated = DateTime.UtcNow,
                        });

                    crossRates.Add(
                        new ExchangeRate
                        {
                            Id = nextId++,
                            FromCurrencyCode = toCurrency,
                            ToCurrencyCode = fromCurrency,
                            Rate = 1 / crossRate,
                            LastUpdated = DateTime.UtcNow,
                        });
                }
            }

            if (crossRates.Any())
            {
                modelBuilder.Entity<ExchangeRate>().HasData(crossRates);
            }
        }
    }
}