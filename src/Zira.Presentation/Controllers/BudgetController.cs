using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zira.Common;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Data.Models;
using Zira.Presentation.Extensions;
using Zira.Presentation.Models;
using Zira.Services.Identity.Constants;
using Zira.Services.Identity.Extensions;

namespace Zira.Presentation.Controllers
{
    [Authorize(Policies.UserPolicy)]
    public class BudgetController : Controller
    {
        private readonly EntityContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public BudgetController(EntityContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpGet("/set-budget/")]
        public async Task<IActionResult> SetBudget()
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);

            var userId = this.User.GetUserId();

            var existingBudgets = await this.context.Budgets
                .Where(
                    b => b.UserId == userId && b.Month.Year == DateTime.UtcNow.Year &&
                         b.Month.Month == DateTime.UtcNow.Month)
                .ToListAsync();

            this.ViewBag.Categories = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
            return this.View(new Budget());
        }

        [HttpPost("/set-budget/")]
        public async Task<IActionResult> SetBudget(Budget model)
        {
            var userId = this.User.GetUserId();

            var user = await this.context.Users.FirstOrDefaultAsync(u => u.ApplicationUserId == userId);
            if (user == null)
            {
                this.ModelState.AddModelError("", @AuthenticationText.UserNotExisting);
                this.ViewBag.Categories = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
                return this.View(model);
            }

            model.UserId = user.Id;

            model.Month = model.Month == default ? DateTime.UtcNow : model.Month;

            try
            {
                this.context.Budgets.Add(model);
                await this.context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this.TempData["ErrorMessage"] = @BudgetText.BudgetError;
                return this.View(model);
            }

            return this.RedirectToAction("ViewBudgets");
        }

        [HttpGet("/view-budgets/")]
        public async Task<IActionResult> ViewBudgets(int page = 1, int pageSize = 5)
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);

            var userId = this.User.GetUserId();
            var user = await this.context.Users
                .FirstOrDefaultAsync(u => u.ApplicationUserId == userId);

            if (user == null)
            {
                return this.NotFound(@AuthenticationText.UserNotExisting);
            }

            var totalBudgets = await this.context.Budgets
                .Where(i => i.UserId == user.Id)
                .CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalBudgets / pageSize);
            var budgets = await this.context.Budgets
                .Where(i => i.UserId == user.Id)
                .OrderBy(i => i.Month)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

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
            var budget = await this.context.Budgets
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.BudgetId == id && i.User.ApplicationUserId == this.User.GetUserId());

            if (budget == null)
            {
                return this.NotFound();
            }

            this.ViewBag.Categories = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();

            return this.View(budget);
        }

        [HttpPost("/edit-budget/{id}")]
        public async Task<IActionResult> EditBudget(Guid id, Budget budgetModel)
        {
            if (id != budgetModel.BudgetId)
            {
                return this.BadRequest();
            }

            if (!this.ModelState.IsValid)
            {
                this.TempData["ErrorMessage"] = @BudgetText.BudgetError;
                this.ViewBag.Categories = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
                return this.RedirectToAction("ViewBudgets");
            }

            var budget = await this.context.Budgets
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.BudgetId == id && i.User.ApplicationUserId == this.User.GetUserId());

            if (budget == null)
            {
                this.TempData["ErrorMessage"] = @BudgetText.BudgetNotFound;
                return this.NotFound();
            }

            budget.Category = budgetModel.Category;
            budget.Amount = budgetModel.Amount;
            budget.Month = budgetModel.Month;

            await this.context.SaveChangesAsync();
            this.TempData["SuccessMessage"] = @BudgetText.BudgetSuccess;
            return this.RedirectToAction("ViewBudgets");
        }

        [HttpGet("/delete-budget/{id}")]
        public async Task<IActionResult> DeleteBudget(Guid id)
        {
            var budget = await this.context.Budgets
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.BudgetId == id && i.User.ApplicationUserId == this.User.GetUserId());

            if (budget == null)
            {
                return this.NotFound();
            }

            return this.View(budget);
        }

        [HttpPost("/delete-budget/{id}")]
        public async Task<IActionResult> DeleteBudgetAsync(Guid id)
        {
            var budget = await this.context.Budgets
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.BudgetId == id && i.User.ApplicationUserId == this.User.GetUserId());

            if (budget == null)
            {
                return this.NotFound();
            }

            this.context.Budgets.Remove(budget);
            await this.context.SaveChangesAsync();

            return this.RedirectToAction("ViewBudgets");
        }
    }
}