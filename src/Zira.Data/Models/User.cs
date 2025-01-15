using System;
using System.Collections.Generic;

namespace Zira.Data.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string? ImageUrl { get; set; }

        public Guid ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        public ICollection<Income>? Incomes { get; set; }
        public ICollection<Expense>? Expenses { get; set; }
        public ICollection<Budget>? Budgets { get; set; }
        public ICollection<Reminder>? Reminders { get; set; }
    }
}