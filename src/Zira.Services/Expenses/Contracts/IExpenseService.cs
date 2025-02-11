using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zira.Data;
using Zira.Data.Models;

namespace Zira.Services.Expenses.Contracts;

public interface IExpenseService
{
    Task AddExpenseAsync(Expense expenseModel, Guid userId);
    Task<List<Expense>> GetExpensesAsync(Guid userId, int page, int pageSize);
    Task<Expense> GetExpenseByIdAsync(Guid id, Guid userId);
    Task UpdateExpenseAsync(Expense expenseModel, Guid userId);
    Task DeleteExpenseAsync(Guid id, Guid userId);
}