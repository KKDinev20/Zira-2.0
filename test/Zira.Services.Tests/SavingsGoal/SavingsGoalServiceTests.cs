using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Zira.Services.SavingsGoal.Internals;

namespace Zira.Services.Tests.SavingsGoal;

public class SavingsGoalServiceTests
{
    [Fact]
    public async Task GetUserSavingsGoalsAsync_ShouldReturnPagedSavingsGoals()
    {
        var context = TestHelpers.CreateDbContext();
        var userId = Guid.NewGuid();

        context.SavingsGoals.AddRange(
            new Data.Models.SavingsGoal
                { Id = Guid.NewGuid(), UserId = userId, Name = "Goal 1", CreatedAt = DateTime.UtcNow },
            new Data.Models.SavingsGoal
                { Id = Guid.NewGuid(), UserId = userId, Name = "Goal 2", CreatedAt = DateTime.UtcNow.AddDays(-1) }
        );

        await context.SaveChangesAsync();

        var service = new SavingsGoalService(context);
        var result = await service.GetUserSavingsGoalsAsync(userId, page: 1, pageSize: 1);

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Goal 1");
    }

    [Fact]
    public async Task GetSavingsGoalByIdAsync_ShouldReturnGoal_WhenExists()
    {
        var context = TestHelpers.CreateDbContext();
        var userId = Guid.NewGuid();
        var goal = new Data.Models.SavingsGoal
            { Id = Guid.NewGuid(), UserId = userId, Name = "Goal Test", CreatedAt = DateTime.UtcNow };

        context.SavingsGoals.Add(goal);
        await context.SaveChangesAsync();

        var service = new SavingsGoalService(context);
        var result = await service.GetSavingsGoalByIdAsync(userId, goal.Id);

        result.Should().NotBeNull();
        result?.Name.Should().Be("Goal Test");
    }

    [Fact]
    public async Task GetSavingsGoalByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var context = TestHelpers.CreateDbContext();
        var service = new SavingsGoalService(context);

        var result = await service.GetSavingsGoalByIdAsync(Guid.NewGuid(), Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddSavingsGoalsAsync_ShouldAddNewGoal()
    {
        var context = TestHelpers.CreateDbContext();
        var service = new SavingsGoalService(context);

        var goal = new Data.Models.SavingsGoal
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Name = "New Goal",
            CreatedAt = DateTime.UtcNow
        };

        var result = await service.AddSavingsGoalsAsync(goal);
        result.Should().BeTrue();

        var savedGoal = await context.SavingsGoals.FirstOrDefaultAsync(g => g.Id == goal.Id);
        savedGoal.Should().NotBeNull();
        savedGoal?.Name.Should().Be("New Goal");
    }

    [Fact]
    public async Task AddSavingsGoalsAsync_ShouldNotAddDuplicateGoal()
    {
        var context = TestHelpers.CreateDbContext();
        var userId = Guid.NewGuid();
        var goalName = "Duplicate Goal";

        context.SavingsGoals.Add(new Data.Models.SavingsGoal
            { Id = Guid.NewGuid(), UserId = userId, Name = goalName, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var service = new SavingsGoalService(context);
        var newGoal = new Data.Models.SavingsGoal
            { Id = Guid.NewGuid(), UserId = userId, Name = goalName, CreatedAt = DateTime.UtcNow };

        var result = await service.AddSavingsGoalsAsync(newGoal);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateSavingsGoalsAsync_ShouldUpdateGoal_WhenExists()
    {
        var context = TestHelpers.CreateDbContext();
        var goal = new Data.Models.SavingsGoal
        {
            Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Name = "Old Goal", TargetAmount = 1000,
            CreatedAt = DateTime.UtcNow
        };

        context.SavingsGoals.Add(goal);
        await context.SaveChangesAsync();

        var service = new SavingsGoalService(context);
        goal.Name = "Updated Goal";
        goal.TargetAmount = 2000;

        var result = await service.UpdateSavingsGoalsAsync(goal);
        result.Should().BeTrue();

        var updatedGoal = await context.SavingsGoals.FindAsync(goal.Id);
        updatedGoal.Should().NotBeNull();
        updatedGoal?.Name.Should().Be("Updated Goal");
        updatedGoal?.TargetAmount.Should().Be(2000);
    }

    [Fact]
    public async Task UpdateSavingsGoalsAsync_ShouldReturnFalse_WhenGoalDoesNotExist()
    {
        var context = TestHelpers.CreateDbContext();
        var service = new SavingsGoalService(context);

        var goal = new Data.Models.SavingsGoal
            { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Name = "Non-existing Goal", TargetAmount = 5000 };

        var result = await service.UpdateSavingsGoalsAsync(goal);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteSavingsGoalsAsync_ShouldRemoveGoal_WhenExists()
    {
        var context = TestHelpers.CreateDbContext();
        var goal = new Data.Models.SavingsGoal
            { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Name = "Goal to Delete", CreatedAt = DateTime.UtcNow };

        context.SavingsGoals.Add(goal);
        await context.SaveChangesAsync();

        var service = new SavingsGoalService(context);
        var result = await service.DeleteSavingsGoalsAsync(goal);

        result.Should().BeTrue();

        var deletedGoal = await context.SavingsGoals.FindAsync(goal.Id);
        deletedGoal.Should().BeNull();
    }

    [Fact]
    public async Task DeleteSavingsGoalsAsync_ShouldReturnFalse_WhenGoalDoesNotExist()
    {
        var context = TestHelpers.CreateDbContext();
        var service = new SavingsGoalService(context);

        var goal = new Data.Models.SavingsGoal
            { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Name = "Non-existent Goal" };

        var result = await service.DeleteSavingsGoalsAsync(goal);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetTotalSavingsGoalsAsync_ShouldReturnCorrectCount()
    {
        var context = TestHelpers.CreateDbContext();
        var userId = Guid.NewGuid();

        context.SavingsGoals.AddRange(
            new Data.Models.SavingsGoal
                { Id = Guid.NewGuid(), UserId = userId, Name = "Goal 1", CreatedAt = DateTime.UtcNow },
            new Data.Models.SavingsGoal
                { Id = Guid.NewGuid(), UserId = userId, Name = "Goal 2", CreatedAt = DateTime.UtcNow }
        );

        await context.SaveChangesAsync();

        var service = new SavingsGoalService(context);
        var result = await service.GetTotalSavingsGoalsAsync(userId);

        result.Should().Be(2);
    }
}