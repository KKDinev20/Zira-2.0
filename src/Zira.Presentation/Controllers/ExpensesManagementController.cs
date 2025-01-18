using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Services.Identity.Constants;
using Zira.Services.Identity.Extensions;

namespace Zira.Presentation.Controllers;

[Authorize(Policies.UserPolicy)]
public class ExpensesManagementController : Controller
{
    private readonly EntityContext context;

    public ExpensesManagementController(EntityContext context)
    {
        this.context = context;
    }

    [HttpGet("/add-expenses/")]
    public IActionResult AddExpenses()
    {
        this.ViewBag.Categories = Enum.GetValues(typeof(Categories));
        return this.View();
    }

    [HttpPost("/add-expenses")]
    public async Task<IActionResult> AddExpenses(Expense expenseModel)
    {
        if (this.ModelState.IsValid)
        {
            var userId = this.User.GetUserId();

            var user = await this.context.Users
                .FirstOrDefaultAsync(u => u.ApplicationUserId == userId);

            if (user == null)
            {
                this.ModelState.AddModelError(" ", "User does not exist.");
                return this.View(expenseModel);
            }

            expenseModel.ExpenseId = Guid.NewGuid();
            expenseModel.UserId = user.Id;

            if (expenseModel.DateSpent == default)
            {
                expenseModel.DateSpent = DateTime.UtcNow;
            }

            this.context.Expenses.Add(expenseModel);
            await this.context.SaveChangesAsync();

            return this.RedirectToAction("ExpensesList");
        }

        this.ViewBag.Sources = Enum.GetValues(typeof(Sources)).Cast<Sources>().ToList();
        return this.View(expenseModel);
    }

    [HttpGet("/expenses-list/")]
    public async Task<IActionResult> ExpensesList()
    {
        var userId = this.User.GetUserId();

        var user = await this.context.Users
            .FirstOrDefaultAsync(u => u.ApplicationUserId == userId);

        if (user == null)
        {
            return this.NotFound("User not found.");
        }

        var expenses = await this.context.Expenses
            .Where(i => i.UserId == user.Id)
            .ToListAsync();

        return this.View(expenses);
    }

    [HttpGet("/edit-expense/{id}")]
    public async Task<IActionResult> EditExpenses(Guid id)
    {
        var expense = await this.context.Expenses
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.ExpenseId == id && i.UserId == this.User.GetUserId());

        if (expense == null)
        {
            return this.NotFound();
        }

        this.ViewBag.Categories = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
        return this.View(expense);
    }

    [HttpPost("/edit-expense/{id}")]
    public async Task<IActionResult> EditExpenses(Guid id, Expense model)
    {
        if (id != model.ExpenseId)
        {
            return this.BadRequest();
        }

        if (this.ModelState.IsValid)
        {
            var expense = await this.context.Expenses
                .FirstOrDefaultAsync(i => i.ExpenseId == id && i.UserId == this.User.GetUserId());

            if (expense == null)
            {
                return this.NotFound();
            }

            expense.Category = model.Category;
            expense.Amount = model.Amount;
            expense.DateSpent = model.DateSpent;

            try
            {
                await this.context.SaveChangesAsync();
                return this.RedirectToAction("ExpensesList");
            }
            catch (DbUpdateException ex)
            {
                this.ModelState.AddModelError(" ", $"Error updating expense: {ex.Message}");
            }
        }

        this.ViewBag.Categories = Enum.GetValues(typeof(Categories));
        return this.View(model);
    }

    [HttpGet("/delete-expense/{id}")]
    public async Task<IActionResult> DeleteExpense(Guid id)
    {
        var expense = await this.context.Expenses
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.ExpenseId == id && i.UserId == this.User.GetUserId());

        if (expense == null)
        {
            return this.NotFound();
        }

        return this.View(expense);
    }

    [HttpPost("/delete-expense/{id}")]
    public async Task<IActionResult> DeleteExpenseAsync(Guid id)
    {
        var expense = await this.context.Expenses
            .FirstOrDefaultAsync(i => i.ExpenseId == id && i.UserId == this.User.GetUserId());

        if (expense == null)
        {
            return this.NotFound();
        }

        try
        {
            this.context.Expenses.Remove(expense);
            await this.context.SaveChangesAsync();
            return this.RedirectToAction("ExpensesList");
        }
        catch (DbUpdateException ex)
        {
            this.ModelState.AddModelError(" ", $"Error deleting expense: {ex.Message}");
            return this.View(expense);
        }
    }
}