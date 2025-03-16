using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Data.Models;
using Zira.Presentation.Extensions;
using Zira.Presentation.Models;
using Zira.Services.Identity.Constants;
using Zira.Services.Transaction.Contracts;

namespace Zira.Presentation.Controllers;

[Route("/dashboard/")]
public class DashboardController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly EntityContext context;
    private readonly ITransactionService transactionService;

    public DashboardController(
        EntityContext context,
        UserManager<ApplicationUser> userManager,
        ITransactionService transactionService)
    {
        this.context = context;
        this.userManager = userManager;
        this.transactionService = transactionService;
    }

    [HttpGet("")]
    [Authorize(Policies.UserPolicy)]
    public async Task<IActionResult> Index(string type = "Income")
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.context);

        var user = await this.userManager.GetUserAsync(this.User);
        var preferredCurrency = await this.context.Currencies
            .Where(c => c.Code == user.PreferredCurrencyCode)
            .Select(c => new { c.Code, c.Symbol })
            .FirstOrDefaultAsync();
        if (!Enum.TryParse(type, true, out TransactionType selectedType))
        {
            selectedType = TransactionType.Income;
        }

        var (monthlyTotals, monthLabels) =
            await this.transactionService.GetLastSixMonthsDataAsync(user.Id, selectedType);
        var weeklyTotal = await this.transactionService.GetCurrentWeekTotalAsync(user.Id, selectedType);
        var income = await this.transactionService.GetCurrentMonthIncomeAsync(user.Id);
        var expenses = await this.transactionService.GetCurrentMonthExpensesAsync(user.Id);
        var food = await this.transactionService.GetCurrentMonthFoodExpense(user.Id);
        var utilities = await this.transactionService.GetCurrentMonthUtilitiesExpense(user.Id);
        var recentTransactions = await this.transactionService.GetRecentTransactions(user.Id);
        var (monthlyIncomes, monthlyExpenses) =
            await this.transactionService.GetMonthlyIncomeAndExpensesAsync(user.Id, DateTime.UtcNow.Year);
        var totalIncome = await this.context.Transactions
            .Where(t => t.UserId == user.Id && t.Type == TransactionType.Income).SumAsync(t => t.Amount);
        var totalExpenses = await this.context.Transactions
            .Where(t => t.UserId == user.Id && t.Type == TransactionType.Expense).SumAsync(t => t.Amount);
        var topCategories = await this.transactionService.GetTopExpenseCategoriesAsync(user.Id, 5);

        int reminderCount = await this.context.Reminders.CountAsync(r => r.IsNotified == false);
        
        var viewModel = new DashboardViewModel
        {
            MonthlyIncome = income,
            MonthlyExpense = expenses,
            MonthlyFood = food,
            MonthlyUtilities = utilities,
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            WeeklyTotal = weeklyTotal,
            CurrentMonthIncome = selectedType == TransactionType.Income ? income : 0,
            CurrentMonthExpenses = selectedType == TransactionType.Expense ? expenses : 0,
            TopExpenseCategories = topCategories,
            RecentTransactions = recentTransactions,
            MonthlyIncomes = monthlyIncomes,
            MonthlyExpenses = monthlyExpenses,
            MonthlyTotals = monthlyTotals,
            MonthLabels = monthLabels,
            SelectedType = selectedType.ToString(),
            PreferredCurrencySymbol = preferredCurrency?.Symbol ?? "лв.",
            ReminderCount = reminderCount,
        };

        return this.View(viewModel);
    }
}