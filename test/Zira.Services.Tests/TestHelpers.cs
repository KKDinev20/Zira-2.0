using System;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Data.Models;
using Zira.Services.Currency.Contracts;
using Zira.Services.Common.Contracts;

namespace Zira.Services.Tests;

public static class TestHelpers
{
    public static EntityContext CreateDbContext()
    {
        var dbContextOptions = new DbContextOptionsBuilder<EntityContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());
        
        return new EntityContext(dbContextOptions.Options);
    }
    
    public static IIdGenerationService CreateIdGenerationService() => Mock.Of<IIdGenerationService>();
    public static ICurrencyConverter CreateCurrencyConverterService() => Mock.Of<ICurrencyConverter>();

    public static Mock<UserManager<ApplicationUser>> CreateMockUserManager()
    {
        return new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
    }
}