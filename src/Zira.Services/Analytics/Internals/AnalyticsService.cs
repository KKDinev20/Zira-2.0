using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Services.Analytics.Contracts;
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
                    "Plan meals ahead and reduce dining out.",
                    "Buy ingredients in bulk when possible.",
                    "Cook at home instead of ordering takeout.",
                    "Shop during sales periods.",
                    "Use coupons and discount apps for groceries.",
                });

            this.AddTips(
                Categories.Bills,
                new[]
                {
                    "Negotiate with service providers or switch to budget-friendly plans.",
                    "Consider bundling services together.",
                    "Review and adjust subscription services monthly.",
                    "Look for promotional rates and discounts.",
                    "Automate bill payments to avoid late fees.",
                });

            this.AddTips(
                Categories.Utilities,
                new[]
                {
                    "Use energy-efficient appliances and turn off unused electronics.",
                    "Install smart thermostats for temperature management.",
                    "Fix water leaks promptly to prevent waste.",
                    "Upgrade to LED lighting throughout your space.",
                    "Schedule utility audits to identify inefficiencies.",
                });

            this.AddTips(
                Categories.Transportation,
                new[]
                {
                    "Consider public transport, carpooling, or fuel-efficient routes.",
                    "Maintain your vehicle regularly for better mileage.",
                    "Research alternative transportation methods.",
                    "Combine errands to reduce trips.",
                    "Track fuel efficiency and optimize driving habits.",
                });

            this.AddTips(
                Categories.Entertainment,
                new[]
                {
                    "Look for free events or subscription alternatives.",
                    "Create entertainment at home.",
                    "Share streaming services with family members.",
                    "Take advantage of public facilities.",
                    "Plan activities during off-peak hours.",
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

            return expenses;
        }

        public Dictionary<Categories, string> GetCostSavingTips(List<CategoryExpenseSummary> expenseCategories)
        {
            var tips = new Dictionary<Categories, string>();

            foreach (var summary in expenseCategories)
            {
                if (this.tips.ContainsKey(summary.Category))
                {
                    var categoryTips = this.tips[summary.Category];
                    if (categoryTips.Any())
                    {
                        tips[summary.Category] = categoryTips[0];
                    }
                    else
                    {
                        tips[summary.Category] = "Monitor and budget wisely.";
                    }
                }
                else
                {
                    tips[summary.Category] = "Monitor and budget wisely.";
                }
            }

            return tips;
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