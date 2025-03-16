using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zira.Services.Budget.Contracts
{
    public interface IBudgetService
    {
        Task<bool> AddBudgetAsync(Data.Models.Budget budget, Guid userId);
        Task<bool> UpdateBudgetAsync(Data.Models.Budget budget);
        Task<bool> DeleteBudgetAsync(Guid budgetId, Guid userId);
        Task<Data.Models.Budget?> GetBudgetByIdAsync(Guid budgetId, Guid userId);
        Task<List<Data.Models.Budget>> GetUserBudgetsAsync(Guid userId, int page, int pageSize);
        Task<int> GetTotalBudgetsAsync(Guid userId);

        Task<List<string>> GetBudgetWarningsAsync(Guid userId);
    }
}