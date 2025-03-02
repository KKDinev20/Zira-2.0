using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zira.Common;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Data.Models;
using Zira.Presentation.Extensions;
using Zira.Presentation.Models;
using Zira.Services.Budget.Contracts;
using Zira.Services.Identity.Constants;
using Zira.Services.Identity.Extensions;

namespace Zira.Presentation.Controllers
{
    [Authorize(Policies.UserPolicy)]
    public class BudgetController : Controller
    {
        private readonly IBudgetService budgetService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly EntityContext entityContext;

        public BudgetController(
            IBudgetService budgetService,
            UserManager<ApplicationUser> userManager,
            EntityContext entityContext)
        {
            this.budgetService = budgetService;
            this.userManager = userManager;
            this.entityContext = entityContext;
        }

        [HttpGet("/set-budget/")]
        public async Task<IActionResult> SetBudget()
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.entityContext);
            this.ViewBag.Categories = Enum.GetValues(typeof(Categories));
            return this.View(new Budget());
        }

        [HttpPost("/set-budget/")]
        public async Task<IActionResult> SetBudget(Budget budgetModel)
        {
            var userId = this.User.GetUserId();
            budgetModel.UserId = userId;

            if (!await this.budgetService.AddBudgetAsync(budgetModel))
            {
                this.TempData["ErrorMessage"] = @BudgetText.BudgetExists;
                this.ViewBag.Categories = Enum.GetValues(typeof(Categories));
                return this.View(budgetModel);
            }

            this.TempData["SuccessMessage"] = @BudgetText.BudgetSuccess;
            return this.RedirectToAction("ViewBudgets");
        }

        [HttpGet("/view-budgets/")]
        public async Task<IActionResult> ViewBudgets(int page = 1, int pageSize = 5)
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.entityContext);

            var userId = this.User.GetUserId();
            var budgets = await this.budgetService.GetUserBudgetsAsync(userId, page, pageSize);
            var totalBudgets = await this.budgetService.GetTotalBudgetsAsync(userId);
            var totalPages = (int)Math.Ceiling((double)totalBudgets / pageSize);

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
            await this.SetGlobalUserInfoAsync(this.userManager, this.entityContext);

            var budget = await this.budgetService.GetBudgetByIdAsync(id, this.User.GetUserId());
            if (budget == null)
            {
                return this.NotFound();
            }

            this.ViewBag.Categories = Enum.GetValues(typeof(Categories));
            return this.View(budget);
        }

        [HttpPost("/edit-budget/{id}")]
        public async Task<IActionResult> EditBudget(Guid id, Budget budgetModel)
        {
            if (id != budgetModel.Id)
            {
                return this.BadRequest();
            }

            budgetModel.UserId = this.User.GetUserId();
            if (!await this.budgetService.UpdateBudgetAsync(budgetModel))
            {
                this.TempData["ErrorMessage"] = @BudgetText.BudgetUpdateFail;
                this.ViewBag.Categories = Enum.GetValues(typeof(Categories));
                return this.View(budgetModel);
            }

            this.TempData["SuccessMessage"] = @BudgetText.BudgetUpdateSuccess;
            return this.RedirectToAction("ViewBudgets");
        }

        [HttpGet("/delete-budget/{id}")]
        public async Task<IActionResult> DeleteBudget(Guid id)
        {
            var budget = await this.budgetService.GetBudgetByIdAsync(id, this.User.GetUserId());
            if (budget == null)
            {
                this.TempData["ErrorMessage"] = @BudgetText.BudgetNotFound;
                return this.RedirectToAction("ViewBudgets");
            }

            return this.View(budget);
        }

        [HttpPost("/delete-budget/{id}")]
        public async Task<IActionResult> DeleteBudgetAsync(Guid id)
        {
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