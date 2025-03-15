using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Data.Enums;
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

            budget.BudgetId = this.idGenerationService.GenerateDigitIdAsync();

            decimal totalSpent = 0;
            if (await this.context.Transactions.AnyAsync(
                    t => t.UserId == budget.UserId &&
                         t.Category == budget.Category &&
                         t.Date.Year == budget.Month.Year &&
                         t.Date.Month == budget.Month.Month))
            {
                totalSpent = await this.context.Transactions
                    .Where(
                        t => t.UserId == budget.UserId &&
                             t.Type == TransactionType.Expense &&
                             t.Category == budget.Category &&
                             t.Date.Year == budget.Month.Year &&
                             t.Date.Month == budget.Month.Month)
                    .SumAsync(t => t.Amount);
            }

            budget.SpentPercentage = budget.Amount > 0 ? (totalSpent / budget.Amount) * 100 : 0;

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
            existingBudget.Remark = budget.Remark;

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
            var budgets = await this.context.Budgets
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Month)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var budget in budgets)
            {
                var totalSpent = await this.context.Transactions
                    .Where(
                        t => t.UserId == userId &&
                             t.Type == TransactionType.Expense &&
                             t.Category == budget.Category &&
                             t.Date.Year == budget.Month.Year &&
                             t.Date.Month == budget.Month.Month)
                    .SumAsync(t => t.Amount);

                budget.SpentPercentage = budget.Amount > 0 ? (totalSpent / budget.Amount) * 100 : 0;
            }

            return budgets;
        }

        public async Task<int> GetTotalBudgetsAsync(Guid userId)
        {
            return await this.context.Budgets.CountAsync(b => b.UserId == userId);
        }

        public async Task<List<string>> GetBudgetWarningsAsync(Guid userId)
        {
            var warnings = new List<string>();

            var budgets = await this.context.Budgets
                .Where(b => b.UserId == userId)
                .ToListAsync();

            foreach (var budget in budgets)
            {
                var totalExpenses = await this.context.Transactions
                    .Where(
                        t => t.UserId == userId &&
                             t.Type == TransactionType.Expense &&
                             t.Category == budget.Category &&
                             t.Date.Year == budget.Month.Year &&
                             t.Date.Month == budget.Month.Month)
                    .SumAsync(t => t.Amount);

                decimal expensePercentage = (totalExpenses / budget.Amount) * 100;

                if (expensePercentage >= 50 && expensePercentage < 100)
                {
                    warnings.Add(
                        $"⚠️ Warning: You have used {expensePercentage:F2}% of your budget for {budget.Category}.");
                }
                else if (expensePercentage >= 100)
                {
                    warnings.Add(
                        $"🚨 Alert: You have exceeded your budget for {budget.Category} by {expensePercentage - 100:F2}%.");
                }
            }

            return warnings;
        }
    }
}