using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zira.Services.Transaction.Contracts;

public interface ITransactionService
{
    Task<List<Data.Models.Transaction>> GetTransactionsAsync(Guid userId, int page, int pageSize);
    Task<int> GetTotalTransactionRecordsAsync(Guid userId);
    Task<Data.Models.Transaction?> GetTransactionByIdAsync(Guid id, Guid userId);
    Task AddTransactionAsync(Data.Models.Transaction transactionModel, Guid userId);
    Task UpdateTransactionAsync(Data.Models.Transaction transactionModel);
    Task DeleteTransactionAsync(Guid id, Guid userId);
}
