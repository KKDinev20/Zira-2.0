using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Services.Transaction.Contracts;

namespace Zira.Services.Transaction.Internals;

public class TransactionService : ITransactionService
{
    private readonly EntityContext context;

    public TransactionService(EntityContext context)
    {
        this.context = context;
    }

    public async Task<List<Data.Models.Transaction>> GetTransactionsAsync(
        Guid userId,
        int page,
        int pageSize,
        Categories? category = null)
    {
        var query = this.context.Transactions.Where(t => t.UserId == userId);

        if (category.HasValue)
        {
            query = query.Where(t => t.Type == TransactionType.Expense && t.Category == category.Value);
        }

        return await query
            .OrderByDescending(t => t.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalTransactionRecordsAsync(Guid userId)
    {
        return await this.context.Transactions.CountAsync(t => t.UserId == userId);
    }

    public async Task<Data.Models.Transaction?> GetTransactionByIdAsync(Guid id, Guid userId)
    {
        return await this.context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task AddTransactionAsync(Data.Models.Transaction transactionModel, Guid userId)
    {
        transactionModel.Id = Guid.NewGuid();
        transactionModel.UserId = userId;
        transactionModel.Date = transactionModel.Date == default ? DateTime.UtcNow : transactionModel.Date;

        var budget = await this.context.Budgets
            .FirstOrDefaultAsync(
                b => b.UserId == userId
                     && b.Category == transactionModel.Category
                     && b.Month.Year == transactionModel.Date.Year
                     && b.Month.Month == transactionModel.Date.Month);

        if (budget != null)
        {
            budget.Amount -= transactionModel.Amount;

            if (budget.Amount < 0)
            {
                throw new InvalidOperationException("Expense exceeds budget!");
            }

            this.context.Budgets.Update(budget);
        }

        this.context.Transactions.Add(transactionModel);
        await this.context.SaveChangesAsync();
    }

    public async Task UpdateTransactionAsync(Data.Models.Transaction transactionModel)
    {
        var existingTransaction = await this.context.Transactions.FindAsync(transactionModel.Id);
        if (existingTransaction != null)
        {
            existingTransaction.Description = transactionModel.Description;
            existingTransaction.Amount = transactionModel.Amount;
            existingTransaction.Date = transactionModel.Date;

            if (transactionModel.Type == TransactionType.Expense && transactionModel.Category != null)
            {
                existingTransaction.Category = transactionModel.Category;
            }
            else
            {
                existingTransaction.Source = transactionModel.Source;
            }

            await this.context.SaveChangesAsync();
        }
    }

    public async Task DeleteTransactionAsync(Guid id, Guid userId)
    {
        var transaction = await this.context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (transaction != null)
        {
            this.context.Transactions.Remove(transaction);
            await this.context.SaveChangesAsync();
        }
    }

    public async Task QuickAddTransactionAsync(Data.Models.Transaction transaction, Guid userId)
    {
        var user = await this.context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        transaction.Id = Guid.NewGuid();
        transaction.UserId = userId;
        transaction.Date = transaction.Date == default ? DateTime.UtcNow : transaction.Date;
        transaction.Type = TransactionType.Expense;

        var budget = await this.context.Budgets
            .FirstOrDefaultAsync(
                b => b.UserId == userId
                     && b.Category == transaction.Category
                     && b.Month.Year == transaction.Date.Year
                     && b.Month.Month == transaction.Date.Month);

        if (budget != null)
        {
            budget.Amount -= transaction.Amount;

            if (budget.Amount < 0)
            {
                throw new InvalidOperationException("Expense exceeds budget!");
            }

            this.context.Budgets.Update(budget);
        }

        this.context.Transactions.Add(transaction);
        await this.context.SaveChangesAsync();
    }

    public async Task<decimal> GetCurrentMonthIncomeAsync(Guid userId)
    {
        var startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var endDate = startDate.AddMonths(1).AddTicks(-1);

        return await this.context.Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionType.Income && t.Date >= startDate &&
                        t.Date <= endDate)
            .SumAsync(t => t.Amount);
    }

    public async Task<decimal> GetCurrentMonthExpensesAsync(Guid userId)
    {
        var startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var endDate = startDate.AddMonths(1).AddTicks(-1);

        return await this.context.Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionType.Expense && t.Date >= startDate &&
                        t.Date <= endDate)
            .SumAsync(t => t.Amount);
    }
}