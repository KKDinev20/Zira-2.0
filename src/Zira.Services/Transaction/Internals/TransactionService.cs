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

    public async Task<List<Data.Models.Transaction>> GetTransactionsAsync(Guid userId, int page, int pageSize)
    {
        return await this.context.Transactions
            .Where(t => t.UserId == userId)
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
}