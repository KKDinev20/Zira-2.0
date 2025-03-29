using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zira.Services.SavingsGoal.Contracts;

public interface ISavingsGoalService
{
    Task<List<Data.Models.SavingsGoal>> GetUserSavingsGoalsAsync(
        Guid userId,
        int page,
        int pageSize,
        string targetCurrency);

    Task<Data.Models.SavingsGoal?> GetSavingsGoalByIdAsync(Guid userId, Guid goalsId);
    Task<bool> AddSavingsGoalsAsync(Data.Models.SavingsGoal goal, string userPreferredCurrency);
    Task<bool> UpdateSavingsGoalsAsync(Data.Models.SavingsGoal goal, string userPreferredCurrency);
    Task<int> GetTotalSavingsGoalsAsync(Guid userId);
    Task<bool> DeleteSavingsGoalsAsync(Data.Models.SavingsGoal goal);
    Task<List<Data.Models.SavingsGoal>> SetAsideForSavingsGoalsAsync(Data.Models.Transaction transaction);

    Task<List<Data.Models.SavingsGoal>> GetSavingsGoalsAsync(Guid userId, int page, int pageSize);
}