using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zira.Data;
using Zira.Data.Models;
using Zira.Presentation.Extensions;
using Zira.Presentation.Models;
using Zira.Services.Analytics.Contracts;
using Zira.Services.Identity.Constants;

namespace Zira.Presentation.Controllers;

[Authorize(Policies.UserPolicy)]
public class AnalyticsController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IAnalyticsService expenseAnalyticsService;
    private readonly EntityContext context;

    public AnalyticsController(UserManager<ApplicationUser> userManager, IAnalyticsService expenseAnalyticsService, EntityContext context)
    {
        this.userManager = userManager;
        this.expenseAnalyticsService = expenseAnalyticsService;
        this.context = context;
    }

    [HttpGet("/financial-summary")]
    public async Task<IActionResult> FinancialSummary()
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.context);

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