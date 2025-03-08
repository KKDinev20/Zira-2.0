using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Services.Budget.Contracts;
using Zira.Services.Common.Contracts;

namespace Zira.Services.Budget.Internals
{
    public class BudgetService : IBudgetService
    {
        private readonly EntityContext context;
        private readonly IIdGenerationService idGenerationService;

        public BudgetService(EntityContext context, IIdGenerationService idGenerationService)
        {
            this.context = context;
            this.idGenerationService = idGenerationService;
        }

        public async Task<bool> AddBudgetAsync(Data.Models.Budget budget)
        {
            var existingBudget = await this.context.Budgets
                .FirstOrDefaultAsync(
                    b => b.UserId == budget.UserId &&
                         b.Category == budget.Category &&
                         b.Month.Year == budget.Month.Year &&
                         b.Month.Month == budget.Month.Month);

            if (existingBudget != null)
            {
                return false;
            }

            budget.Month = new DateTime(budget.Month.Year, budget.Month.Month, 1);
            budget.BudgetId = idGenerationService.GenerateDigitIdAsync();

            this.context.Budgets.Add(budget);
            await this.context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateBudgetAsync(Data.Models.Budget budget)
        {
            var existingBudget = await this.context.Budgets.FindAsync(budget.Id);
            if (existingBudget == null || existingBudget.UserId != budget.UserId)
            {
                return false;
            }

            existingBudget.Amount = budget.Amount;
            existingBudget.WarningThreshold = budget.WarningThreshold;
            existingBudget.Category = budget.Category;
            existingBudget.Month = new DateTime(budget.Month.Year, budget.Month.Month, 1);

            await this.context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBudgetAsync(Guid budgetId, Guid userId)
        {
            var budget = await this.context.Budgets.FirstOrDefaultAsync(b => b.Id == budgetId && b.UserId == userId);
            if (budget == null)
            {
                return false;
            }

            this.context.Budgets.Remove(budget);
            await this.context.SaveChangesAsync();
            return true;
        }

        public async Task<Data.Models.Budget?> GetBudgetByIdAsync(Guid budgetId, Guid userId)
        {
            return await this.context.Budgets
                .FirstOrDefaultAsync(b => b.Id == budgetId && b.UserId == userId);
        }

        public async Task<List<Data.Models.Budget>> GetUserBudgetsAsync(Guid userId, int page, int pageSize)
        {
            return await this.context.Budgets
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Month)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalBudgetsAsync(Guid userId)
        {
            return await this.context.Budgets.CountAsync(b => b.UserId == userId);
        }
    }
}