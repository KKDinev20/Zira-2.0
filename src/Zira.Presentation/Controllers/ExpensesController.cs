using System;
using System.Linq;
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

namespace Zira.Presentation.Controllers;

[Authorize(Policies.UserPolicy)]
public class ExpensesController : Controller
{
    private readonly EntityContext context;
    private readonly UserManager<ApplicationUser> userManager;

    public ExpensesController(EntityContext context, UserManager<ApplicationUser> userManager)
    {
        this.context = context;
        this.userManager = userManager;
    }

    [HttpGet("/add-expenses/")]
    public async Task<IActionResult> AddExpenses()
    {
        await this.SetGlobalUserInfoAsync(userManager, context);

        this.ViewBag.Categories = Enum.GetValues(typeof(Categories));
        return this.View();
    }

    [HttpPost("/add-expenses")]
    public async Task<IActionResult> AddExpenses(Expense expenseModel)
    {
        if (expenseModel.Amount <= 0)
        {
            this.ModelState.AddModelError(nameof(expenseModel.Amount), "Amount must be greater than zero.");
            this.RedirectToAction("ExpensesList");
        }

        if (!this.ModelState.IsValid)
        {
            this.TempData["ErrorMessage"] = "Failed to add expense. Please correct the errors and try again.";
            this.ViewBag.Categories = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
            this.RedirectToAction("ExpensesList");
        }

        var userId = this.User.GetUserId();
        var user = await this.context.Users.FirstOrDefaultAsync(u => u.ApplicationUserId == userId);

        if (user == null)
        {
            this.ModelState.AddModelError("", "User does not exist.");
            this.TempData["ErrorMessage"] = "Failed to add expense. User not found.";
            this.RedirectToAction("ExpensesList");
        }

        expenseModel.ExpenseId = Guid.NewGuid();
        expenseModel.UserId = user.Id;
        expenseModel.DateSpent = expenseModel.DateSpent == default ? DateTime.UtcNow : expenseModel.DateSpent;

        this.context.Expenses.Add(expenseModel);
        await this.context.SaveChangesAsync();

        this.TempData["SuccessMessage"] = "Expense added successfully!";
        return this.RedirectToAction("ExpensesList");
    }

    [HttpGet("/expenses-list/")]
    public async Task<IActionResult> ExpensesList(int page = 1, int pageSize = 10)
    {
        await this.SetGlobalUserInfoAsync(userManager, context);

        var userId = this.User.GetUserId();
        var user = await this.context.Users
            .FirstOrDefaultAsync(u => u.ApplicationUserId == userId);

        if (user == null)
        {
            return this.NotFound("User not found.");
        }

        var totalExpenses = await this.context.Expenses
            .Where(i => i.UserId == user.Id)
            .CountAsync();

        var totalPages = (int)Math.Ceiling(totalExpenses / (double)pageSize);

        var expenses = await this.context.Expenses
            .Where(i => i.UserId == user.Id)
            .OrderBy(i => i.DateSpent)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var model = new ExpensesListViewModel
        {
            Expenses = expenses,
            CurrentPage = page,
            TotalPages = totalPages,
        };

        return this.View(model);
    }

    [HttpGet("/edit-expenses/{id}")]
    public async Task<IActionResult> EditExpenses(Guid id)
    {
        var expense = await this.context.Expenses
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.ExpenseId == id && i.User.ApplicationUserId == this.User.GetUserId());

        if (expense == null)
        {
            return this.NotFound();
        }

        this.ViewBag.Categories = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();

        return this.View(expense);
    }

    [HttpPost("/edit-expenses/{id}")]
    public async Task<IActionResult> EditExpenses(Guid id, Expense model)
    {
        if (id != model.ExpenseId)
        {
            return this.BadRequest();
        }

        if (model.Amount <= 0)
        {
            this.ModelState.AddModelError(nameof(model.Amount), "Amount must be greater than zero.");
        }

        if (!this.ModelState.IsValid)
        {
            this.TempData["ErrorMessage"] = "Failed to update expense. Please correct the errors and try again.";
            this.ViewBag.Categories = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
            this.RedirectToAction("ExpensesList");
        }

        var expense = await this.context.Expenses
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.ExpenseId == id && i.User.ApplicationUserId == this.User.GetUserId());

        if (expense == null)
        {
            this.TempData["ErrorMessage"] = "Failed to update expense. Expense not found.";
            return this.NotFound();
        }

        expense.Category = model.Category;
        expense.Amount = model.Amount;
        expense.DateSpent = model.DateSpent;

        await this.context.SaveChangesAsync();
        this.TempData["SuccessMessage"] = "Expense updated successfully!";
        return this.RedirectToAction("ExpensesList");
    }

    [HttpGet("/delete-expenses/{id}")]
    public async Task<IActionResult> DeleteExpense(Guid id)
    {
        var expense = await this.context.Expenses
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.ExpenseId == id && i.User.ApplicationUserId == this.User.GetUserId());

        if (expense == null)
        {
            return this.NotFound();
        }

        return this.View(expense);
    }

    [HttpPost("/delete-expenses/{id}")]
    public async Task<IActionResult> DeleteExpenseAsync(Guid id)
    {
        var expense = await this.context.Expenses
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.ExpenseId == id && i.User.ApplicationUserId == this.User.GetUserId());

        if (expense == null)
        {
            return this.NotFound();
        }

        this.context.Expenses.Remove(expense);
        await this.context.SaveChangesAsync();

        return this.RedirectToAction("ExpensesList");
    }

    [HttpPost("/quick-add-expenses")]
    public async Task<IActionResult> QuickAddExpenses(Expense model)
    {
        if (model.Amount <= 0)
        {
            this.ModelState.AddModelError(nameof(model.Amount), "Amount must be greater than zero.");
        }

        if (model.DateSpent > DateTime.UtcNow)
        {
            this.ModelState.AddModelError(nameof(model.DateSpent), "Date spent cannot be in the future.");
        }

        if (string.IsNullOrWhiteSpace(model.Description))
        {
            this.ModelState.AddModelError(nameof(model.Description), "Description is required.");
        }

        if (!this.ModelState.IsValid)
        {
            this.TempData["ErrorMessage"] = "Failed to add expense. Please correct the errors and try again.";
            this.ViewBag.Categories = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
            return this.RedirectToAction("ExpensesList");
        }

        var userId = this.User.GetUserId();
        var user = await this.context.Users.FirstOrDefaultAsync(u => u.ApplicationUserId == userId);

        if (user == null)
        {
            this.ModelState.AddModelError("", "User not found.");
            this.TempData["ErrorMessage"] = "Failed to add expense. Please try again.";
            return this.RedirectToAction("ExpensesList");
        }

        model.ExpenseId = Guid.NewGuid();
        model.UserId = user.Id;

        if (model.DateSpent == default)
        {
            model.DateSpent = DateTime.UtcNow;
        }

        this.context.Expenses.Add(model);
        await this.context.SaveChangesAsync();

        this.TempData["SuccessMessage"] = "Expense added successfully!";
        return this.RedirectToAction("ExpensesList");
    }

}