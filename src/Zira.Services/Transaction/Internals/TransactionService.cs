using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Data.Models;
using Zira.Services.Common.Contracts;
using Zira.Services.Currency.Contracts;
using Zira.Services.Transaction.Contracts;
using Zira.Services.Transaction.Models;

namespace Zira.Services.Transaction.Internals;

public class TransactionService : ITransactionService
{
    private readonly EntityContext context;
    private readonly IIdGenerationService idGenerationService;
    private readonly ICurrencyConverter currencyConverter;
    private readonly UserManager<ApplicationUser> userManager;

    public TransactionService(
        EntityContext context,
        IIdGenerationService idGenerationService,
        ICurrencyConverter currencyConverter,
        UserManager<ApplicationUser> userManager)
    {
        this.context = context;
        this.idGenerationService = idGenerationService;
        this.currencyConverter = currencyConverter;
        this.userManager = userManager;
    }

    public async Task<List<Data.Models.Transaction>> GetTransactionsAsync(
        Guid userId,
        int page,
        int pageSize,
        Categories? category = null)
    {
        var query = this.context.Transactions
            .Include(t => t.Currency)
            .Where(t => t.UserId == userId);

        if (category.HasValue)
        {
            query = query.Where(t => t.Category == category);
        }

        var transactions = await query
            .OrderByDescending(t => t.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var user = await this.userManager.FindByIdAsync(userId.ToString());
        if (user != null && !string.IsNullOrEmpty(user.PreferredCurrencyCode))
        {
            foreach (var transaction in transactions)
            {
                string transactionCurrencyCode = transaction.Currency?.Code ?? "BGN";

                if (transactionCurrencyCode != user.PreferredCurrencyCode)
                {
                    transaction.Amount = await this.currencyConverter.ConvertCurrencyAsync(
                        userId,
                        transaction.Amount,
                        transactionCurrencyCode,
                        user.PreferredCurrencyCode);
                }
            }
        }

        return transactions;
    }

    public async Task<List<Data.Models.Transaction>> GetUserTransactionsAsync(Guid userId)
    {
        var transactions = await this.context.Transactions
            .Include(t => t.Currency)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();

        var user = await this.userManager.FindByIdAsync(userId.ToString());
        if (user != null && !string.IsNullOrEmpty(user.PreferredCurrencyCode))
        {
            foreach (var transaction in transactions)
            {
                if (transaction.Currency?.Code != user.PreferredCurrencyCode)
                {
                    transaction.Amount = await this.currencyConverter.ConvertCurrencyAsync(
                        userId,
                        transaction.Amount,
                        transaction.Currency?.Code ?? "BGN",
                        user.PreferredCurrencyCode);
                }
            }
        }

        return transactions;
    }

    public async Task<int> GetTotalTransactionRecordsAsync(Guid userId)
    {
        return await this.context.Transactions.CountAsync(t => t.UserId == userId);
    }

    public async Task<Data.Models.Transaction?> GetTransactionByIdAsync(Guid id, Guid userId)
    {
        var transaction = await this.context.Transactions
            .Include(t => t.Currency)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (transaction == null)
        {
            return null;
        }

        var user = await this.userManager.FindByIdAsync(userId.ToString());
        if (user != null && !string.IsNullOrEmpty(user.PreferredCurrencyCode) &&
            transaction.Currency?.Code != user.PreferredCurrencyCode)
        {
            transaction.Amount = await this.currencyConverter.ConvertCurrencyAsync(
                userId,
                transaction.Amount,
                transaction.Currency?.Code ?? "BGN",
                user.PreferredCurrencyCode);
        }

        return transaction;
    }

    public async Task AddTransactionAsync(Data.Models.Transaction transactionModel, Guid userId)
    {
        transactionModel.Id = Guid.NewGuid();
        transactionModel.UserId = userId;
        transactionModel.Date = transactionModel.Date == default ? DateTime.UtcNow : transactionModel.Date;
        transactionModel.TransactionId = this.idGenerationService.GenerateDigitIdAsync();

        var user = await this.userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            throw new InvalidOperationException("User not found!");
        }

        transactionModel.CurrencyCode = user.PreferredCurrencyCode ?? "BGN";

        transactionModel.Currency = await this.context.Currencies
            .FirstOrDefaultAsync(c => c.Code == transactionModel.CurrencyCode);

        if (transactionModel.Currency == null)
        {
            throw new InvalidOperationException("Currency not found!");
        }

        var budget = await this.context.Budgets
            .FirstOrDefaultAsync(
                b => b.UserId == userId
                     && b.Category == transactionModel.Category
                     && b.Month.Year == transactionModel.Date.Year
                     && b.Month.Month == transactionModel.Date.Month);

        if (budget != null && transactionModel.Type == TransactionType.Expense)
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
        var existingTransaction = await this.context.Transactions
            .Include(t => t.Currency)
            .FirstOrDefaultAsync(t => t.Id == transactionModel.Id);

        if (existingTransaction == null)
        {
            throw new KeyNotFoundException("Transaction not found");
        }

        var user = await this.userManager.FindByIdAsync(existingTransaction.UserId.ToString());

        if (user == null)
        {
            throw new InvalidOperationException("User not found!");
        }

        string originalCurrencyCode = existingTransaction.CurrencyCode;
        string newCurrencyCode = user.PreferredCurrencyCode ?? "BGN";

        existingTransaction.Remark = transactionModel.Remark;
        existingTransaction.Reference = transactionModel.Reference;
        existingTransaction.Date = transactionModel.Date;

        if (originalCurrencyCode != newCurrencyCode)
        {
            existingTransaction.Amount = await this.currencyConverter.ConvertCurrencyAsync(
                existingTransaction.UserId,
                existingTransaction.Amount,
                originalCurrencyCode,
                newCurrencyCode);

            existingTransaction.CurrencyCode = newCurrencyCode;
            existingTransaction.Currency = await this.context.Currencies
                .FirstOrDefaultAsync(c => c.Code == newCurrencyCode);
        }

        await this.context.SaveChangesAsync();
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
        var user = await this.userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new InvalidOperationException("User not found!");
        }

        transaction.Id = Guid.NewGuid();
        transaction.UserId = userId;
        transaction.Date = transaction.Date == default ? DateTime.UtcNow : transaction.Date;
        transaction.Type = TransactionType.Expense;
        transaction.TransactionId = this.idGenerationService.GenerateDigitIdAsync();

        transaction.CurrencyCode = user.PreferredCurrencyCode ?? "BGN";

        transaction.Currency = await this.context.Currencies
            .FirstOrDefaultAsync(c => c.Code == transaction.CurrencyCode);

        if (transaction.Currency == null)
        {
            throw new InvalidOperationException("Currency not found!");
        }

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
        var income = await this.context.Transactions
            .Where(
                t => t.UserId == userId
                     && t.Type == TransactionType.Income
                     && t.Date.Year == now.Year
                     && t.Date.Month == now.Month)
            .SumAsync(t => t.Amount);

        var user = await this.userManager.FindByIdAsync(userId.ToString());
        if (user != null && !string.IsNullOrEmpty(user.PreferredCurrencyCode))
        {
            income = await this.currencyConverter.ConvertCurrencyAsync(
                userId,
                income,
                "BGN",
                user.PreferredCurrencyCode);
        }

        return income;
    }

    public async Task<decimal> GetCurrentMonthExpensesAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        var expenses = await this.context.Transactions
            .Where(
                t => t.UserId == userId
                     && t.Type == TransactionType.Expense
                     && t.Date.Year == now.Year
                     && t.Date.Month == now.Month)
            .SumAsync(t => t.Amount);

        var user = await this.userManager.FindByIdAsync(userId.ToString());
        if (user != null && !string.IsNullOrEmpty(user.PreferredCurrencyCode))
        {
            expenses = await this.currencyConverter.ConvertCurrencyAsync(
                userId,
                expenses,
                "BGN",
                user.PreferredCurrencyCode);
        }

        return expenses;
    }

    public async Task<decimal> GetCurrentMonthFoodExpense(Guid userId)
    {
        return await this.ConvertExpenseByCategoryAsync(userId, Categories.Food);
    }

    public async Task<decimal> GetCurrentMonthUtilitiesExpense(Guid userId)
    {
        return await this.ConvertExpenseByCategoryAsync(userId, Categories.Utilities);
    }

    public async Task<List<Data.Models.Transaction>> GetRecentTransactions(Guid userId)
    {
        var transactions = await this.context.Transactions
            .Include(t => t.Currency)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .Take(6)
            .ToListAsync();

        var user = await this.userManager.FindByIdAsync(userId.ToString());
        if (user != null && !string.IsNullOrEmpty(user.PreferredCurrencyCode))
        {
            foreach (var transaction in transactions)
            {
                if (transaction.Currency?.Code != user.PreferredCurrencyCode)
                {
                    transaction.Amount = await this.currencyConverter.ConvertCurrencyAsync(
                        userId,
                        transaction.Amount,
                        transaction.Currency?.Code ?? "BGN",
                        user.PreferredCurrencyCode);
                }
            }
        }

        return transactions;
    }

    public async Task<(List<decimal> Incomes, List<decimal> Expenses)> GetMonthlyIncomeAndExpensesAsync(Guid userId, int year)
    {
        var transactions = await this.context.Transactions
            .Include(t => t.Currency)
            .Where(t => t.UserId == userId && t.Date.Year == year)
            .ToListAsync();

        var user = await this.userManager.FindByIdAsync(userId.ToString());
        var preferredCurrency = user?.PreferredCurrencyCode ?? "BGN";

        var incomes = new List<decimal>(new decimal[12]);
        var expenses = new List<decimal>(new decimal[12]);

        foreach (var t in transactions)
        {
            int monthIndex = t.Date.Month - 1;
            decimal amount = t.Amount;
            

            if (t.Type == TransactionType.Income)
            {
                incomes[monthIndex] += amount;
            }
            else if (t.Type == TransactionType.Expense)
            {
                expenses[monthIndex] += amount;
            }
        }

        return (incomes, expenses);
    }


    public async Task<List<(DateTime Month, decimal NetWorth)>> GetNetWorthTrendAsync(Guid userId)
    {
        var transactions = await this.GetUserTransactionsAsync(userId);

        return transactions
            .GroupBy(t => new { t.Date.Year, t.Date.Month })
            .Select(
                g => (
                    Month: new DateTime(g.Key.Year, g.Key.Month, 1),
                    NetWorth: g.Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount)))
            .OrderBy(x => x.Month)
            .ToList();
    }

    public async Task<(List<decimal> MonthlyTotals, List<string> MonthLabels)> GetLastSixMonthsDataAsync(Guid userId, TransactionType type)
    {
        var today = DateTime.UtcNow;
        var startMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-5);
        List<decimal> totals = new List<decimal>();
        List<string> labels = new List<string>();

        var user = await this.userManager.FindByIdAsync(userId.ToString());
        var preferredCurrency = user?.PreferredCurrencyCode ?? "BGN";

        for (int i = 0; i < 6; i++)
        {
            var month = startMonth.AddMonths(i);
            labels.Add(month.ToString("MMM"));

            decimal total = await this.context.Transactions
                .Where(
                    t => t.UserId == userId &&
                         t.Type == type &&
                         t.Date.Year == month.Year &&
                         t.Date.Month == month.Month)
                .SumAsync(t => t.Amount);

            total = await this.currencyConverter.ConvertCurrencyAsync(userId, total, "BGN", preferredCurrency);
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

    public async Task<List<CategoryExpenseSummary>> GetTopExpenseCategoriesAsync(Guid userId, int top = 5)
    {
        var user = await this.userManager.FindByIdAsync(userId.ToString());
        var preferredCurrency = user?.PreferredCurrencyCode ?? "BGN";

        var summaries = await this.context.Transactions
            .Include(t => t.Currency)
            .Where(t => t.UserId == userId && t.Type == TransactionType.Expense && t.Category != null)
            .GroupBy(t => t.Category.Value)
            .Select(
                g => new CategoryExpenseSummary
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(t => t.Amount),
                })
            .ToListAsync();

        foreach (var summary in summaries)
        {
            summary.TotalAmount = await this.currencyConverter.ConvertCurrencyAsync(userId, summary.TotalAmount, "BGN", preferredCurrency);
        }

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

        foreach (var summary in summaries)
        {
            summary.TotalAmount = await this.currencyConverter.ConvertCurrencyAsync(
                userId,
                summary.TotalAmount,
                "BGN",
                preferredCurrency);
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

    private async Task<decimal> ConvertExpenseByCategoryAsync(Guid userId, Categories category)
    {
        var now = DateTime.UtcNow;
        var expense = await this.context.Transactions
            .Where(
                t => t.UserId == userId
                     && t.Type == TransactionType.Expense
                     && t.Category == category
                     && t.Date.Year == now.Year
                     && t.Date.Month == now.Month)
            .SumAsync(t => t.Amount);

        var user = await this.userManager.FindByIdAsync(userId.ToString());
        if (user != null && !string.IsNullOrEmpty(user.PreferredCurrencyCode))
        {
            expense = await this.currencyConverter.ConvertCurrencyAsync(
                userId,
                expense,
                "BGN",
                user.PreferredCurrencyCode);
        }

        return expense;
    }
}