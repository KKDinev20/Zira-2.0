using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zira.Common;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Services.Analytics.Contracts;
using Zira.Services.Analytics.Models;
using Zira.Services.Transaction.Models;

namespace Zira.Services.Analytics.Internals
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly EntityContext context;
        private readonly Dictionary<Categories, List<string>> tips;

        public AnalyticsService(EntityContext context)
        {
            this.context = context;
            this.tips = new Dictionary<Categories, List<string>>();

            foreach (Categories category in Enum.GetValues(typeof(Categories)))
            {
                this.tips[category] = new List<string>();
            }

            this.AddTips(
                Categories.Food,
                new[]
                {
                    @AnalyticsText.FoodTipOne,
                    @AnalyticsText.FoodTipTwo,
                    @AnalyticsText.FoodTipThree,
                    @AnalyticsText.FoodTipFour,
                    @AnalyticsText.FoodTipFive,
                });

            this.AddTips(
                Categories.Bills,
                new[]
                {
                    @AnalyticsText.BillsTipOne,
                    @AnalyticsText.BillsTipTwo,
                    @AnalyticsText.BillsTipThree,
                    @AnalyticsText.BillsTipFour,
                    @AnalyticsText.BillsTipFive,
                });

            this.AddTips(
                Categories.Utilities,
                new[]
                {
                    @AnalyticsText.UtilitiesTipOne,
                    @AnalyticsText.UtilitiesTipTwo,
                    @AnalyticsText.UtilitiesTipThree,
                    @AnalyticsText.UtilitiesTipFour,
                    @AnalyticsText.UtilitiesTipFive,
                });

            this.AddTips(
                Categories.Transportation,
                new[]
                {
                    @AnalyticsText.TransportationTipOne,
                    @AnalyticsText.TransportationTipTwo,
                    @AnalyticsText.TransportationTipThree,
                    @AnalyticsText.TransportationTipFour,
                    @AnalyticsText.TransportationTipFive,
                });

            this.AddTips(
                Categories.Healthcare,
                new[]
                {
                    @AnalyticsText.HealthcareTipOne,
                    @AnalyticsText.HealthcareTipTwo,
                    @AnalyticsText.HealthcareTipThree,
                    @AnalyticsText.HealthcareTipFour,
                    @AnalyticsText.HealthcareTipFive,
                });

            this.AddTips(
                Categories.Insurance,
                new[]
                {
                    @AnalyticsText.InsuranceTipOne,
                    @AnalyticsText.InsuranceTipTwo,
                    @AnalyticsText.InsuranceTipThree,
                    @AnalyticsText.InsuranceTipFour,
                    @AnalyticsText.InsuranceTipFive,
                });

            this.AddTips(
                Categories.Education,
                new[]
                {
                    @AnalyticsText.EducationTipOne,
                    @AnalyticsText.EducationTipTwo,
                    @AnalyticsText.EducationTipThree,
                    @AnalyticsText.EducationTipFour,
                    @AnalyticsText.EducationTipFive,
                });

            this.AddTips(
                Categories.Childcare,
                new[]
                {
                    @AnalyticsText.ChildcareTipOne,
                    @AnalyticsText.ChildcareTipTwo,
                    @AnalyticsText.ChildcareTipThree,
                    @AnalyticsText.ChildcareTipFour,
                    @AnalyticsText.ChildcareTipFive,
                });

            this.AddTips(
                Categories.Entertainment,
                new[]
                {
                    @AnalyticsText.EntertainmentTipOne,
                    @AnalyticsText.EntertainmentTipTwo,
                    @AnalyticsText.EntertainmentTipThree,
                    @AnalyticsText.EntertainmentTipFour,
                    @AnalyticsText.EntertainmentTipFive,
                });

            this.AddTips(
                Categories.Clothing,
                new[]
                {
                    @AnalyticsText.ClothingTipOne,
                    @AnalyticsText.ClothingTipTwo,
                    @AnalyticsText.ClothingTipThree,
                    @AnalyticsText.ClothingTipFour,
                    @AnalyticsText.ClothingTipFive,
                });

            this.AddTips(
                Categories.Savings,
                new[]
                {
                    @AnalyticsText.SavingsTipOne,
                    @AnalyticsText.SavingsTipTwo,
                    @AnalyticsText.SavingsTipThree,
                    @AnalyticsText.SavingsTipFour,
                    @AnalyticsText.SavingsTipFive,
                });

            this.AddTips(
                Categories.HouseholdSupplies,
                new[]
                {
                    @AnalyticsText.HouseholdSuppliesTipOne,
                    @AnalyticsText.HouseholdSuppliesTipTwo,
                    @AnalyticsText.HouseholdSuppliesTipThree,
                    @AnalyticsText.HouseholdSuppliesTipFour,
                    @AnalyticsText.HouseholdSuppliesTipFive,
                });

            this.AddTips(
                Categories.Travel,
                new[]
                {
                    @AnalyticsText.TravelTipOne,
                    @AnalyticsText.TravelTipTwo,
                    @AnalyticsText.TravelTipThree,
                    @AnalyticsText.TravelTipFour,
                    @AnalyticsText.TravelTipFive,
                });

            this.AddTips(
                Categories.Other,
                new[]
                {
                    @AnalyticsText.OtherTipOne,
                    @AnalyticsText.OtherTipTwo,
                    @AnalyticsText.OtherTipThree,
                    @AnalyticsText.OtherTipFour,
                    @AnalyticsText.OtherTipFive,
                });
        }

        public async Task<List<CategoryExpenseSummary>> GetTopExpenseCategoriesAsync(Guid userId, int top = 5)
        {
            var expenses = await this.context.Transactions
                .Where(t => t.UserId == userId && t.Type == TransactionType.Expense && t.Category != null)
                .GroupBy(t => t.Category.Value)
                .Select(
                    g => new CategoryExpenseSummary
                    {
                        Category = g.Key,
                        TotalAmount = g.Sum(t => t.Amount),
                    })
                .OrderByDescending(x => x.TotalAmount)
                .Take(top)
                .ToListAsync();

            expenses.Sort((a, b) => b.TotalAmount.CompareTo(a.TotalAmount));

            if (expenses.Count <= top)
            {
                return expenses;
            }

            decimal kthValue = expenses[top - 1].TotalAmount;

            int low = 0, high = expenses.Count - 1;
            int split = expenses.Count;
            while (low <= high)
            {
                int mid = low + (high - low) / 2;
                if (expenses[mid].TotalAmount < kthValue)
                {
                    split = mid;
                    high = mid - 1;
                }
                else
                {
                    low = mid + 1;
                }
            }

            var topExpenses = expenses.Take(top).ToList();
            return topExpenses;

            return expenses;
        }

        public Dictionary<Categories, List<string>> GetCostSavingTips(List<CategoryExpenseSummary> expenseCategories)
        {
            var tips = new Dictionary<Categories, List<string>>();

            foreach (var summary in expenseCategories)
            {
                if (this.tips.ContainsKey(summary.Category) && this.tips[summary.Category].Any())
                {
                    tips[summary.Category] = this.tips[summary.Category];
                }
                else
                {
                    tips[summary.Category] = new List<string> { "Monitor and budget wisely." };
                }
            }

            return tips;
        }

        public async Task<List<MonthlyExpenseSummary>> GetMonthlyExpensesAsync(Guid userId, int year)
        {
            return await this.context.Transactions
                .Where(t => t.UserId == userId && t.Date.Year == year && t.Type == TransactionType.Expense)
                .GroupBy(t => t.Date.Month)
                .Select(g => new MonthlyExpenseSummary
                {
                    Month = g.Key.ToString(),
                    TotalAmount = g.Sum(t => t.Amount),
                })
                .OrderBy(g => g.Month)
                .ToListAsync();
        }

        private void AddTips(Categories category, string[] tipsToAdd)
        {
            foreach (var tip in tipsToAdd)
            {
                this.tips[category].Add(tip);
            }
        }
    }
}