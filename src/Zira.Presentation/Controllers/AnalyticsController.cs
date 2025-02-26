using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zira.Data.Models;
using Zira.Presentation.Models;
using Zira.Services.Analytics.Contracts;
using Zira.Services.Identity.Constants;

namespace Zira.Presentation.Controllers;

[Authorize(Policies.UserPolicy)]
public class AnalyticsController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IAnalyticsService expenseAnalyticsService;

    public AnalyticsController(UserManager<ApplicationUser> userManager, IAnalyticsService expenseAnalyticsService)
    {
        this.userManager = userManager;
        this.expenseAnalyticsService = expenseAnalyticsService;
    }

    [Authorize(Policies.UserPolicy)]
    [HttpGet("/financial-summary")]
    public async Task<IActionResult> FinancialSummary()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.RedirectToAction("Login", "Authentication");
        }

        var topExpenses = await this.expenseAnalyticsService.GetTopExpenseCategoriesAsync(user.Id);

        var savingTips = this.expenseAnalyticsService.GetCostSavingTips(topExpenses);

        var viewModel = new ExpenseAnalyticsViewModel
        {
            TopExpenses = topExpenses,
            CostSavingTips = savingTips,
        };

        return this.View(viewModel);
    }
}