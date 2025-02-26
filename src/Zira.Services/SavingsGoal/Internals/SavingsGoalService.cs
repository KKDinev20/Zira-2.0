using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Services.SavingsGoal.Contracts;

namespace Zira.Services.SavingsGoal.Internals
{
    public class SavingsGoalService : ISavingsGoalService
    {
        private readonly EntityContext context;

        public SavingsGoalService(EntityContext context)
        {
            this.context = context;
        }

        public async Task<List<Data.Models.SavingsGoal>> GetUserSavingsGoalsAsync(Guid userId, int page, int pageSize)
        {
            return await this.context.SavingsGoals
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Data.Models.SavingsGoal?> GetSavingsGoalByIdAsync(Guid userId, Guid goalId)
        {
            return await this.context.SavingsGoals
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Id == goalId);
        }

        public async Task<bool> AddSavingsGoalsAsync(Data.Models.SavingsGoal goal)
        {
            var existingGoal = await this.context.SavingsGoals
                .FirstOrDefaultAsync(g => g.UserId == goal.UserId && g.Name == goal.Name);

            if (existingGoal != null)
            {
                return false;
            }

            await this.context.SavingsGoals.AddAsync(goal);
            await this.context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateSavingsGoalsAsync(Data.Models.SavingsGoal goal)
        {
            var existingGoal = await this.context.SavingsGoals.FindAsync(goal.Id);

            if (existingGoal == null)
            {
                return false;
            }

            existingGoal.Name = goal.Name;
            existingGoal.TargetAmount = goal.TargetAmount;
            existingGoal.CurrentAmount = goal.CurrentAmount;
            existingGoal.TargetDate = goal.TargetDate;

            await this.context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSavingsGoalsAsync(Data.Models.SavingsGoal goal)
        {
            var existingGoal = await this.context.SavingsGoals.FindAsync(goal.Id);
            if (existingGoal == null)
            {
                return false;
            }

            this.context.SavingsGoals.Remove(existingGoal);
            await this.context.SaveChangesAsync();
            return true;
        }
        
        public async Task<int> GetTotalSavingsGoalsAsync(Guid userId)
        {
            return await this.context.SavingsGoals.CountAsync(b => b.UserId == userId);
        }
    }
}