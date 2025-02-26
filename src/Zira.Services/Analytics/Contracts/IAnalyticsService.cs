using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zira.Data.Enums;
using Zira.Services.Transaction.Models;

namespace Zira.Services.Analytics.Contracts;

public interface IAnalyticsService
{
    Task<List<CategoryExpenseSummary>> GetTopExpenseCategoriesAsync(Guid userId, int top = 5);
    Dictionary<Categories, List<string>> GetCostSavingTips(List<CategoryExpenseSummary> expenseCategories);
}