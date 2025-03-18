using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Data.Models;
using Zira.Services.Currency.Contracts;
using Zira.Services.SavingsGoal.Contracts;

namespace Zira.Services.SavingsGoal.Internals
{
    public class SavingsGoalService : ISavingsGoalService
    {
        private readonly EntityContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICurrencyConverter currencyConverter;

        public SavingsGoalService(
            EntityContext context,
            ICurrencyConverter currencyConverter,
            UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.currencyConverter = currencyConverter;
            this.userManager = userManager;
        }

        public async Task<List<Data.Models.SavingsGoal>> GetUserSavingsGoalsAsync(
            Guid userId,
            int page,
            int pageSize,
            string targetCurrency)
        {
            var goals = await this.context.SavingsGoals
                .Include(s => s.Currency)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var user = await this.userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                foreach (var goal in goals)
                {
                    if (goal.Currency == null)
                    {
                        goal.Currency = user.PreferredCurrency ?? new Data.Models.Currency
                            { Code = "BGN", Symbol = "BGN" };
                    }

                    string goalCurrencyCode = goal.CurrencyCode ?? "BGN";

                    if (!string.IsNullOrEmpty(goalCurrencyCode) && goalCurrencyCode != targetCurrency)
                    {
                        goal.TargetAmount = await this.currencyConverter.ConvertCurrencyAsync(
                            userId,
                            goal.TargetAmount,
                            goalCurrencyCode,
                            targetCurrency);
                        goal.CurrentAmount = await this.currencyConverter.ConvertCurrencyAsync(
                            userId,
                            goal.CurrentAmount,
                            goalCurrencyCode,
                            targetCurrency);
                        goal.CurrencyCode = targetCurrency;
                        goal.Currency = user.PreferredCurrency ?? new Data.Models.Currency
                            { Code = targetCurrency, Symbol = targetCurrency };
                    }
                }
            }

            return goals;
        }

        public async Task<Data.Models.SavingsGoal?> GetSavingsGoalByIdAsync(Guid userId, Guid goalId)
        {
            return await this.context.SavingsGoals
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Id == goalId);
        }

        public async Task<bool> AddSavingsGoalsAsync(Data.Models.SavingsGoal goal, string userPreferredCurrency)
        {
            var existingGoal = await this.context.SavingsGoals
                .FirstOrDefaultAsync(g => g.UserId == goal.UserId && g.Name == goal.Name);

            if (existingGoal != null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(goal.CurrencyCode) && goal.CurrencyCode != userPreferredCurrency)
            {
                goal.TargetAmount = await this.currencyConverter.ConvertCurrencyAsync(
                    goal.UserId,
                    goal.TargetAmount,
                    userPreferredCurrency,
                    goal.CurrencyCode);
                var currency = await this.context.Currencies.FirstOrDefaultAsync(c => c.Code == userPreferredCurrency);

                if (currency == null)
                {
                    currency = new Data.Models.Currency
                    {
                        Code = userPreferredCurrency,
                        Symbol = userPreferredCurrency,
                    };
                }

                goal.Currency = currency;
                goal.CurrencyCode = userPreferredCurrency;

                goal.Currency = currency;
                goal.CurrencyCode = userPreferredCurrency;
            }

            await this.context.SavingsGoals.AddAsync(goal);
            await this.context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateSavingsGoalsAsync(Data.Models.SavingsGoal goal, string userPreferredCurrency)
        {
            var existingGoal = await this.context.SavingsGoals.FindAsync(goal.Id);

            if (existingGoal == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(goal.CurrencyCode) && goal.CurrencyCode != userPreferredCurrency)
            {
                goal.TargetAmount = await this.currencyConverter.ConvertCurrencyAsync(
                    goal.UserId,
                    goal.TargetAmount,
                    userPreferredCurrency,
                    goal.CurrencyCode);
                goal.CurrencyCode = userPreferredCurrency;
            }

            existingGoal.Name = goal.Name;
            existingGoal.TargetAmount = goal.TargetAmount;
            existingGoal.CurrentAmount = goal.CurrentAmount;
            existingGoal.TargetDate = goal.TargetDate;
            existingGoal.CurrencyCode = goal.CurrencyCode;
            existingGoal.Currency = goal.Currency;

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

        public async Task<List<Data.Models.SavingsGoal>> SetAsideForSavingsGoalsAsync(
            Data.Models.Transaction transactionModel)
        {
            if (transactionModel.Type != TransactionType.Income)
            {
                return new List<Data.Models.SavingsGoal>();
            }

            decimal goalAmount = transactionModel.Amount * 0.10m;
            Guid userId = transactionModel.UserId;
            var month = transactionModel.Date.Month;
            var year = transactionModel.Date.Year;

            var savingsGoals = await this.context.SavingsGoals
                .Where(
                    sg => sg.UserId == userId &&
                          sg.TargetDate.Value.Month == month &&
                          sg.TargetDate.Value.Year == year)
                .ToListAsync();

            if (!savingsGoals.Any())
            {
                return new List<Data.Models.SavingsGoal>();
            }

            if (savingsGoals.Count == 1)
            {
                var goal = savingsGoals.First();
                goal.CurrentAmount += goalAmount;

                if (goal.CurrentAmount > goal.TargetAmount)
                {
                    goal.CurrentAmount = goal.TargetAmount;
                }

                transactionModel.Amount -= goalAmount;

                await this.context.SaveChangesAsync();
                return new List<Data.Models.SavingsGoal> { goal };
            }

            return savingsGoals;
        }

        public async Task<List<Data.Models.SavingsGoal>> GetSavingsGoalsAsync(Guid userId, int page, int pageSize)
        {
            var query = this.context.SavingsGoals
                .Include(t => t.Currency)
                .Where(t => t.UserId == userId);

            var savingsGoals = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var user = await this.userManager.FindByIdAsync(userId.ToString());
            if (user != null && !string.IsNullOrEmpty(user.PreferredCurrencyCode))
            {
                foreach (var transaction in savingsGoals)
                {
                    string transactionCurrencyCode = transaction.Currency?.Code ?? "BGN";

                    if (transactionCurrencyCode != user.PreferredCurrencyCode)
                    {
                        transaction.TargetAmount = await this.currencyConverter.ConvertCurrencyAsync(
                            userId,
                            transaction.TargetAmount,
                            transactionCurrencyCode,
                            user.PreferredCurrencyCode);
                        transaction.CurrentAmount = await this.currencyConverter.ConvertCurrencyAsync(
                            userId,
                            transaction.CurrentAmount,
                            transactionCurrencyCode,
                            user.PreferredCurrencyCode);
                    }
                }
            }

            return savingsGoals;
        }

        public async Task<int> GetTotalSavingsGoalsAsync(Guid userId)
        {
            return await this.context.SavingsGoals.CountAsync(b => b.UserId == userId);
        }
    }
}