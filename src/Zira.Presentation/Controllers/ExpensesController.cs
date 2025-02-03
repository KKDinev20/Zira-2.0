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
using Zira.Presentation.Validations;
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
        await this.SetGlobalUserInfoAsync(this.userManager, this.context);

        this.ViewBag.Categories = Enum.GetValues(typeof(Categories));
        return this.View();
    }

    [HttpPost("/add-expenses")]
    public async Task<IActionResult> AddExpenses(Expense expenseModel)
    {
        ExpenseValidations.ValidateExpense(expenseModel, this.ModelState);

        if (!this.ModelState.IsValid)
        {
            this.TempData["ErrorMessage"] = @ExpensesText.ExpenseError;
            this.ViewBag.Categories = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
            return this.RedirectToAction("ExpensesList");
        }

        var userId = this.User.GetUserId();
        var user = await this.context.Users.FirstOrDefaultAsync(u => u.ApplicationUserId == userId);

        if (user == null)
        {
            this.ModelState.AddModelError("", @AuthenticationText.UserNotExisting);
            this.TempData["ErrorMessage"] = @ExpensesText.UserNotExists;
            return this.RedirectToAction("ExpensesList");
        }

        expenseModel.ExpenseId = Guid.NewGuid();
        expenseModel.UserId = user.Id;
        expenseModel.DateSpent = expenseModel.DateSpent == default ? DateTime.UtcNow : expenseModel.DateSpent;

        var budget = await this.context.Budgets
            .FirstOrDefaultAsync(
                b => b.UserId == user.Id
                     && b.Category == expenseModel.Category
                     && b.Month.Year == expenseModel.DateSpent.Year
                     && b.Month.Month == expenseModel.DateSpent.Month);

        if (budget != null)
        {
            budget.Amount -= expenseModel.Amount;

            if (budget.Amount < 0)
            {
                this.TempData["ErrorMessage"] = @ExpensesText.ExceedingExpenses;
                return this.RedirectToAction("ExpensesList");
            }

            this.context.Budgets.Update(budget);
        }

        this.context.Expenses.Add(expenseModel);
        await this.context.SaveChangesAsync();

        this.TempData["SuccessMessage"] = @ExpensesText.ExpenseSuccess;
        return this.RedirectToAction("ExpensesList");
    }

    [HttpGet("/expenses-list/")]
    public async Task<IActionResult> ExpensesList(int page = 1, int pageSize = 10)
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.context);

        var userId = this.User.GetUserId();
        var user = await this.context.Users
            .FirstOrDefaultAsync(u => u.ApplicationUserId == userId);

        if (user == null)
        {
            return this.NotFound(@AuthenticationText.UserNotExisting);
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

        ExpenseValidations.ValidateExpense(model, this.ModelState);

        if (!this.ModelState.IsValid)
        {
            this.TempData["ErrorMessage"] = @ExpensesText.ExpenseError;
            this.ViewBag.Categories = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
            return this.RedirectToAction("ExpensesList");
        }

        var expense = await this.context.Expenses
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.ExpenseId == id && i.User.ApplicationUserId == this.User.GetUserId());

        if (expense == null)
        {
            this.TempData["ErrorMessage"] = @ExpensesText.ExpenseNotFound;
            return this.NotFound();
        }

        expense.Category = model.Category;
        expense.Amount = model.Amount;
        expense.DateSpent = model.DateSpent;

        await this.context.SaveChangesAsync();
        this.TempData["SuccessMessage"] = @ExpensesText.ExpenseSuccess;
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
    public async Task<IActionResult> QuickAddExpenses(Expense expenseModel)
    {
        ExpenseValidations.ValidateExpense(expenseModel, this.ModelState);

        if (!this.ModelState.IsValid)
        {
            this.TempData["ErrorMessage"] = @ExpensesText.ExpenseError;
            this.ViewBag.Categories = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
            return this.RedirectToAction("ExpensesList");
        }

        var userId = this.User.GetUserId();
        var user = await this.context.Users.FirstOrDefaultAsync(u => u.ApplicationUserId == userId);

        if (user == null)
        {
            this.ModelState.AddModelError("", @AuthenticationText.UserNotExisting);
            this.TempData["ErrorMessage"] = @ExpensesText.ExpenseError;
            return this.RedirectToAction("ExpensesList");
        }

        expenseModel.ExpenseId = Guid.NewGuid();
        expenseModel.UserId = user.Id;

        if (expenseModel.DateSpent == default)
        {
            expenseModel.DateSpent = DateTime.UtcNow;
        }

        var budget = await this.context.Budgets
            .FirstOrDefaultAsync(
                b => b.UserId == user.Id
                     && b.Category == expenseModel.Category
                     && b.Month.Year == expenseModel.DateSpent.Year
                     && b.Month.Month == expenseModel.DateSpent.Month);

        if (budget != null)
        {
            budget.Amount -= expenseModel.Amount;

            if (budget.Amount < 0)
            {
                this.TempData["ErrorMessage"] = @ExpensesText.ExceedingExpenses;
                return this.RedirectToAction("ExpensesList");
            }

            this.context.Budgets.Update(budget);
        }

        this.context.Expenses.Add(expenseModel);
        await this.context.SaveChangesAsync();

        this.TempData["SuccessMessage"] = @ExpensesText.ExpenseSuccess;
        return this.RedirectToAction("ExpensesList");
    }
}