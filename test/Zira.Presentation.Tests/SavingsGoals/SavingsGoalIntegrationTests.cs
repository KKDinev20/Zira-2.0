using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Data.Models;
using Zira.Presentation.Tests.Helpers;
using Zira.Services.SavingsGoal.Contracts;

namespace Zira.Presentation.Tests.SavingsGoals;

public class SavingsGoalIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory webApplicationFactory;
    private readonly TestHelpers testHelpers;

    public SavingsGoalIntegrationTests(CustomWebApplicationFactory webApplicationFactory)
    {
        this.webApplicationFactory = webApplicationFactory;
        testHelpers = new TestHelpers(webApplicationFactory);
    }

    [Fact]
    public async Task AddSavingsGoalAsync_OnValidInput_ShouldAddGoal()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var savingsGoalService = scope.ServiceProvider.GetRequiredService<ISavingsGoalService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();
        var savingsGoal = testHelpers.CreateTestSavingsGoal(user.Id);

        // Act
        var result = await savingsGoalService.AddSavingsGoalsAsync(savingsGoal);

        // Assert
        result.Should().BeTrue();

        var addedGoal = await context.SavingsGoals.FindAsync(savingsGoal.Id);
        addedGoal.Should().NotBeNull();
        addedGoal.Name.Should().Be(savingsGoal.Name);
        addedGoal.TargetAmount.Should().Be(savingsGoal.TargetAmount);
        addedGoal.CurrentAmount.Should().Be(savingsGoal.CurrentAmount);
        addedGoal.TargetDate.Should().Be(savingsGoal.TargetDate);
    }

    [Fact]
    public async Task AddSavingsGoalAsync_OnDuplicateName_ShouldReturnFalse()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var savingsGoalService = scope.ServiceProvider.GetRequiredService<ISavingsGoalService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();
        var savingsGoal = testHelpers.CreateTestSavingsGoal(user.Id);
        await context.SavingsGoals.AddAsync(savingsGoal);
        await context.SaveChangesAsync();

        var duplicateGoal = testHelpers.CreateTestSavingsGoal(user.Id);
        duplicateGoal.Name = savingsGoal.Name;

        // Act
        var result = await savingsGoalService.AddSavingsGoalsAsync(duplicateGoal);

        // Assert
        result.Should().BeFalse();

        var goals = await context.SavingsGoals.Where(g =>
            g.UserId == user.Id && 
            g.Name == duplicateGoal.Name).ToListAsync();
        goals.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateSavingsGoalAsync_OnValidInput_ShouldUpdateGoal()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var savingsGoalService = scope.ServiceProvider.GetRequiredService<ISavingsGoalService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();
        var savingsGoal = testHelpers.CreateTestSavingsGoal(user.Id);
        await context.SavingsGoals.AddAsync(savingsGoal);
        await context.SaveChangesAsync();

        var updatedGoal = new Data.Models.SavingsGoal
        {
            Id = savingsGoal.Id,
            UserId = user.Id,
            Name = "Updated Goal",
            TargetAmount = 7500.00m,
            CurrentAmount = 1500.00m,
            CreatedAt = savingsGoal.CreatedAt,
            TargetDate = DateTime.UtcNow.AddMonths(12)
        };

        // Act
        var result = await savingsGoalService.UpdateSavingsGoalsAsync(updatedGoal);

        // Assert
        result.Should().BeTrue();

        var updatedEntity = await context.SavingsGoals.FindAsync(savingsGoal.Id);
        updatedEntity.Should().NotBeNull();
        updatedEntity.Name.Should().Be(updatedGoal.Name);
        updatedEntity.TargetAmount.Should().Be(updatedGoal.TargetAmount);
        updatedEntity.CurrentAmount.Should().Be(updatedGoal.CurrentAmount);
        updatedEntity.TargetDate.Should().Be(updatedGoal.TargetDate);
    }

    [Fact]
    public async Task DeleteSavingsGoalAsync_OnExistingGoal_ShouldDelete()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var savingsGoalService = scope.ServiceProvider.GetRequiredService<ISavingsGoalService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();
        var savingsGoal = testHelpers.CreateTestSavingsGoal(user.Id);
        await context.SavingsGoals.AddAsync(savingsGoal);
        await context.SaveChangesAsync();

        // Act
        var result = await savingsGoalService.DeleteSavingsGoalsAsync(savingsGoal);

        // Assert
        result.Should().BeTrue();

        var deletedGoal = await context.SavingsGoals.FindAsync(savingsGoal.Id);
        deletedGoal.Should().BeNull();
    }

    [Fact]
    public async Task GetUserSavingsGoalsAsync_ShouldReturnPaginatedGoals()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var savingsGoalService = scope.ServiceProvider.GetRequiredService<ISavingsGoalService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();

        var goals = Enumerable.Range(0, 10)
            .Select(i => testHelpers.CreateTestSavingsGoal(user.Id))
            .ToList();
        
        await context.SavingsGoals.AddRangeAsync(goals);
        await context.SaveChangesAsync();

        // Act
        var result = await savingsGoalService.GetUserSavingsGoalsAsync(user.Id, 1, 5);

        // Assert
        result.Should().HaveCount(5);
        result.Should().BeInDescendingOrder(g => g.CreatedAt);
    }

    [Fact]
    public async Task GetTotalSavingsGoalsAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var savingsGoalService = scope.ServiceProvider.GetRequiredService<ISavingsGoalService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();
        
        var goals = Enumerable.Range(0, 10)
            .Select(i => testHelpers.CreateTestSavingsGoal(user.Id))
            .ToList();
        
        await context.SavingsGoals.AddRangeAsync(goals);
        await context.SaveChangesAsync();

        // Act
        var count = await savingsGoalService.GetTotalSavingsGoalsAsync(user.Id);

        // Assert
        count.Should().Be(10);
    }
    
    [Fact]
    public async Task SetAsideForSavingsGoalsAsync_OnNonIncomeTransaction_ShouldReturnEmptyList()
    {
        // Arrange
        using var scope = webApplicationFactory.Services.CreateScope();
        var savingsGoalService = scope.ServiceProvider.GetRequiredService<ISavingsGoalService>();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = await testHelpers.CreateUserAsync();
        
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Amount = 1000.00m,
            Type = TransactionType.Expense,
            Date = DateTime.UtcNow
        };

        // Act
        var updatedGoals = await savingsGoalService.SetAsideForSavingsGoalsAsync(transaction);

        // Assert
        updatedGoals.Should().BeEmpty();
    }
}