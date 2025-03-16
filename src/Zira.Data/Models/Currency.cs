using System.Collections.Generic;

namespace Zira.Data.Models;

public class Currency
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }

    public ICollection<Transaction> Transactions { get; set; }
    public ICollection<Budget> Budgets { get; set; }
    public ICollection<Reminder> Reminders { get; set; }
    public ICollection<SavingsGoal> SavingsGoals { get; set; }
    public ICollection<ApplicationUser> Users { get; set; }
}