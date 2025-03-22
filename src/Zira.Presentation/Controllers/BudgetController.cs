using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Zira.Common;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Data.Models;
using Zira.Presentation.Extensions;
using Zira.Presentation.Models;
using Zira.Services.Budget.Contracts;
using Zira.Services.Identity.Constants;
using Zira.Services.Identity.Extensions;
using Zira.Services.Reminder.Internals;

namespace Zira.Presentation.Controllers
{
    [Authorize(Policies.UserPolicy)]
    public class BudgetController : Controller
    {
        private readonly IBudgetService budgetService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly EntityContext context;
        private readonly IHubContext<NotificationHub> hubContext;

        public BudgetController(
            IBudgetService budgetService,
            UserManager<ApplicationUser> userManager,
            EntityContext context,
            IHubContext<NotificationHub> hubContext)
        {
            this.budgetService = budgetService;
            this.userManager = userManager;
            this.context = context;
            this.hubContext = hubContext;
        }

        [HttpGet("/set-budget/")]
        public async Task<IActionResult> SetBudget()
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);
            this.ViewBag.Categories = Enum.GetValues(typeof(Categories));
            var availableCurrencies = await this.context.Currencies
                .Select(c => c.Code)
                .ToListAsync();
            var userId = this.User.GetUserId();
            var user = await this.userManager.FindByIdAsync(userId.ToString());
            this.ViewBag.Currencies = availableCurrencies;
            this.ViewBag.DefaultCurrency = user?.PreferredCurrencyCode ?? "BGN";

            return this.View(new BudgetViewModel());
        }

        [HttpPost("/set-budget/")]
        public async Task<IActionResult> SetBudget(BudgetViewModel viewModel)
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);
    
            var availableCurrencies = await this.context.Currencies
                .Select(c => c.Code)
                .ToListAsync();
            var userId = this.User.GetUserId();
            var user = await this.userManager.FindByIdAsync(userId.ToString());
            this.ViewBag.Currencies = availableCurrencies;
            this.ViewBag.DefaultCurrency = user?.PreferredCurrencyCode ?? "BGN";
            this.ViewBag.Categories = Enum.GetValues(typeof(Categories));
    
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            var budgetModel = new Budget
            {
                UserId = userId,
                Amount = viewModel.Amount,
                WarningThreshold = viewModel.WarningThreshold,
                Category = viewModel.Category,
                Month = new DateTime(viewModel.Month.Year, viewModel.Month.Month, 1),
                Remark = viewModel.Remark,
                CurrencyCode = viewModel.CurrencyCode ?? user?.PreferredCurrencyCode ?? "BGN"
            };

            if (!await this.budgetService.AddBudgetAsync(budgetModel, userId))
            {
                this.TempData["ErrorMessage"] = @BudgetText.BudgetExists;
                return this.View(viewModel);
            }

            this.TempData["SuccessMessage"] = @BudgetText.BudgetSuccess;
            return this.RedirectToAction("ViewBudgets");
        }


        [HttpGet("/view-budgets/")]
        public async Task<IActionResult> ViewBudgets(int page = 1, int pageSize = 5)
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);
            var userId = this.User.GetUserId();
            var budgets = await this.budgetService.GetUserBudgetsAsync(userId, page, pageSize);
            var totalBudgets = await this.budgetService.GetTotalBudgetsAsync(userId);
            var totalPages = (int)Math.Ceiling((double)totalBudgets / pageSize);

            var warnings = await this.budgetService.GetBudgetWarningsAsync(userId);
            this.TempData["BudgetWarnings"] = warnings;

            var model = new BudgetListViewModel
            {
                Budgets = budgets,
                CurrentPage = page,
                TotalPages = totalPages,
            };

            return this.View(model);
        }

        [HttpGet("/edit-budget/{id}")]
        public async Task<IActionResult> EditBudget(Guid id)
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);
            var budget = await this.budgetService.GetBudgetByIdAsync(id, this.User.GetUserId());
            if (budget == null)
            {
                return this.NotFound();
            }

            var viewModel = new BudgetViewModel
            {
                Id = budget.Id,
                Amount = budget.Amount,
                WarningThreshold = budget.WarningThreshold,
                Category = budget.Category,
                Month = budget.Month,
                Remark = budget.Remark,
            };

            this.ViewBag.Categories = Enum.GetValues(typeof(Categories));
            return this.View(viewModel);
        }

        [HttpPost("/edit-budget/{id}")]
        public async Task<IActionResult> EditBudget(Guid id, BudgetViewModel viewModel)
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);
            if (id != viewModel.Id || !this.ModelState.IsValid)
            {
                this.ViewBag.Categories = Enum.GetValues(typeof(Categories));
                return this.View(viewModel);
            }

            var budgetModel = new Budget
            {
                Id = viewModel.Id,
                UserId = this.User.GetUserId(),
                Amount = viewModel.Amount,
                Category = viewModel.Category,
                WarningThreshold = viewModel.WarningThreshold,
                Month = new DateTime(viewModel.Month.Year, viewModel.Month.Month, 1),
                Remark = viewModel.Remark,
            };

            if (!await this.budgetService.UpdateBudgetAsync(budgetModel))
            {
                this.TempData["ErrorMessage"] = @BudgetText.BudgetUpdateFail;
                this.ViewBag.Categories = Enum.GetValues(typeof(Categories));
                return this.View(viewModel);
            }

            this.TempData["SuccessMessage"] = @BudgetText.BudgetUpdateSuccess;
            return this.RedirectToAction("ViewBudgets");
        }

        [HttpGet("/delete-budget/{id}")]
        public async Task<IActionResult> DeleteBudget(Guid id)
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);
            var budget = await this.budgetService.GetBudgetByIdAsync(id, this.User.GetUserId());
            if (budget == null)
            {
                this.TempData["ErrorMessage"] = @BudgetText.BudgetNotFound;
                return this.RedirectToAction("ViewBudgets");
            }

            return this.View(
                new BudgetViewModel
                {
                    Id = budget.Id,
                    Amount = budget.Amount,
                    WarningThreshold = budget.WarningThreshold,
                    Category = budget.Category,
                    Month = budget.Month,
                });
        }

        [HttpPost("/delete-budget/{id}")]
        public async Task<IActionResult> DeleteBudgetAsync(Guid id)
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);
            if (!await this.budgetService.DeleteBudgetAsync(id, this.User.GetUserId()))
            {
                this.TempData["ErrorMessage"] = @BudgetText.BudgetNotFound;
                return this.RedirectToAction("ViewBudgets");
            }

            this.TempData["SuccessMessage"] = @BudgetText.BudgetDeleteSuccess;
            return this.RedirectToAction("ViewBudgets");
        }
    }
}