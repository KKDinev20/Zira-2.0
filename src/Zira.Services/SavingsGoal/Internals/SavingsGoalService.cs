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
                        goal.Currency = await this.context.Currencies
                                            .FirstOrDefaultAsync(c => c.Code == goal.CurrencyCode)
                                        ?? new Data.Models.Currency { Code = "BGN", Symbol = "BGN" }; 
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
                
                goal.CurrencyCode = goal.CurrencyCode;

                goal.Currency = await this.context.Currencies
                    .FirstOrDefaultAsync(c => c.Code == goal.CurrencyCode);

                if (goal.Currency == null)
                {
                    throw new InvalidOperationException("Currency not found!");
                }

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
    
            existingGoal.Name = goal.Name;
            existingGoal.TargetAmount = goal.TargetAmount;
            if (goal.CurrentAmount >= 0)
            {
                existingGoal.CurrentAmount = goal.CurrentAmount; 
            }
            existingGoal.TargetDate = goal.TargetDate;
            existingGoal.Remark = goal.Remark;
            existingGoal.CurrencyCode = goal.CurrencyCode;
            existingGoal.Currency = await this.context.Currencies
                .FirstOrDefaultAsync(c => c.Code == goal.CurrencyCode);

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
        
        // Read-only: which goals can receive money from this income
        public Task<List<Data.Models.SavingsGoal>> GetEligibleGoalsAsync(Data.Models.Transaction income)
        {
            return this.context.SavingsGoals
                .Where(g => g.UserId == income.UserId
                            && g.CurrentAmount < g.TargetAmount
                            && (g.TargetDate == null || g.TargetDate >= income.Date))
                .ToListAsync();
        }

// The only method that moves money; runs once per income
        public async Task<SetAsideResult> SetAsideAsync(Guid userId, Guid transactionId, Guid goalId)
        {
            var income = await this.context.Transactions.FirstOrDefaultAsync(t =>
                             t.Id == transactionId && t.UserId == userId && t.Type == TransactionType.Income)
                         ?? throw new KeyNotFoundException("Transaction not found");

            if (income.IsSetAside)
            {
                throw new InvalidOperationException("Savings were already set aside from this income.");
            }

            var goal = await this.context.SavingsGoals.FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId)
                       ?? throw new KeyNotFoundException("Savings goal not found");

            var amount = await this.currencyConverter.ConvertCurrencyAsync(
                userId, income.Amount * 0.10m, income.CurrencyCode, goal.CurrencyCode ?? income.CurrencyCode);

            goal.CurrentAmount = Math.Min(goal.CurrentAmount + amount, goal.TargetAmount);
            income.IsSetAside = true;

            await this.context.SaveChangesAsync();
            return new SetAsideResult(amount, goal.CurrencyCode, goal.Name);
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