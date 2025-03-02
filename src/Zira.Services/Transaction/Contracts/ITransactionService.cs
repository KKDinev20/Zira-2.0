using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zira.Data.Enums;
using Zira.Data.Models;
using Zira.Services.Transaction.Models;

namespace Zira.Services.Transaction.Contracts;

public interface ITransactionService
{
    Task<List<Data.Models.Transaction>> GetTransactionsAsync(Guid userId, int page, int pageSize, Categories? category);

    Task<int> GetTotalTransactionRecordsAsync(Guid userId);

    Task<Data.Models.Transaction?> GetTransactionByIdAsync(Guid id, Guid userId);

    Task AddTransactionAsync(Data.Models.Transaction transactionModel, Guid userId);

    Task UpdateTransactionAsync(Data.Models.Transaction transactionModel);

    Task DeleteTransactionAsync(Guid id, Guid userId);

    Task QuickAddTransactionAsync(Data.Models.Transaction transaction, Guid userId);

    Task<decimal> GetCurrentMonthIncomeAsync(Guid userId);

    Task<decimal> GetCurrentMonthExpensesAsync(Guid userId);

    Task<decimal> GetCurrentMonthFoodExpense(Guid userId);

    Task<decimal> GetCurrentMonthUtilitiesExpense(Guid userId);

    Task<List<Data.Models.Transaction>> GetRecentTransactions(Guid userId);

    Task<(List<decimal> MonthlyTotals, List<string> MonthLabels)> GetLastSixMonthsDataAsync(
        Guid userId,
        TransactionType type);

    Task<decimal> GetCurrentWeekTotalAsync(Guid userId, TransactionType type);

    Task<List<CategoryExpenseSummary>> GetTopExpenseCategoriesAsync(Guid userId, int top = 5);

    Task<(List<decimal> Incomes, List<decimal> Expenses)> GetMonthlyIncomeAndExpensesAsync(Guid userId, int year);
}