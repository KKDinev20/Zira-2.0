using Zira.Services.Analytics.Models;

namespace Zira.Presentation.Models;

public class AnalyticsViewModel
{
    public ExpenseAnalyticsViewModel ExpenseAnalytics { get; set; }
    public FinancialSummaryModel FinancialSummary { get; set; }
}