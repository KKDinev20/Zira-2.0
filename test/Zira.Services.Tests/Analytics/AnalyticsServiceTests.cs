using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Zira.Data.Enums;
using Zira.Services.Analytics.Internals;
using Zira.Services.Transaction.Models;

namespace Zira.Services.Tests.Analytics
{
    public class AnalyticsServiceTests
    {
        
        [Fact]
        public async Task GetTopExpenseCategoriesAsync_WhenNoTransactions_ShouldReturnEmptyList()
        {
            // Arrange
            var dbContext = TestHelpers.CreateDbContext();
            var userId = Guid.NewGuid();
            var currencyService = TestHelpers.CreateCurrencyConverterService();
            var userManager = TestHelpers.CreateMockUserManager();
            var analyticsService = new AnalyticsService(dbContext, currencyService, userManager);

            // Act
            var result = await analyticsService.GetTopExpenseCategoriesAsync(userId, 5);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetCostSavingTips_ShouldReturnTipsForEachCategory()
        {
            // Arrange
            var dbContext = TestHelpers.CreateDbContext();
            var currencyService = TestHelpers.CreateCurrencyConverterService();
            var userManager = TestHelpers.CreateMockUserManager();
            var analyticsService = new AnalyticsService(dbContext, currencyService, userManager);

            var expenseSummaries = new List<CategoryExpenseSummary>
            {
                new() { Category = Categories.Food, TotalAmount = 1000 },
                new() { Category = Categories.Transportation, TotalAmount = 500 }
            };

            // Act
            var result = analyticsService.GetCostSavingTips(expenseSummaries);

            // Assert
            result.Should().ContainKey(Categories.Food);
            result.Should().ContainKey(Categories.Transportation);
            result[Categories.Food].Should().NotBeEmpty();
            result[Categories.Transportation].Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetMonthlyExpensesAsync_WhenExpensesExist_ShouldReturnCorrectSummaries()
        {
            // Arrange
            var dbContext = TestHelpers.CreateDbContext();
            var userId = Guid.NewGuid();
            var currencyService = TestHelpers.CreateCurrencyConverterService();
            var userManager = TestHelpers.CreateMockUserManager();

            dbContext.Transactions.AddRange(
                new Data.Models.Transaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = TransactionType.Expense,
                    Amount = 100,
                    Date = new DateTime(2024, 1, 10)
                },
                new Data.Models.Transaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = TransactionType.Expense,
                    Amount = 200,
                    Date = new DateTime(2024, 1, 15)
                },
                new Data.Models.Transaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = TransactionType.Expense,
                    Amount = 300,
                    Date = new DateTime(2024, 2, 20)
                }
            );

            await dbContext.SaveChangesAsync();
            var analyticsService = new AnalyticsService(dbContext, currencyService, userManager);

            // Act
            var result = await analyticsService.GetMonthlyExpensesAsync(userId, 2024);

            // Assert
            result.Should().HaveCount(2);
            result.First().Month.Should().Be("1");
            result.First().TotalAmount.Should().Be(300);
            result.Last().Month.Should().Be("2");
            result.Last().TotalAmount.Should().Be(300);
        }

        [Fact]
        public async Task GetMonthlyExpensesAsync_WhenNoExpenses_ShouldReturnEmptyList()
        {
            // Arrange
            var dbContext = TestHelpers.CreateDbContext();
            var userId = Guid.NewGuid();
            var currencyService = TestHelpers.CreateCurrencyConverterService();
            var userManager = TestHelpers.CreateMockUserManager();
            var analyticsService = new AnalyticsService(dbContext, currencyService, userManager);

            // Act
            var result = await analyticsService.GetMonthlyExpensesAsync(userId, 2024);

            // Assert
            result.Should().BeEmpty();
        }
    }
}