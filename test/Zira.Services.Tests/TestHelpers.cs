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
    
    public static IIdGenerationService CreateIdGenerationService()
    {
        var idServiceMock = new Mock<IIdGenerationService>();
        idServiceMock.Setup(x => x.GenerateDigitIdAsync())
            .Returns("20250321123456");
        return idServiceMock.Object;
    }
    
    public static ICurrencyConverter CreateCurrencyConverterService()
    {
        var converterMock = new Mock<ICurrencyConverter>();
        converterMock.Setup(x => x.ConvertCurrencyAsync(
                It.IsAny<Guid>(), 
                It.IsAny<decimal>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()))
            .ReturnsAsync((Guid _, decimal amount, string _, string _) => amount);
        return converterMock.Object;
    }

    public static UserManager<ApplicationUser> CreateMockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var mockUserManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string userId) => new ApplicationUser
            {
                Id = Guid.Parse(userId),
                PreferredCurrencyCode = "USD",
            });

        return mockUserManager.Object;
    }
}