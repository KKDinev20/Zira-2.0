using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
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


    public DashboardController(EntityContext context, UserManager<ApplicationUser> userManager,
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

        var viewModel = new DashboardViewModel
        {
            MonthlyIncome = income,
            MonthlyExpenses = expenses,
        };

        return this.View(viewModel);
    }
}