using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Zira.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string? ImageUrl { get; set; }
        public string? PreferredCurrency { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Budget>? Budgets { get; set; }
        public ICollection<Reminder>? Reminders { get; set; }
        public ICollection<SavingsGoal>? SavingsGoals { get; set; }
    }
}