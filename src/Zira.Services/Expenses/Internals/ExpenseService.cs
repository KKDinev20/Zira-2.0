using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Data.Models;
using Zira.Services.Expenses.Contracts;

namespace Zira.Services.Expenses.Internals;

public class ExpenseService : IExpenseService
{
    private readonly EntityContext context;

    public ExpenseService(EntityContext context)
    {
        this.context = context;
    }

    public async Task AddExpenseAsync(Expense expenseModel, Guid userId)
    {
        var user = await this.context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        expenseModel.ExpenseId = Guid.NewGuid();
        expenseModel.UserId = user.Id;
        expenseModel.DateSpent = expenseModel.DateSpent == default ? DateTime.UtcNow : expenseModel.DateSpent;

        var budget = await this.context.Budgets
            .FirstOrDefaultAsync(b => b.UserId == user.Id && b.Category == expenseModel.Category &&
                                      b.Month.Year == expenseModel.DateSpent.Year &&
                                      b.Month.Month == expenseModel.DateSpent.Month);

        if (budget != null)
        {
            budget.Amount -= expenseModel.Amount;
            if (budget.Amount < 0)
            {
                throw new Exception("Exceeding expenses");
            }

            this.context.Budgets.Update(budget);
        }

        this.context.Expenses.Add(expenseModel);
        await this.context.SaveChangesAsync();
    }

    public async Task<List<Expense>> GetExpensesAsync(Guid userId, int page, int pageSize)
    {
        return await this.context.Expenses
            .Where(i => i.UserId == userId)
            .OrderBy(i => i.DateSpent)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Expense> GetExpenseByIdAsync(Guid id, Guid userId)
    {
        return await this.context.Expenses
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.ExpenseId == id && i.User.Id == userId);
    }

    public async Task UpdateExpenseAsync(Expense expenseModel, Guid userId)
    {
        var expense = await this.context.Expenses
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.ExpenseId == expenseModel.ExpenseId && i.User.Id == userId);

        if (expense == null)
        {
            throw new Exception("Expense not found");
        }

        expense.Category = expenseModel.Category;
        expense.Amount = expenseModel.Amount;
        expense.DateSpent = expenseModel.DateSpent;

        await this.context.SaveChangesAsync();
    }

    public async Task DeleteExpenseAsync(Guid id, Guid userId)
    {
        var expense = await this.context.Expenses
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.ExpenseId == id && i.User.Id == userId);

        if (expense == null)
        {
            throw new Exception("Expense not found");
        }

        this.context.Expenses.Remove(expense);
        await this.context.SaveChangesAsync();
    }
}