using System.Collections.Generic;
using Zira.Data.Enums;
using Zira.Services.Analytics.Models;
using Zira.Services.Transaction.Models;

namespace Zira.Presentation.Models;

public class ExpenseAnalyticsViewModel
{
    public List<CategoryExpenseSummary> TopExpenses { get; set; }
    public Dictionary<Categories, List<string>> CostSavingTips { get; set; }

    public List<MonthlyExpenseSummary> MonthlyExpenses { get; set; }
}