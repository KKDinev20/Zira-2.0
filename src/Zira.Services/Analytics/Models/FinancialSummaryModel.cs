using System.Collections.Generic;

namespace Zira.Services.Analytics.Models;

public class FinancialSummaryModel
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetWorth { get; set; }
    public List<SavingsGoalProgressModel> SavingsGoals { get; set; }
    public List<NetWorthTrendModel> NetWorthTrend { get; set; }
    public List<BudgetComparisonModel> BudgetComparison { get; set; } = new();
}