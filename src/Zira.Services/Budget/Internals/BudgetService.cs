using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Zira.Common;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Data.Models;
using Zira.Services.Budget.Contracts;
using Zira.Services.Common.Contracts;
using Zira.Services.Currency.Contracts;

namespace Zira.Services.Budget.Internals
{
    public class BudgetService : IBudgetService
    {
        private readonly EntityContext context;
        private readonly IIdGenerationService idGenerationService;
        private readonly ICurrencyConverter currencyConverter;
        private readonly UserManager<ApplicationUser> userManager;

        public BudgetService(
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

        public async Task<bool> AddBudgetAsync(Data.Models.Budget budget, Guid userId)
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

            if (string.IsNullOrEmpty(budget.CurrencyCode))
            {
                var user = await this.userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    throw new InvalidOperationException("User not found!");
                }

                budget.CurrencyCode = user.PreferredCurrencyCode ?? "BGN";
            }

            budget.Currency = await this.context.Currencies
                .FirstOrDefaultAsync(c => c.Code == budget.CurrencyCode);

            if (budget.Currency == null)
            {
                throw new InvalidOperationException($"Currency with code '{budget.CurrencyCode}' not found!");
            }

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

            var user = await this.userManager.FindByIdAsync(existingBudget.UserId.ToString());
            if (user == null)
            {
                throw new InvalidOperationException("User not found!");
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
                .Include(b => b.Currency)
                .OrderByDescending(b => b.Month)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var user = await this.userManager.FindByIdAsync(userId.ToString());
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
                    .ToListAsync();

                decimal convertedTotalExpenses = 0;
                foreach (var expense in totalExpenses)
                {
                    convertedTotalExpenses += await this.currencyConverter.ConvertCurrencyAsync(
                        userId,
                        expense.Amount,
                        expense.CurrencyCode,
                        budget.CurrencyCode);
                }

                decimal expensePercentage = budget.Amount > 0 ? (convertedTotalExpenses / budget.Amount) * 100 : 0;

                var resourceManager = ExpenseCategories.ResourceManager;
                var culture = CultureInfo.CurrentUICulture;

                if (expensePercentage >= 50 && expensePercentage < 80)
                {
                    warnings.Add(
                        $"🟡 Предупреждение: Използвали сте {expensePercentage:F2}% от своя {resourceManager.GetString(budget.Category.ToString(), culture)} бюджет.");
                }
                else if (expensePercentage >= 80 && expensePercentage < 100)
                {
                    warnings.Add(
                        $"🟠 Внимание: Изхарчили сте {expensePercentage:F2}% от своя {resourceManager.GetString(budget.Category.ToString(), culture)} budget.");
                }
                else if (expensePercentage >= 100)
                {
                    warnings.Add(
                        $"🔴 Внимание: Надвишили сте своя {resourceManager.GetString(budget.Category.ToString(), culture)} бюджет с {expensePercentage - 100:F2}%!");
                }
            }

            return warnings;
        }
    }
}