using System.Collections.Generic;
using Zira.Data.Enums;
using Zira.Services.Transaction.Models;

namespace Zira.Presentation.Models;

public class ExpenseAnalyticsViewModel
{
    public List<CategoryExpenseSummary> TopExpenses { get; set; }
    public Dictionary<Categories, string> CostSavingTips { get; set; }
}