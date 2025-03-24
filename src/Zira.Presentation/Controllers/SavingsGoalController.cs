using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zira.Common;
using Zira.Data;
using Zira.Data.Models;
using Zira.Presentation.Extensions;
using Zira.Presentation.Models;
using Zira.Services.Identity.Constants;
using Zira.Services.Identity.Extensions;
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

            var goals = await this.savingsGoalService.GetUserSavingsGoalsAsync(
                user.Id,
                page,
                pageSize,
                null);
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
            var availableCurrencies = await this.context.Currencies
                .Select(c => c.Code)
                .ToListAsync();
            var userId = this.User.GetUserId();
            var user = await this.userManager.FindByIdAsync(userId.ToString());
            this.ViewBag.Currencies = availableCurrencies;
            this.ViewBag.DefaultCurrency = user?.PreferredCurrencyCode ?? "BGN";
            return this.View(new SavingsGoalViewModel());
        }

        [HttpPost("/create-savings-goal/")]
        public async Task<IActionResult> CreateSavingsGoal(SavingsGoalViewModel model)
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);

            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.RedirectToAction("Login", "Authentication");
            }

            if (!this.ModelState.IsValid)
            {
                var availableCurrencies = await this.context.Currencies
                    .Select(c => c.Code)
                    .ToListAsync();
                this.ViewBag.Currencies = availableCurrencies;
                this.ViewBag.DefaultCurrency = user?.PreferredCurrencyCode ?? "BGN";
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
                Currency = model.Currency,
                CurrencyCode = model.CurrencyCode,
            };

            await this.savingsGoalService.AddSavingsGoalsAsync(goal, goal.CurrencyCode);
            this.TempData["SuccessMessage"] = @SavingsGoalText.SavingsGoalSuccess;
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
                this.TempData["ErrorMessage"] = @SavingsGoalText.SavingsGoalNotFound;
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
                Currency = goal.Currency,
                CurrencyCode = goal.CurrencyCode,
            };

            var availableCurrencies = await this.context.Currencies
                .Select(c => c.Code)
                .ToListAsync();
            this.ViewBag.Currencies = availableCurrencies;
            this.ViewBag.DefaultCurrency = goal.CurrencyCode ?? user.PreferredCurrencyCode ?? "BGN";

            return this.View(model);
        }

        [HttpPost("/edit-savings-goal/{id}")]
        public async Task<IActionResult> EditSavingsGoal(Guid id, SavingsGoalViewModel model)
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);
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
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                var availableCurrencies = await this.context.Currencies
                    .Select(c => c.Code)
                    .ToListAsync();
                this.ViewBag.Currencies = availableCurrencies;
                this.ViewBag.DefaultCurrency = model.CurrencyCode ?? user.PreferredCurrencyCode ?? "BGN";

                return this.View(model);
            }


            model.CurrencyCode = model.CurrencyCode ?? user.PreferredCurrencyCode ?? "BGN";

            var existingGoal = await this.savingsGoalService.GetSavingsGoalByIdAsync(user.Id, id);
            if (existingGoal == null)
            {
                this.TempData["ErrorMessage"] = @SavingsGoalText.SavingsGoalNotFound;
                return this.RedirectToAction("ViewSavingsGoals");
            }

            existingGoal.Name = model.Name;
            existingGoal.TargetAmount = model.TargetAmount;
            if (model.CurrentAmount != 0)
            {
                existingGoal.CurrentAmount = model.CurrentAmount;
            }

            existingGoal.TargetDate = model.TargetDate;
            existingGoal.Remark = model.Remark;
            existingGoal.CurrencyCode = model.CurrencyCode;

            await this.savingsGoalService.UpdateSavingsGoalsAsync(existingGoal, existingGoal.CurrencyCode);
            this.TempData["SuccessMessage"] = @SavingsGoalText.UpdateSuccess;
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
                this.TempData["ErrorMessage"] = @SavingsGoalText.SavingsGoalNotFound;
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
                this.TempData["ErrorMessage"] = @SavingsGoalText.SavingsGoalNotFound;
                return this.RedirectToAction("ViewSavingsGoals");
            }

            await this.savingsGoalService.DeleteSavingsGoalsAsync(goal);
            this.TempData["SuccessMessage"] = @SavingsGoalText.DeleteSuccess;
            return this.RedirectToAction("ViewSavingsGoals");
        }
    }
}