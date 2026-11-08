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
        // Used when a budget was saved without a warning threshold (0 would warn on every budget).
        private const decimal DefaultWarningThreshold = 80m;

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

        // Budget.Amount is the monthly limit. What has been spent is always calculated from
        // transactions (GetSpentAsync), never stored or subtracted.
        public async Task<bool> AddBudgetAsync(Data.Models.Budget budget, Guid userId)
        {
            budget.UserId = userId;
            budget.Month = new DateTime(budget.Month.Year, budget.Month.Month, 1);

            if (await this.ExistsAsync(userId, budget.Category, budget.Month, excludeBudgetId: null))
            {
                return false;
            }

            if (string.IsNullOrEmpty(budget.CurrencyCode))
            {
                var user = await this.userManager.FindByIdAsync(userId.ToString())
                           ?? throw new InvalidOperationException(AccountText.UserNotFound);

                budget.CurrencyCode = user.PreferredCurrencyCode ?? "BGN";
            }

            budget.Currency = await this.context.Currencies
                                  .FirstOrDefaultAsync(c => c.Code == budget.CurrencyCode)
                              ?? throw new InvalidOperationException(
                                  string.Format(BudgetText.CurrencyNotFound, budget.CurrencyCode));

            budget.BudgetId = this.idGenerationService.GenerateDigitIdAsync();
            budget.SpentPercentage = await this.GetSpentPercentageAsync(budget);

            this.context.Budgets.Add(budget);
            await this.context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateBudgetAsync(Data.Models.Budget budget)
        {
            var existingBudget = await this.context.Budgets
                .FirstOrDefaultAsync(b => b.Id == budget.Id && b.UserId == budget.UserId);

            if (existingBudget == null)
            {
                return false;
            }

            var month = new DateTime(budget.Month.Year, budget.Month.Month, 1);

            // Changing the category or month must not create a second budget for the same slot.
            if (await this.ExistsAsync(budget.UserId, budget.Category, month, excludeBudgetId: budget.Id))
            {
                return false;
            }

            existingBudget.Amount = budget.Amount;
            existingBudget.WarningThreshold = budget.WarningThreshold;
            existingBudget.Category = budget.Category;
            existingBudget.Month = month;
            existingBudget.Remark = budget.Remark;

            await this.context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBudgetAsync(Guid budgetId, Guid userId)
        {
            var budget = await this.context.Budgets
                .FirstOrDefaultAsync(b => b.Id == budgetId && b.UserId == userId);

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
                .AsNoTracking()
                .Where(b => b.UserId == userId)
                .Include(b => b.Currency)
                .OrderByDescending(b => b.Month)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var budget in budgets)
            {
                budget.SpentPercentage = await this.GetSpentPercentageAsync(budget);
            }

            return budgets;
        }

        public async Task<int> GetTotalBudgetsAsync(Guid userId)
        {
            return await this.context.Budgets.CountAsync(b => b.UserId == userId);
        }

        // Warnings only for the current month, using each budget's own warning threshold.
        public async Task<List<string>> GetBudgetWarningsAsync(Guid userId)
        {
            var now = DateTime.UtcNow;

            var budgets = await this.context.Budgets
                .AsNoTracking()
                .Where(b => b.UserId == userId
                            && b.Month.Year == now.Year
                            && b.Month.Month == now.Month)
                .ToListAsync();

            var warnings = new List<string>();

            foreach (var budget in budgets)
            {
                var percent = await this.GetSpentPercentageAsync(budget);
                var threshold = budget.WarningThreshold > 0 ? budget.WarningThreshold : DefaultWarningThreshold;
                var categoryName = GetCategoryName(budget.Category);

                if (percent >= 100)
                {
                    warnings.Add(string.Format(BudgetText.WarningExceeded, categoryName, percent - 100));
                }
                else if (percent >= threshold)
                {
                    warnings.Add(string.Format(BudgetText.WarningThresholdReached, categoryName, percent));
                }
            }

            return warnings;
        }

        private static string GetCategoryName(Categories? category)
        {
            if (category == null)
            {
                return string.Empty;
            }

            return ExpenseCategories.ResourceManager.GetString(category.ToString()!, CultureInfo.CurrentUICulture)
                   ?? category.ToString()!;
        }

        private Task<bool> ExistsAsync(Guid userId, Categories? category, DateTime month, Guid? excludeBudgetId)
        {
            return this.context.Budgets.AnyAsync(
                b => b.UserId == userId
                     && b.Category == category
                     && b.Month.Year == month.Year
                     && b.Month.Month == month.Month
                     && b.Id != excludeBudgetId);
        }

        private async Task<decimal> GetSpentPercentageAsync(Data.Models.Budget budget)
        {
            if (budget.Amount <= 0)
            {
                return 0;
            }

            var spent = await this.GetSpentAsync(budget);
            return Math.Round(spent / budget.Amount * 100, 2);
        }

        // Sum of this month's expenses in the budget's category, converted to the budget's currency.
        private async Task<decimal> GetSpentAsync(Data.Models.Budget budget)
        {
            var expenses = await this.context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == budget.UserId
                            && t.Type == TransactionType.Expense
                            && t.Category == budget.Category
                            && t.Date.Year == budget.Month.Year
                            && t.Date.Month == budget.Month.Month)
                .Select(t => new { t.Amount, t.CurrencyCode })
                .ToListAsync();

            decimal spent = 0;
            foreach (var expense in expenses)
            {
                spent += await this.currencyConverter.ConvertCurrencyAsync(
                    budget.UserId,
                    expense.Amount,
                    expense.CurrencyCode,
                    budget.CurrencyCode ?? expense.CurrencyCode);
            }

            return spent;
        }
    }
}