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
    public async Task<IActionResult> Index()
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.context);

        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.RedirectToAction("Login", "Authentication");
        }

        var income = await this.transactionService.GetCurrentMonthIncomeAsync(user.Id);
        var expenses = await this.transactionService.GetCurrentMonthExpensesAsync(user.Id);
        var food = await this.transactionService.GetCurrentMonthFoodExpense(user.Id);
        var utilities = await this.transactionService.GetCurrentMonthUtilitiesExpense(user.Id);
        var recentTransactions = await this.transactionService.GetLastSixRecentTransactions(user.Id);
        var (monthlyIncomes, monthlyExpenses) =
            await this.transactionService.GetMonthlyIncomeAndExpensesAsync(user.Id, DateTime.UtcNow.Year);
        var totalIncome = await this.context.Transactions
            .Where(t => t.UserId == user.Id && t.Type == TransactionType.Income)
            .SumAsync(t => t.Amount);

        var totalExpenses = await this.context.Transactions
            .Where(t => t.UserId == user.Id && t.Type == TransactionType.Expense)
            .SumAsync(t => t.Amount);

        this.ViewBag.TotalIncome = totalIncome;
        this.ViewBag.TotalExpenses = totalExpenses;

        this.ViewBag.MonthlyIncomes = monthlyIncomes;
        this.ViewBag.MonthlyExpenses = monthlyExpenses;
        this.ViewBag.RecentTransactions = recentTransactions;

        var viewModel = new DashboardViewModel
        {
            MonthlyIncome = income,
            MonthlyExpenses = expenses,
            MonthlyFood = food,
            MonthlyUtilities = utilities,
        };

        return this.View(viewModel);
    }
}