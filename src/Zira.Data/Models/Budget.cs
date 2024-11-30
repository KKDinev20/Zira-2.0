using System;
using Zira.Data.Enums;

namespace Zira.Data.Models;

public class Budget
{
    public Guid BudgetId { get; set; }
    public Guid UserId { get; set; }
    public Categories Category { get; set; }
    public decimal Amount { get; set; }
    public DateTime Month { get; set; }

    public ApplicationUser? User { get; set; }
}