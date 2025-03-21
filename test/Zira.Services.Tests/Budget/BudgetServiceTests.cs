using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Zira.Data.Enums;
using Zira.Services.Budget.Internals;

namespace Zira.Services.Tests.Budget;

public class BudgetServiceTests
{
    [Fact]
    public async Task GetTotalBudgetsAsync_OnValidUser_ShouldReturnsBudgetOfUser()
    {
        // Arrange
        var dbContext = TestHelpers.CreateDbContext();
        var userId = Guid.NewGuid();
        var idGenerationService = TestHelpers.CreateIdGenerationService();
        var currencyService = TestHelpers.CreateCurrencyConverterService();
        var userManager = TestHelpers.CreateMockUserManager();
        var expectedAmount = 1000.00m;

        var budget = new Data.Models.Budget
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = expectedAmount,
            Category = Categories.Food,
            WarningThreshold = 500.00m,
            Month = DateTime.Now
        };

        dbContext.Budgets.Add(budget);
        await dbContext.SaveChangesAsync();

        var budgetService = new BudgetService(dbContext, idGenerationService, currencyService, userManager);

        // Act
        var result = await budgetService.GetTotalBudgetsAsync(userId);

        // Assert
        result.Should().Be(1);
        await dbContext.DisposeAsync();
    }

    [Fact]
    public async Task GetTotalBudgetsAsync_OnNonExistingUser_ShouldReturnZero()
    {
        // Arrange
        var dbContext = TestHelpers.CreateDbContext();
        var nonExistingUserId = Guid.NewGuid();
        var idGenerationService = TestHelpers.CreateIdGenerationService();
        var currencyService = TestHelpers.CreateCurrencyConverterService();
        var userManager = TestHelpers.CreateMockUserManager();

        var existingBudget = new Data.Models.Budget
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Amount = 1000.00m,
            Category = Categories.Food,
            WarningThreshold = 500.00m,
            Month = DateTime.Now
        };

        dbContext.Budgets.Add(existingBudget);
        await dbContext.SaveChangesAsync();

        var budgetService = new BudgetService(dbContext, idGenerationService, currencyService, userManager);

        // Act
        var result = await budgetService.GetTotalBudgetsAsync(nonExistingUserId);

        // Assert
        result.Should().Be(0);
        await dbContext.DisposeAsync();
    }

    [Fact]
    public async Task GetTotalBudgetsAsync_WithMultipleBudgets_ShouldReturnTotalCount()
    {
        // Arrange
        var dbContext = TestHelpers.CreateDbContext();
        var userId = Guid.NewGuid();
        var idGenerationService = TestHelpers.CreateIdGenerationService();
        var currencyService = TestHelpers.CreateCurrencyConverterService();
        var userManager = TestHelpers.CreateMockUserManager();
        var budgets = new List<Data.Models.Budget>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = 1000.00m,
                Category = Categories.Food,
                WarningThreshold = 500.00m,
                Month = DateTime.Now
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = 2000.00m,
                Category = Categories.Transportation,
                WarningThreshold = 1000.00m,
                Month = DateTime.Now
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = 1500.00m,
                Category = Categories.Utilities,
                WarningThreshold = 750.00m,
                Month = DateTime.Now
            }
        };

        await dbContext.Budgets.AddRangeAsync(budgets);
        await dbContext.SaveChangesAsync();

        var budgetService = new BudgetService(dbContext, idGenerationService, currencyService, userManager);

        // Act
        var result = await budgetService.GetTotalBudgetsAsync(userId);

        // Assert
        result.Should().Be(budgets.Count);
        await dbContext.DisposeAsync();
    }

    [Fact]
    public async Task GetUserBudgetsAsync_OnValidUser_ShouldReturnPagedBudgets()
    {
        // Arrange
        var dbContext = TestHelpers.CreateDbContext();
        var idGenerationService = TestHelpers.CreateIdGenerationService();
        var currencyService = TestHelpers.CreateCurrencyConverterService();
        var userManager = TestHelpers.CreateMockUserManager();
        var userId = Guid.NewGuid();

        var budgets = new List<Data.Models.Budget>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = 1000.00m,
                Category = Categories.Food,
                WarningThreshold = 500.00m,
                Month = new DateTime(2025, 1, 1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = 2000.00m,
                Category = Categories.Transportation,
                WarningThreshold = 1000.00m,
                Month = new DateTime(2025, 2, 1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = 1500.00m,
                Category = Categories.Utilities,
                WarningThreshold = 750.00m,
                Month = new DateTime(2025, 3, 1)
            }
        };

        await dbContext.Budgets.AddRangeAsync(budgets);
        await dbContext.SaveChangesAsync();

        var budgetService = new BudgetService(dbContext, idGenerationService, currencyService, userManager);
        var page = 1;
        var pageSize = 2;

        // Act
        var result = await budgetService.GetUserBudgetsAsync(userId, page, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(pageSize);
        result.Should().BeInDescendingOrder(b => b.Month);
        result.First().Month.Should().Be(new DateTime(2025, 3, 1));
        result.Last().Month.Should().Be(new DateTime(2025, 2, 1));
        await dbContext.DisposeAsync();
    }

    [Fact]
    public async Task GetUserBudgetsAsync_OnNonExistingUser_ShouldReturnEmptyList()
    {
        //Arrange
        var dbContext = TestHelpers.CreateDbContext();
        var userId = Guid.NewGuid();
        var idGenerationService = TestHelpers.CreateIdGenerationService();
        var currencyService = TestHelpers.CreateCurrencyConverterService();
        var userManager = TestHelpers.CreateMockUserManager();
        var existingBudget = new Data.Models.Budget
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Amount = 1000.00m,
            Category = Categories.Food,
            WarningThreshold = 500.00m,
            Month = DateTime.Now
        };

        dbContext.Budgets.Add(existingBudget);
        await dbContext.SaveChangesAsync();

        var budgetService = new BudgetService(dbContext, idGenerationService, currencyService, userManager);
        var page = 1;
        var pageSize = 10;

        // Act
        var result = await budgetService.GetUserBudgetsAsync(userId, page, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        await dbContext.DisposeAsync();
    }

    [Fact]
    public async Task GetBudgetByIdAsync_OnValidBudgetIdAndUserId_ShouldReturnBudget()
    {
        // Arrange
        var dbContext = TestHelpers.CreateDbContext();
        var userId = Guid.NewGuid();
        var idGenerationService = TestHelpers.CreateIdGenerationService();
        var currencyService = TestHelpers.CreateCurrencyConverterService();
        var userManager = TestHelpers.CreateMockUserManager();
        var budget = new Data.Models.Budget
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = 1000.00m,
            Category = Categories.Food,
            WarningThreshold = 500.00m,
            Month = new DateTime(2025, 1, 1)
        };

        dbContext.Budgets.Add(budget);
        await dbContext.SaveChangesAsync();

        var budgetService = new BudgetService(dbContext, idGenerationService, currencyService, userManager);

        // Act
        var result = await budgetService.GetBudgetByIdAsync(budget.Id, userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(budget);
        await dbContext.DisposeAsync();
    }

    [Fact]
    public async Task GetBudgetByIdAsync_OnInvalidBudgetId_ShouldReturnNull()
    {
        // Arrange
        var dbContext = TestHelpers.CreateDbContext();
        var userId = Guid.NewGuid();
        var idGenerationService = TestHelpers.CreateIdGenerationService();
        var currencyService = TestHelpers.CreateCurrencyConverterService();
        var userManager = TestHelpers.CreateMockUserManager();
        var budgetId = Guid.NewGuid();

        var budget = new Data.Models.Budget
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = 1000.00m,
            Category = Categories.Food,
            WarningThreshold = 500.00m,
            Month = new DateTime(2025, 1, 1)
        };

        dbContext.Budgets.Add(budget);
        await dbContext.SaveChangesAsync();

        var budgetService = new BudgetService(dbContext, idGenerationService, currencyService, userManager);

        // Act
        var result = await budgetService.GetBudgetByIdAsync(budgetId, userId);

        // Assert
        result.Should().BeNull();
        await dbContext.DisposeAsync();
    }
    
    [Fact]
    public async Task AddBudgetAsync_OnExistingBudget_ShouldReturnFalse()
    {
        // Arrange
        var dbContext = TestHelpers.CreateDbContext();
        var userId = Guid.NewGuid();
        var service = TestHelpers.CreateIdGenerationService();
        var idGenerationService = TestHelpers.CreateIdGenerationService();
        var currencyService = TestHelpers.CreateCurrencyConverterService();
        var userManager = TestHelpers.CreateMockUserManager();
        var budget = new Data.Models.Budget
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = 1500.00m,
            Category = Categories.Food,
            WarningThreshold = 750.00m,
            Month = new DateTime(2025, 4, 1)
        };

        await dbContext.Budgets.AddAsync(budget);
        await dbContext.SaveChangesAsync();

        var budgetService = new BudgetService(dbContext, idGenerationService, currencyService, userManager);

        // Act
        var result = await budgetService.AddBudgetAsync(budget, userId);

        // Assert
        result.Should().BeFalse();
        await dbContext.DisposeAsync();
    }
    
    [Fact]
    public async Task DeleteBudgetAsync_OnValidBudget_ShouldReturnTrue()
    {
        // Arrange
        var dbContext = TestHelpers.CreateDbContext();
        var userId = Guid.NewGuid();
        var idGenerationService = TestHelpers.CreateIdGenerationService();
        var currencyService = TestHelpers.CreateCurrencyConverterService();
        var userManager = TestHelpers.CreateMockUserManager();
        var budget = new Data.Models.Budget
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = 1500.00m,
            Category = Categories.Food,
            WarningThreshold = 750.00m,
            Month = new DateTime(2025, 4, 1)
        };

        await dbContext.Budgets.AddAsync(budget);
        await dbContext.SaveChangesAsync();

        var budgetService = new BudgetService(dbContext, idGenerationService, currencyService, userManager);

        // Act
        var result = await budgetService.DeleteBudgetAsync(budget.Id, userId);

        // Assert
        result.Should().BeTrue();
        dbContext.Budgets.Should().NotContain(budget);
        await dbContext.DisposeAsync();
    }

    [Fact]
    public async Task DeleteBudgetAsync_OnInvalidBudget_ShouldReturnFalse()
    {
        // Arrange
        var dbContext = TestHelpers.CreateDbContext();
        var userId = Guid.NewGuid();
        var budgetId = Guid.NewGuid();
        var idGenerationService = TestHelpers.CreateIdGenerationService();
        var currencyService = TestHelpers.CreateCurrencyConverterService();
        var userManager = TestHelpers.CreateMockUserManager();

        var budgetService = new BudgetService(dbContext, idGenerationService, currencyService, userManager);

        // Act
        var result = await budgetService.DeleteBudgetAsync(budgetId, userId);

        // Assert
        result.Should().BeFalse();
        await dbContext.DisposeAsync();
    }
}