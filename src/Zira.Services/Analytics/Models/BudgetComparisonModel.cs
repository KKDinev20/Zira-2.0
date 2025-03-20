using Zira.Data.Enums;

namespace Zira.Services.Analytics.Models;

public class BudgetComparisonModel
{
    public Categories? Category { get; set; }
    public decimal BudgetedAmount { get; set; }
    public decimal ActualAmount { get; set; }
}