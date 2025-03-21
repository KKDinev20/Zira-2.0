using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Presentation.Tests.Helpers;
using Zira.Services.Budget.Contracts;

namespace Zira.Presentation.Tests;

public class BudgetIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory webApplicationFactory;
    private readonly TestHelpers testHelpers;

    public BudgetIntegrationTests(CustomWebApplicationFactory webApplicationFactory)
    {
        this.webApplicationFactory = webApplicationFactory;
        testHelpers = new TestHelpers(webApplicationFactory);
    }

    [Fact]
    public async Task AddBudgetAsync_OnValidInput_ShouldAddBudget()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();
        var budget = testHelpers.CreateTestBudget(user.Id);

        // Act
        var result = await budgetService.AddBudgetAsync(budget, user.Id);

        // Assert
        result.Should().BeTrue();

        var addedBudget = await context.Budgets.FindAsync(budget.Id);
        addedBudget.Should().NotBeNull();
        addedBudget.Amount.Should().Be(budget.Amount);
        addedBudget.Category.Should().Be(budget.Category);
        addedBudget.WarningThreshold.Should().Be(budget.WarningThreshold);
        addedBudget.Month.Should().Be(new DateTime(budget.Month.Year, budget.Month.Month, 1));
    }

    [Fact]
    public async Task AddBudgetAsync_OnDuplicateBudget_ShouldReturnFalse()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();
        var budget = testHelpers.CreateTestBudget(user.Id);
        await context.Budgets.AddAsync(budget);
        await context.SaveChangesAsync();

        // Act
        var result = await budgetService.AddBudgetAsync(budget, user.Id);

        // Assert
        result.Should().BeFalse();

        var budgets = await context.Budgets.Where(b =>
            b.UserId == user.Id &&
            b.Category == budget.Category &&
            b.Month == budget.Month).ToListAsync();
        budgets.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateBudgetAsync_OnValidInput_ShouldUpdateBudget()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();
        var budget = testHelpers.CreateTestBudget(user.Id);

        await context.Budgets.AddAsync(budget);
        await context.SaveChangesAsync();

        var updatedBudget = new Data.Models.Budget
        {
            Id = budget.Id,
            UserId = user.Id,
            Amount = 1500.00m,
            Category = Categories.Food,
            WarningThreshold = 750.00m,
            Month = budget.Month
        };

        // Act
        var result = await budgetService.UpdateBudgetAsync(updatedBudget);

        // Assert
        result.Should().BeTrue();

        var updatedEntity = await context.Budgets.FindAsync(budget.Id);
        updatedEntity.Should().NotBeNull();
        updatedEntity.Amount.Should().Be(1500.00m);
        updatedEntity.WarningThreshold.Should().Be(750.00m);
    }

    [Fact]
    public async Task UpdateBudgetAsync_OnInvalidUser_ShouldReturnFalse()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();
        var budget = testHelpers.CreateTestBudget(user.Id);
        await context.Budgets.AddAsync(budget);
        await context.SaveChangesAsync();

        var updatedBudget = new Data.Models.Budget
        {
            Id = budget.Id,
            UserId = Guid.NewGuid(),
            Amount = 1500.00m,
            Category = Categories.Food,
            WarningThreshold = 750.00m,
            Month = budget.Month
        };

        // Act
        var result = await budgetService.UpdateBudgetAsync(updatedBudget);

        // Assert
        result.Should().BeFalse();

        var unchangedEntity = await context.Budgets.FindAsync(budget.Id);
        unchangedEntity.Should().NotBeNull();
        unchangedEntity.Amount.Should().Be(1000.00m);
    }

    [Fact]
    public async Task DeleteBudgetAsync_OnExistingBudget_ShouldDelete()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();
        var budget = testHelpers.CreateTestBudget(user.Id);
        await context.Budgets.AddAsync(budget);
        await context.SaveChangesAsync();

        // Act
        var result = await budgetService.DeleteBudgetAsync(budget.Id, user.Id);

        // Assert
        result.Should().BeTrue();

        var deletedBudget = await context.Budgets.FindAsync(budget.Id);
        deletedBudget.Should().BeNull();
    }

    [Fact]
    public async Task GetBudgetByIdAsync_OnNonExistingBudget_ShouldReturnNull()
    {
        // Arrange
        var user = await testHelpers.CreateUserAsync();
        using var scope = webApplicationFactory.Services.CreateScope();
        var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetService>();

        // Act
        var result = await budgetService.GetBudgetByIdAsync(Guid.NewGuid(), user.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserBudgetsAsync_ShouldReturnPaginatedBudgets()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();

        var budgets = Enumerable.Range(0, 10)
            .Select(i => testHelpers.CreateTestBudget(user.Id))
            .ToList();

        await context.Budgets.AddRangeAsync(budgets);
        await context.SaveChangesAsync();

        // Act
        var result = await budgetService.GetUserBudgetsAsync(user.Id, 1, 5);

        // Assert
        result.Should().HaveCount(5);
        result.Should().BeInDescendingOrder(b => b.Month);
    }

    [Fact]
    public async Task GetTotalBudgetsAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();

        var budgets = Enumerable.Range(0, 10)
            .Select(i => testHelpers.CreateTestBudget(user.Id))
            .ToList();

        await context.Budgets.AddRangeAsync(budgets);
        await context.SaveChangesAsync();

        // Act
        var count = await budgetService.GetTotalBudgetsAsync(user.Id);

        // Assert
        count.Should().Be(10);
    }

    [Fact]
    public async Task GetBudgetByIdAsync_OnExistingBudget_ShouldReturnBudget()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();
        var budget = testHelpers.CreateTestBudget(user.Id);
        await context.Budgets.AddAsync(budget);
        await context.SaveChangesAsync();

        // Act
        var result = await budgetService.GetBudgetByIdAsync(budget.Id, user.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(budget.Id);
        result.Amount.Should().Be(budget.Amount);
        result.Category.Should().Be(budget.Category);
    }

    [Fact]
    public async Task GetBudgetByIdAsync_OnWrongUserId_ShouldReturnNull()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();
        var budget = testHelpers.CreateTestBudget(user.Id);
        await context.Budgets.AddAsync(budget);
        await context.SaveChangesAsync();

        // Act
        var result = await budgetService.GetBudgetByIdAsync(budget.Id, Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteBudgetAsync_OnNonExistingBudget_ShouldReturnFalse()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetService>();
        var user = await testHelpers.CreateUserAsync();

        // Act
        var result = await budgetService.DeleteBudgetAsync(Guid.NewGuid(), user.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteBudgetAsync_OnWrongUserId_ShouldReturnFalse()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();
        var budget = testHelpers.CreateTestBudget(user.Id);
        await context.Budgets.AddAsync(budget);
        await context.SaveChangesAsync();

        // Act
        var result = await budgetService.DeleteBudgetAsync(budget.Id, Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetUserBudgetsAsync_OnEmptyBudgets_ShouldReturnEmptyList()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetService>();
        var user = await testHelpers.CreateUserAsync();

        // Act
        var result = await budgetService.GetUserBudgetsAsync(user.Id, 1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserBudgetsAsync_ShouldCalculateSpentPercentage()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();
        var budget = testHelpers.CreateTestBudget(user.Id);
        await context.Budgets.AddAsync(budget);

        var transaction = new Data.Models.Transaction
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Amount = 500.00m,
            Category = budget.Category,
            Type = TransactionType.Expense,
            Date = budget.Month
        };
        await context.Transactions.AddAsync(transaction);
        await context.SaveChangesAsync();

        // Act
        var results = await budgetService.GetUserBudgetsAsync(user.Id, 1, 10);
        var result = results.FirstOrDefault();

        // Assert
        result.Should().NotBeNull();
        result.SpentPercentage.Should().Be(50.00m);
    }
    
    [Fact]
    public async Task GetTotalBudgetsAsync_OnEmptyBudgets_ShouldReturnZero()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetService>();
        var user = await testHelpers.CreateUserAsync();

        // Act
        var count = await budgetService.GetTotalBudgetsAsync(user.Id);

        // Assert
        count.Should().Be(0);
    }
    
}