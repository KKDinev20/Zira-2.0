using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Data.Models;

namespace Zira.Presentation.Tests.Helpers;

public class TestHelpers
{
    private readonly CustomWebApplicationFactory webApplicationFactory;

    public TestHelpers(CustomWebApplicationFactory webApplicationFactory)
    {
        this.webApplicationFactory = webApplicationFactory;
    }

    public async Task<ApplicationUser> CreateUserAsync()
    {
        using var scope = webApplicationFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EntityContext>();

        var user = new ApplicationUser { Id = Guid.NewGuid() };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    public Budget CreateTestBudget(Guid userId)
    {
        return new Budget
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = 1000.00m,
            Category = Categories.Food,
            WarningThreshold = 500.00m,
            Month = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)
        };
    }
    
    public SavingsGoal CreateTestSavingsGoal(Guid userId)
    {
        return new SavingsGoal
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = "Test Goal",
            TargetAmount = 5000.00m,
            CurrentAmount = 1500.00m,
            CreatedAt = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
        };
    }
}