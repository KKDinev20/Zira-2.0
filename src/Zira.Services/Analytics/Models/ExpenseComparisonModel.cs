using System.Collections.Generic;

namespace Zira.Services.Analytics.Models;

public class ExpenseComparisonModel
{
    public List<MonthlyComparisonModel> MonthlyComparison { get; set; }
    public List<CategoryComparisonModel> CategoryComparison { get; set; }
    public List<MonthlySavingsRateModel> MonthlySavingsRate { get; set; }

    public string PreferredCurrencySymbol { get; set; } = "лв.";
}