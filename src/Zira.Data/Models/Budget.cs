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
    public string? CurrencyCode { get; set; }
    public Currency? Currency { get; set; }
    public decimal WarningThreshold { get; set; }

    public decimal? SpentPercentage { get; set; } = 0;
    public DateTime Month { get; set; }

    public string? Remark { get; set; }

    public ApplicationUser? User { get; set; }
}