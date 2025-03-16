using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zira.Data;
using Zira.Data.Models;
using Zira.Presentation.Extensions;
using Zira.Presentation.Models;
using Zira.Services.Identity.Constants;
using Zira.Services.SavingsGoal.Contracts;

namespace Zira.Presentation.Controllers
{
    [Authorize(Policies.UserPolicy)]
    public class SavingsGoalController : Controller
    {
        private readonly ISavingsGoalService savingsGoalService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly EntityContext context;

        public SavingsGoalController(
            ISavingsGoalService savingsGoalService,
            UserManager<ApplicationUser> userManager,
            EntityContext context)
        {
            this.savingsGoalService = savingsGoalService;
            this.userManager = userManager;
            this.context = context;
        }

        [HttpGet("/savings-goals/")]
        public async Task<IActionResult> ViewSavingsGoals(int page = 1, int pageSize = 5)
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);

            var user = await this.userManager.GetUserAsync(this.User);

            var goals = await this.savingsGoalService.GetUserSavingsGoalsAsync(user.Id, page, pageSize, user.PreferredCurrencyCode);
            var totalGoals = await this.savingsGoalService.GetTotalSavingsGoalsAsync(user.Id);
            var totalPages = (int)Math.Ceiling((double)totalGoals / pageSize);

            var viewModel = new SavingsGoalListViewModel
            {
                Goals = goals,
                CurrentPage = page,
                TotalPages = totalPages,
            };

            return this.View(viewModel);
        }

        [HttpGet("/create-savings-goal/")]
        public async Task<IActionResult> CreateSavingsGoal()
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);

            return this.View(new SavingsGoalViewModel());
        }

        [HttpPost("/create-savings-goal/")]
        public async Task<IActionResult> CreateSavingsGoal(SavingsGoalViewModel model)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.RedirectToAction("Login", "Authentication");
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var goal = new SavingsGoal
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = model.Name,
                TargetAmount = model.TargetAmount,
                CurrentAmount = 0,
                TargetDate = model.TargetDate,
                Remark = model.Remark,
                Currency = user.PreferredCurrency,
                CurrencyCode = user.PreferredCurrencyCode ?? "BGN",
            };

            await this.savingsGoalService.AddSavingsGoalsAsync(goal, goal.CurrencyCode);
            this.TempData["SuccessMessage"] = "Savings goal created successfully!";
            return this.RedirectToAction("ViewSavingsGoals");
        }

        [HttpGet("/edit-savings-goal/{id}")]
        public async Task<IActionResult> EditSavingsGoal(Guid id)
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.RedirectToAction("Login", "Authentication");
            }

            var goal = await this.savingsGoalService.GetSavingsGoalByIdAsync(user.Id, id);
            if (goal == null)
            {
                this.TempData["ErrorMessage"] = "Savings goal not found.";
                return this.RedirectToAction("ViewSavingsGoals");
            }

            var model = new SavingsGoalViewModel
            {
                Id = goal.Id,
                Name = goal.Name,
                TargetAmount = goal.TargetAmount,
                CurrentAmount = goal.CurrentAmount,
                TargetDate = goal.TargetDate,
                Remark = goal.Remark,
            };

            return this.View(model);
        }

        [HttpPost("/edit-savings-goal/{id}")]
        public async Task<IActionResult> EditSavingsGoal(Guid id, SavingsGoalViewModel model)
        {
            if (id != model.Id)
            {
                return this.BadRequest();
            }

            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.RedirectToAction("Login", "Authentication");
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var goal = new SavingsGoal
            {
                Id = model.Id,
                UserId = user.Id,
                Name = model.Name,
                TargetAmount = model.TargetAmount,
                CurrentAmount = model.CurrentAmount,
                TargetDate = model.TargetDate,
                Currency = user.PreferredCurrency,
                CurrencyCode = user.PreferredCurrencyCode ?? "BGN",
            };

            await this.savingsGoalService.UpdateSavingsGoalsAsync(goal, goal.CurrencyCode);
            this.TempData["SuccessMessage"] = "Savings goal updated successfully!";
            return this.RedirectToAction("ViewSavingsGoals");
        }

        [HttpGet("/delete-savings-goal/{id}")]
        public async Task<IActionResult> DeleteSavingsGoal(Guid id)
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.RedirectToAction("Login", "Authentication");
            }

            var goal = await this.savingsGoalService.GetSavingsGoalByIdAsync(user.Id, id);
            if (goal == null)
            {
                this.TempData["ErrorMessage"] = "Savings goal not found.";
                return this.RedirectToAction("ViewSavingsGoals");
            }

            return this.View(goal);
        }

        [HttpPost("/delete-savings-goal/{id}")]
        public async Task<IActionResult> DeleteSavingsGoalConfirmed(Guid id)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.RedirectToAction("Login", "Authentication");
            }

            var goal = await this.savingsGoalService.GetSavingsGoalByIdAsync(user.Id, id);
            if (goal == null)
            {
                this.TempData["ErrorMessage"] = "Savings goal not found.";
                return this.RedirectToAction("ViewSavingsGoals");
            }

            await this.savingsGoalService.DeleteSavingsGoalsAsync(goal);
            this.TempData["SuccessMessage"] = "Savings goal deleted successfully!";
            return this.RedirectToAction("ViewSavingsGoals");
        }
    }
}