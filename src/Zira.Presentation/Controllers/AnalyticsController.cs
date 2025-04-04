using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Data.Models;
using Zira.Presentation.Extensions;
using Zira.Presentation.Models;
using Zira.Services.Analytics.Contracts;
using Zira.Services.Analytics.Models;
using Zira.Services.Budget.Contracts;
using Zira.Services.Identity.Constants;
using Zira.Services.SavingsGoal.Contracts;
using Zira.Services.Transaction.Contracts;

namespace Zira.Presentation.Controllers;

[Authorize(Policies.UserPolicy)]
public class AnalyticsController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IAnalyticsService expenseAnalyticsService;
    private readonly ITransactionService transactionService;
    private readonly IBudgetService budgetService;
    private readonly ISavingsGoalService savingsGoalService;
    private readonly EntityContext context;

    public AnalyticsController(
        UserManager<ApplicationUser> userManager,
        IAnalyticsService expenseAnalyticsService,
        ITransactionService transactionService,
        ISavingsGoalService savingsGoalService,
        EntityContext context,
        IBudgetService budgetService)
    {
        this.userManager = userManager;
        this.expenseAnalyticsService = expenseAnalyticsService;
        this.transactionService = transactionService;
        this.savingsGoalService = savingsGoalService;
        this.context = context;
        this.budgetService = budgetService;
    }

    [HttpGet("/financial-overview")]
    public async Task<IActionResult> FinancialOverview()
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.context);

        var user = await this.userManager.GetUserAsync(this.User);

        var topExpenses = await this.expenseAnalyticsService.GetTopExpenseCategoriesAsync(user.Id);
        var savingTips = this.expenseAnalyticsService.GetCostSavingTips(topExpenses);
        var monthlyExpenses =
            await this.expenseAnalyticsService.GetMonthlyExpensesAsync(user.Id, DateTime.UtcNow.Month);

        var expenseAnalytics = new ExpenseAnalyticsViewModel
        {
            TopExpenses = topExpenses,
            CostSavingTips = savingTips,
            MonthlyExpenses = monthlyExpenses,
        };

        var transactions = await this.transactionService.GetUserTransactionsAsync(user.Id);
        var income = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var expenses = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
        var savingsGoals = await this.savingsGoalService.GetSavingsGoalsAsync(user.Id, 1, 5);
        var totalSavings = savingsGoals.Sum(sg => sg.CurrentAmount);
        var netWorth = income - expenses + totalSavings;
        var budgets = await this.budgetService.GetUserBudgetsAsync(user.Id, 1, 100);

        var budgetComparison = budgets.Select(
            budget => new BudgetComparisonModel
            {
                Category = budget.Category,
                BudgetedAmount = budget.Amount,
                ActualAmount = transactions
                    .Where(t => t.Type == TransactionType.Expense && t.Category == budget.Category)
                    .Sum(t => t.Amount),
            }).ToList();

        var savingsProgress = savingsGoals.Select(
            sg => new SavingsGoalProgressModel
            {
                Name = sg.Name,
                TargetAmount = sg.TargetAmount,
                CurrentAmount = sg.CurrentAmount,
                Progress = sg.CurrentAmount / sg.TargetAmount * 100,
            }).ToList();

        var financialSummary = new FinancialSummaryModel
        {
            TotalIncome = income,
            TotalExpenses = expenses,
            NetWorth = netWorth,
            SavingsGoals = savingsProgress,
            BudgetComparison = budgetComparison,
        };

        var viewModel = new AnalyticsViewModel
        {
            ExpenseAnalytics = expenseAnalytics,
            FinancialSummary = financialSummary,
        };

        return this.View(viewModel);
    }

    [HttpGet("/expense-comparison")]
    public async Task<IActionResult> FinancialComparison()
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.context);

        var user = await this.userManager.GetUserAsync(this.User);

        var transactions = await this.transactionService.GetUserTransactionsAsync(user.Id);

        var monthlyComparison = transactions
            .GroupBy(t => new { t.Date.Year, t.Date.Month })
            .Select(
                g => new MonthlyComparisonModel
                {
                    Month = $"{g.Key.Month}/{g.Key.Year}",
                    Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                    Expenses = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
                })
            .OrderByDescending(m => m.Month)
            .ToList();

        var categoryComparison = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .GroupBy(t => t.Category)
            .Select(
                g => new CategoryComparisonModel
                {
                    Category = g.Key.ToString(),
                    TotalAmount = g.Sum(t => t.Amount),
                })
            .ToList();

        var monthlySavingsRate = transactions
            .GroupBy(t => new { t.Date.Year, t.Date.Month })
            .Select(
                g => new MonthlySavingsRateModel
                {
                    Month = $"{g.Key.Month}/{g.Key.Year}",
                    SavingsRate = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount) > 0
                        ? (g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount) -
                           g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount))
                        / g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount) * 100
                        : 0,
                })
            .OrderByDescending(m => m.Month)
            .ToList();

        var viewModel = new ExpenseComparisonModel
        {
            MonthlyComparison = monthlyComparison,
            CategoryComparison = categoryComparison,
            MonthlySavingsRate = monthlySavingsRate,
        };

        return this.View(viewModel);
    }
}