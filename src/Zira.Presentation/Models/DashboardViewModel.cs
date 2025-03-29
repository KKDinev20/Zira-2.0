using System.Collections.Generic;
using Zira.Data.Models;
using Zira.Services.Transaction.Models;

namespace Zira.Presentation.Models;

public class DashboardViewModel
{
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpense { get; set; }
    public decimal MonthlyFood { get; set; }
    public decimal MonthlyUtilities { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal WeeklyTotal { get; set; }
    public decimal CurrentMonthIncome { get; set; }
    public decimal CurrentMonthExpenses { get; set; }
    public List<CategoryExpenseSummary> TopExpenseCategories { get; set; } = new();
    public List<Transaction> RecentTransactions { get; set; } = new();
    public List<decimal> MonthlyIncomes { get; set; } = new();
    public List<decimal> MonthlyExpenses { get; set; } = new();
    public List<decimal> MonthlyTotals { get; set; } = new();
    public List<string> MonthLabels { get; set; } = new();
    public string SelectedType { get; set; }

    public string PreferredCurrencySymbol { get; set; } = "лв.";

    public int ReminderCount { get; set; }
}