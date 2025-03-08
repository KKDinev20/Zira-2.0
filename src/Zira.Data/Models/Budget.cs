using System;
using Zira.Data.Enums;

namespace Zira.Data.Models;

public class Budget
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string BudgetId { get; set; } = string.Empty;
    public Categories? Category { get; set; }
    public decimal Amount { get; set; }
    public decimal WarningThreshold { get; set; }
    public DateTime Month { get; set; }

    public string? Remark { get; set; }

    public ApplicationUser? User { get; set; }
}