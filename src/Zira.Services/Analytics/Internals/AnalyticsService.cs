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

        public AnalyticsService(EntityContext context)
        {
            this.context = context;
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
                .ToListAsync();

            expenses.Sort((a, b) => b.TotalAmount.CompareTo(a.TotalAmount));

            return expenses.Take(top).ToList();
        }

        public Dictionary<Categories, string> GetCostSavingTips(List<CategoryExpenseSummary> expenseCategories)
        {
            var tips = new Dictionary<Categories, string>
            {
                { Categories.Food, "Plan meals ahead and reduce dining out." },
                { Categories.Bills, "Negotiate with service providers or switch to budget-friendly plans." },
                { Categories.Utilities, "Use energy-efficient appliances and turn off unused electronics." },
                { Categories.Transportation, "Consider public transport, carpooling, or fuel-efficient routes." },
                { Categories.Entertainment, "Look for free events or subscription alternatives." },
                { Categories.Clothing, "Shop during sales or buy second-hand." },
                { Categories.Healthcare, "Use generic medications and compare medical service costs." },
            };

            return expenseCategories.ToDictionary(e => e.Category, e => tips.ContainsKey(e.Category) ? tips[e.Category] : "Monitor and budget wisely.");
        }
    }
}
