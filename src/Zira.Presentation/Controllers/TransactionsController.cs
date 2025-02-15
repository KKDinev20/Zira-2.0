using System;
using System.Collections.Generic;
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
using Zira.Services.Identity.Extensions;
using Zira.Services.Transaction.Contracts;

namespace Zira.Presentation.Controllers;

[Authorize(Policies.UserPolicy)]
public class TransactionsController : Controller
{
    private readonly ITransactionService transactionService;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly EntityContext entityContext;

    public TransactionsController(
        ITransactionService transactionService,
        UserManager<ApplicationUser> userManager,
        EntityContext entityContext)
    {
        this.transactionService = transactionService;
        this.userManager = userManager;
        this.entityContext = entityContext;
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
        if (model.Amount <= 0)
        {
            this.ModelState.AddModelError("Amount", "Amount must be positive.");
        }

        if (!this.ModelState.IsValid)
        {
            this.TempData["ErrorMessage"] = "Invalid transaction details.";
            return this.RedirectToAction("TransactionList");
        }

        var userId = this.User.GetUserId();
        try
        {
            await this.transactionService.AddTransactionAsync(model, userId);
            this.TempData["SuccessMessage"] = "Transactions successfully added!";
        }
        catch (InvalidOperationException ex)
        {
            this.TempData["ErrorMessage"] = ex.Message;
        }

        return this.RedirectToAction("TransactionList");
    }

    [HttpGet("/transactions/")]
    public async Task<IActionResult> TransactionList(int page = 1, int pageSize = 5)
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.entityContext);
        var userId = this.User.GetUserId();

        var totalRecords = await this.transactionService.GetTotalTransactionRecordsAsync(userId);
        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        var transactions = await this.transactionService.GetTransactionsAsync(userId, page, pageSize);

        var model = new TransactionsListViewModel
        {
            Transactions = transactions,
            CurrentPage = page,
            TotalPages = totalPages,
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
            this.TempData["SuccessMessage"] = "Transaction updated successfully!";
        }
        catch (KeyNotFoundException)
        {
            this.TempData["ErrorMessage"] = "Transaction not found.";
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

        this.TempData["SuccessMessage"] = "Transactions deleted successfully!";
        return this.RedirectToAction("TransactionList");
    }

    [HttpPost("/quick-add-transaction")]
    public async Task<IActionResult> QuickAddTransaction(Transaction transactionModel)
    {
        if (transactionModel.Amount <= 0)
        {
            this.ModelState.AddModelError("Amount", "Amount must be positive.");
        }

        if (!this.ModelState.IsValid)
        {
            this.TempData["ErrorMessage"] = "Invalid transaction details.";
            return this.RedirectToAction("TransactionList");
        }

        var userId = this.User.GetUserId();
        var user = await this.entityContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            this.ModelState.AddModelError("", "User not found.");
            this.TempData["ErrorMessage"] = "Transaction error.";
            return this.RedirectToAction("TransactionList");
        }

        transactionModel.Id = Guid.NewGuid();
        transactionModel.UserId = user.Id;
        transactionModel.Date = transactionModel.Date == default ? DateTime.UtcNow : transactionModel.Date;
        transactionModel.Type = TransactionType.Expense;

        var budget = await this.entityContext.Budgets
            .FirstOrDefaultAsync(
                b => b.UserId == user.Id
                     && b.Category == transactionModel.Category
                     && b.Month.Year == transactionModel.Date.Year
                     && b.Month.Month == transactionModel.Date.Month);

        if (budget != null)
        {
            budget.Amount -= transactionModel.Amount;

            if (budget.Amount < 0)
            {
                this.TempData["ErrorMessage"] = "Expense exceeds budget!";
                return this.RedirectToAction("TransactionList");
            }

            this.entityContext.Budgets.Update(budget);
        }

        this.entityContext.Transactions.Add(transactionModel);
        await this.entityContext.SaveChangesAsync();

        this.TempData["SuccessMessage"] = "Transaction successfully added!";
        return this.RedirectToAction("TransactionList");
    }
}