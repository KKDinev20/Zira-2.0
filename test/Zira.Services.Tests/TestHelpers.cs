using System;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Services.Common.Contracts;
using Zira.Services.Common.Internals;

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
        return new IdGenerationService(CreateDbContext());
    }

}