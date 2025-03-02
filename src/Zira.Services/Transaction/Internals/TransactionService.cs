using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Data.Models;
using Zira.Services.Transaction.Contracts;
using Zira.Services.Transaction.Models;

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
        var now = DateTime.UtcNow;
        return await this.context.Transactions
            .Where(
                t => t.UserId == userId
                     && t.Type == TransactionType.Income
                     && t.Date.Year == now.Year
                     && t.Date.Month == now.Month)
            .SumAsync(t => t.Amount);
    }

    public async Task<decimal> GetCurrentMonthExpensesAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        return await this.context.Transactions
            .Where(
                t => t.UserId == userId
                     && t.Type == TransactionType.Expense
                     && t.Date.Year == now.Year
                     && t.Date.Month == now.Month)
            .SumAsync(t => t.Amount);
    }

    public async Task<decimal> GetCurrentMonthFoodExpense(Guid userId)
    {
        return await this.GetCurrentMonthExpenseByCategoryAsync(userId, Categories.Food);
    }

    public async Task<decimal> GetCurrentMonthUtilitiesExpense(Guid userId)
    {
        return await this.GetCurrentMonthExpenseByCategoryAsync(userId, Categories.Utilities);
    }

    public async Task<List<Data.Models.Transaction>> GetRecentTransactions(Guid userId)
    {
        return await this.context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .Take(6)
            .ToListAsync();
    }

    public async Task<(List<decimal> Incomes, List<decimal> Expenses)> GetMonthlyIncomeAndExpensesAsync(
        Guid userId,
        int year)
    {
        var transactions = await this.context.Transactions
            .Where(t => t.UserId == userId && t.Date.Year == year)
            .ToListAsync();

        var incomes = new List<decimal>(new decimal[12]);
        var expenses = new List<decimal>(new decimal[12]);

        foreach (var t in transactions)
        {
            int monthIndex = t.Date.Month - 1;
            if (t.Type == TransactionType.Income)
            {
                incomes[monthIndex] += t.Amount;
            }
            else if (t.Type == TransactionType.Expense)
            {
                expenses[monthIndex] += t.Amount;
            }
        }

        return (incomes, expenses);
    }

    public async Task<(List<decimal> MonthlyTotals, List<string> MonthLabels)> GetLastSixMonthsDataAsync(
        Guid userId,
        TransactionType type)
    {
        var today = DateTime.UtcNow;
        var startMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-5);
        List<decimal> totals = new List<decimal>();
        List<string> labels = new List<string>();

        for (int i = 0; i < 6; i++)
        {
            var month = startMonth.AddMonths(i);
            labels.Add(month.ToString("MMM"));

            decimal total = type switch
            {
                TransactionType.Income => await this.context.Transactions.Where(
                        t => t.UserId == userId && t.Type == TransactionType.Income && t.Date.Year == month.Year &&
                             t.Date.Month == month.Month)
                    .SumAsync(t => t.Amount),
                TransactionType.Expense => await this.context.Transactions.Where(
                        t => t.UserId == userId && t.Type == TransactionType.Expense && t.Date.Year == month.Year &&
                             t.Date.Month == month.Month)
                    .SumAsync(t => t.Amount),
                _ => 0,
            };

            totals.Add(total);
        }

        return (totals, labels);
    }

    public async Task<decimal> GetCurrentWeekTotalAsync(Guid userId, TransactionType type)
    {
        var today = DateTime.UtcNow;
        int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
        var monday = today.AddDays(-1 * diff).Date;
        var sunday = monday.AddDays(7);

        decimal total = 0;
        if (type == TransactionType.Income)
        {
            total = await this.context.Transactions
                .Where(
                    t => t.UserId == userId &&
                         t.Type == TransactionType.Income &&
                         t.Date >= monday &&
                         t.Date < sunday)
                .SumAsync(t => t.Amount);
        }
        else if (type == TransactionType.Expense)
        {
            total = await this.context.Transactions
                .Where(
                    t => t.UserId == userId &&
                         t.Type == TransactionType.Expense &&
                         t.Date >= monday &&
                         t.Date < sunday)
                .SumAsync(t => t.Amount);
        }

        return total;
    }

    public async Task<decimal> GetTotalIncome(ApplicationUser user)
    {
        return await this.context.Transactions
            .Where(t => t.UserId == user.Id && t.Type == TransactionType.Income)
            .SumAsync(t => t.Amount);
    }

    public async Task<decimal> GetTotalExpenses(ApplicationUser user)
    {
        return await this.context.Transactions
            .Where(t => t.UserId == user.Id && t.Type == TransactionType.Expense)
            .SumAsync(t => t.Amount);
    }

    public async Task<List<CategoryExpenseSummary>> GetTopExpenseCategoriesAsync(Guid userId, int top = 5)
    {
        var summaries = await this.context.Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionType.Expense && t.Category != null)
            .GroupBy(t => t.Category.Value)
            .Select(
                g => new CategoryExpenseSummary
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(t => t.Amount),
                })
            .ToListAsync();

        summaries.Sort((a, b) => b.TotalAmount.CompareTo(a.TotalAmount));

        if (summaries.Count <= top)
        {
            return summaries;
        }

        decimal kthValue = summaries[top - 1].TotalAmount;

        int low = 0, high = summaries.Count - 1;
        int split = summaries.Count;
        while (low <= high)
        {
            int mid = low + (high - low) / 2;
            if (summaries[mid].TotalAmount < kthValue)
            {
                split = mid;
                high = mid - 1;
            }
            else
            {
                low = mid + 1;
            }
        }

        var topSummaries = summaries.Take(top).ToList();
        return topSummaries;
    }

    private async Task<decimal> GetCurrentMonthExpenseByCategoryAsync(Guid userId, Categories category)
    {
        var now = DateTime.UtcNow;
        return await this.context.Transactions
            .Where(
                t => t.UserId == userId
                     && t.Type == TransactionType.Expense
                     && t.Category == category
                     && t.Date.Year == now.Year
                     && t.Date.Month == now.Month)
            .SumAsync(t => t.Amount);
    }
}