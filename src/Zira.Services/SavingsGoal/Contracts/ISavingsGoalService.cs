using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zira.Services.SavingsGoal.Contracts;

public interface ISavingsGoalService
{
    Task<List<Data.Models.SavingsGoal>> GetUserSavingsGoalsAsync(Guid userId, int page, int pageSize);

    Task<Data.Models.SavingsGoal?> GetSavingsGoalByIdAsync(Guid userId, Guid goalsId);
    Task<bool> AddSavingsGoalsAsync(Data.Models.SavingsGoal goal);
    Task<bool> UpdateSavingsGoalsAsync(Data.Models.SavingsGoal goal);
    Task<int> GetTotalSavingsGoalsAsync(Guid userId);
    Task<bool> DeleteSavingsGoalsAsync(Data.Models.SavingsGoal goal);
}