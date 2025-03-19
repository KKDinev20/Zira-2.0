using System;
using System.Collections.Generic;
using System.Linq;
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
using Zira.Presentation.Validations;
using Zira.Services.Identity.Constants;
using Zira.Services.Identity.Extensions;
using Zira.Services.SavingsGoal.Contracts;
using Zira.Services.Transaction.Contracts;

namespace Zira.Presentation.Controllers;

[Authorize(Policies.UserPolicy)]
public class TransactionsController : Controller
{
    private readonly ITransactionService transactionService;
    private readonly ISavingsGoalService savingsGoalService;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly EntityContext entityContext;

    public TransactionsController(
        ITransactionService transactionService,
        UserManager<ApplicationUser> userManager,
        EntityContext entityContext,
        ISavingsGoalService savingsGoalService)
    {
        this.transactionService = transactionService;
        this.userManager = userManager;
        this.entityContext = entityContext;
        this.savingsGoalService = savingsGoalService;
    }

    [HttpGet("/add-transaction/")]
    public async Task<IActionResult> AddTransaction()
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.entityContext);
        this.ViewBag.Categories = Enum.GetValues(typeof(Categories));
        this.ViewBag.Sources = Enum.GetValues(typeof(Sources));
        return this.View();
    }

    [HttpPost("/add-transaction/")]
    public async Task<IActionResult> AddTransaction(Transaction model)
    {
        TransactionValidator.ValidateTransaction(model, this.ModelState);

        if (!this.ModelState.IsValid)
        {
            this.TempData["ErrorMessage"] = @TransactionText.InvalidDetails;
            return this.RedirectToAction("TransactionList");
        }

        var userId = this.User.GetUserId();
        try
        {
            await this.transactionService.AddTransactionAsync(model, userId);
            this.TempData["SuccessMessage"] = @TransactionText.TransactionSuccess;
        }
        catch (InvalidOperationException ex)
        {
            this.TempData["ErrorMessage"] = ex.Message;
        }

        return this.RedirectToAction("TransactionList");
    }

    [HttpGet("/transactions/")]
    public async Task<IActionResult> TransactionList(int page = 1, int pageSize = 5, Categories? category = null)
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.entityContext);
        var userId = this.User.GetUserId();

        var transactions = await this.transactionService.GetTransactionsAsync(userId, page, pageSize, category);
        var totalRecords = await this.transactionService.GetTotalTransactionRecordsAsync(userId);

        this.ViewBag.SelectedCategory = category?.ToString() ?? "Всички";

        var model = new TransactionsListViewModel
        {
            Transactions = transactions,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
            SelectedCategory = category,
        };

        return this.View(model);
    }

    [HttpGet("/edit-transaction/{id}")]
    public async Task<IActionResult> EditTransaction(Guid id)
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.entityContext);
        var transaction = await this.transactionService.GetTransactionByIdAsync(id, this.User.GetUserId());

        if (transaction == null)
        {
            return this.NotFound();
        }

        bool isIncome = transaction.Type == TransactionType.Income;

        this.ViewBag.IsIncome = isIncome;

        this.ViewBag.Categories = Enum.GetValues(typeof(Categories));
        this.ViewBag.Sources = Enum.GetValues(typeof(Sources));

        return this.View(transaction);
    }

    [HttpPost("/edit-transaction/{id}")]
    public async Task<IActionResult> EditTransaction(Guid id, Transaction model)
    {
        if (id != model.Id)
        {
            return this.BadRequest();
        }

        try
        {
            await this.transactionService.UpdateTransactionAsync(model);
            this.TempData["SuccessMessage"] = @TransactionText.UpdateSuccess;
        }
        catch (KeyNotFoundException)
        {
            this.TempData["ErrorMessage"] = @TransactionText.NotFound;
        }

        return this.RedirectToAction("TransactionList");
    }

    [HttpGet("/delete-transaction/{id}")]
    public async Task<IActionResult> DeleteTransaction(Guid id)
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.entityContext);
        var transaction = await this.transactionService.GetTransactionByIdAsync(id, this.User.GetUserId());

        if (transaction == null)
        {
            return this.NotFound();
        }

        return this.View(transaction);
    }

    [HttpPost("/delete-transaction/{id}")]
    public async Task<IActionResult> DeleteTransactionAsync(Guid id)
    {
        await this.transactionService.DeleteTransactionAsync(id, this.User.GetUserId());

        this.TempData["SuccessMessage"] = @TransactionText.DeleteSuccess;
        return this.RedirectToAction("TransactionList");
    }

    [HttpPost("/quick-add-transaction")]
    public async Task<IActionResult> QuickAddTransaction(Transaction transactionModel)
    {
        TransactionValidator.ValidateTransaction(transactionModel, this.ModelState);

        if (!this.ModelState.IsValid)
        {
            this.TempData["ErrorMessage"] = @TransactionText.InvalidDetails;
            return this.RedirectToAction("TransactionList");
        }

        var userId = this.User.GetUserId();

        try
        {
            await this.transactionService.QuickAddTransactionAsync(transactionModel, userId);
            this.TempData["SuccessMessage"] = @TransactionText.TransactionSuccess;
        }
        catch (InvalidOperationException ex)
        {
            this.TempData["ErrorMessage"] = ex.Message;
        }

        return this.RedirectToAction("TransactionList");
    }

    [HttpGet("/set-aside-savings/{transactionId}")]
    public async Task<IActionResult> SetAsideForSavings(Guid transactionId)
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.entityContext);

        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.RedirectToAction("Login", "Authentication");
        }

        var transaction = await this.transactionService.GetTransactionByIdAsync(transactionId, user.Id);
        if (transaction == null || transaction.Type != TransactionType.Income)
        {
            this.TempData["ErrorMessage"] = @TransactionText.InvalidTransaction;
            return this.RedirectToAction("TransactionList");
        }

        var savingsGoals = await this.savingsGoalService.SetAsideForSavingsGoalsAsync(transaction);

        if (!savingsGoals.Any())
        {
            this.TempData["ErrorMessage"] = @TransactionText.UnavailableGoals;
            return this.RedirectToAction("TransactionList");
        }

        if (savingsGoals.Count == 1)
        {
            this.TempData["SuccessMessage"] =
                $"10% от {transaction.Amount:C} са заделени за '{savingsGoals[0].Name}'.";
            return this.RedirectToAction("TransactionList");
        }

        return this.RedirectToAction("ChooseSavingsGoal", new { transactionId });
    }

    [HttpGet("/choose-savings-goal/{transactionId}")]
    public async Task<IActionResult> ChooseSavingsGoal(Guid transactionId)
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.entityContext);

        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.RedirectToAction("Login", "Authentication");
        }

        var transaction = await this.transactionService.GetTransactionByIdAsync(transactionId, user.Id);
        if (transaction == null || transaction.Type != TransactionType.Income)
        {
            this.TempData["ErrorMessage"] = @TransactionText.InvalidTransaction;
            return this.RedirectToAction("TransactionList");
        }

        var savingsGoals = await this.savingsGoalService.SetAsideForSavingsGoalsAsync(transaction);

        if (!savingsGoals.Any())
        {
            this.TempData["ErrorMessage"] = @TransactionText.UnavailableGoals;
            return this.RedirectToAction("TransactionList");
        }

        var model = new ChooseSavingsGoalViewModel
        {
            TransactionId = transactionId,
            AmountToSetAside = transaction.Amount * 0.10m,
            SavingsGoals = savingsGoals,
        };

        return this.View(model);
    }

    [HttpPost("/set-aside-savings-manual")]
    public async Task<IActionResult> SetAsideForSavingsManual(Guid transactionId, Guid goalId)
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.RedirectToAction("Login", "Authentication");
        }

        var transaction = await this.transactionService.GetTransactionByIdAsync(transactionId, user.Id);
        var goal = await this.savingsGoalService.GetSavingsGoalByIdAsync(user.Id, goalId);

        if (transaction == null || goal == null)
        {
            this.TempData["ErrorMessage"] = @TransactionText.InvalidSelection;
            return this.RedirectToAction("TransactionList");
        }

        decimal amountToSetAside = transaction.Amount * 0.10m;

        if (transaction.Amount < amountToSetAside)
        {
            this.TempData["ErrorMessage"] = @TransactionText.InsufficientFunds;
            return this.RedirectToAction("TransactionList");
        }

        transaction.Amount -= amountToSetAside;

        goal.CurrentAmount += amountToSetAside;

        if (goal.CurrentAmount > goal.TargetAmount)
        {
            goal.CurrentAmount = goal.TargetAmount;
        }

        await this.entityContext.SaveChangesAsync();

        this.TempData["SuccessMessage"] =
            $"10% от {transaction.Amount + amountToSetAside:C} са заделени за '{goal.Name}'.";
        return this.RedirectToAction("TransactionList");
    }
}