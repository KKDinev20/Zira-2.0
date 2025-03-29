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

    [HttpGet("/export-financial-overview")]
    public async Task<IActionResult> ExportFinancialOverview()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.Unauthorized();
        }

        var transactions = await this.transactionService.GetUserTransactionsAsync(user.Id);
        var income = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var expenses = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

        var savingsGoals = await this.savingsGoalService.GetSavingsGoalsAsync(user.Id, 1, 5);
        var totalSavings = savingsGoals.Sum(sg => sg.CurrentAmount);
        var netWorth = income - expenses + totalSavings;

        var currencySymbol = user.PreferredCurrency.Symbol;

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

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        PdfDocument document = new PdfDocument();
        document.Info.Title = "Финансов отчет";
        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);

        XFont titleFont = new XFont("Roboto", 16, XFontStyleEx.Bold);
        XFont headingFont = new XFont("Roboto", 12, XFontStyleEx.Bold);
        XFont textFont = new XFont("Roboto", 10, XFontStyleEx.Regular);
        XPen tableLine = new XPen(XColors.Black, 0.5);

        int yPosition = 40;
        int leftMargin = 40;
        int pageWidth = (int)page.Width;

        gfx.DrawString(
            "Финансов отчет",
            titleFont,
            XBrushes.Black,
            new XRect(leftMargin, yPosition, pageWidth - (2 * leftMargin), 30),
            XStringFormats.Center);
        yPosition += 40;

        this.DrawFinancialSummary(
            gfx,
            headingFont,
            currencySymbol,
            income,
            expenses,
            netWorth,
            ref yPosition,
            leftMargin);
        this.DrawSavingsGoalsSection(
            gfx,
            headingFont,
            textFont,
            savingsGoals,
            currencySymbol,
            ref yPosition,
            leftMargin);
        this.DrawMonthlyComparison(
            gfx,
            headingFont,
            textFont,
            monthlyComparison,
            currencySymbol,
            tableLine,
            ref yPosition,
            leftMargin);

        return this.GeneratePdfResponse(document);
    }

    private void DrawFinancialSummary(
        XGraphics gfx,
        XFont headingFont,
        string currencySymbol,
        decimal income,
        decimal expenses,
        decimal netWorth,
        ref int yPosition,
        int leftMargin)
    {
        gfx.DrawString(
            $"Общи приходи: {income:N2} {currencySymbol}",
            headingFont,
            XBrushes.Black,
            new XPoint(leftMargin, yPosition));
        yPosition += 20;
        gfx.DrawString(
            $"Общи разходи: {expenses:N2} {currencySymbol}",
            headingFont,
            XBrushes.Black,
            new XPoint(leftMargin, yPosition));
        yPosition += 20;
        gfx.DrawString(
            $"Нетна стойност: {netWorth:N2} {currencySymbol}",
            headingFont,
            XBrushes.Black,
            new XPoint(leftMargin, yPosition));
        yPosition += 30;
    }

    private void DrawSavingsGoalsSection(
        XGraphics gfx,
        XFont headingFont,
        XFont textFont,
        IEnumerable<SavingsGoal> savingsGoals,
        string currencySymbol,
        ref int yPosition,
        int leftMargin)
    {
        gfx.DrawString("Спестовни цели:", headingFont, XBrushes.Black, new XPoint(leftMargin, yPosition));
        yPosition += 20;

        foreach (var goal in savingsGoals)
        {
            gfx.DrawString(
                $"{goal.Name}: {goal.CurrentAmount:N2} {currencySymbol} / {goal.TargetAmount:N2} {currencySymbol}",
                textFont,
                XBrushes.Black,
                new XPoint(leftMargin + 10, yPosition));
            yPosition += 15;
        }

        yPosition += 30;
    }

    private void DrawMonthlyComparison(
        XGraphics gfx,
        XFont headingFont,
        XFont textFont,
        List<MonthlyComparisonModel> monthlyComparison,
        string currencySymbol,
        XPen tableLine,
        ref int yPosition,
        int leftMargin)
    {
        const int col1 = 40;
        const int col2 = 190;
        const int col3 = 340;
        const int rowHeight = 20;
        const int tableWidth = 400;

        gfx.DrawString("Месечни финанси", headingFont, XBrushes.Black, new XPoint(leftMargin, yPosition));
        yPosition += 20;

        gfx.DrawRectangle(XBrushes.LightSlateGray, col1, yPosition, tableWidth, rowHeight);
        gfx.DrawString("Месец", headingFont, XBrushes.Black, new XPoint(col1 + 10, yPosition + 15));
        gfx.DrawString(
            $"Приход ({currencySymbol})",
            headingFont,
            XBrushes.Black,
            new XPoint(col2 + 10, yPosition + 15));
        gfx.DrawString(
            $"Разход ({currencySymbol})",
            headingFont,
            XBrushes.Black,
            new XPoint(col3 + 10, yPosition + 15));
        yPosition += rowHeight;

        foreach (var entry in monthlyComparison)
        {
            gfx.DrawRectangle(XBrushes.White, col1, yPosition, tableWidth, rowHeight);
            gfx.DrawString(entry.Month, textFont, XBrushes.Black, new XPoint(col1 + 10, yPosition + 15));
            gfx.DrawString($"{entry.Income:N2}", textFont, XBrushes.Black, new XPoint(col2 + 10, yPosition + 15));
            gfx.DrawString($"{entry.Expenses:N2}", textFont, XBrushes.Black, new XPoint(col3 + 10, yPosition + 15));

            gfx.DrawLine(tableLine, col1, yPosition, col1 + tableWidth, yPosition);
            yPosition += rowHeight;
        }
    }

    private IActionResult GeneratePdfResponse(PdfDocument document)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            document.Save(stream, false);
            return this.File(stream.ToArray(), "application/pdf", "Financial_Overview.pdf");
        }
    }
}