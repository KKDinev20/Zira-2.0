using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Services.Transaction.Internals;

namespace Zira.Services.Tests.PerformanceTests;

public class TransactionPerformanceTests
{
    private readonly TransactionService transactionService;
    private readonly Guid userId;
    private readonly EntityContext dbContext;
    private readonly Random random = new Random();

    public TransactionPerformanceTests()
    {
        dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();

        transactionService = new TransactionService(dbContext, idService, currencyConverter, mockUserManager);
        userId = Guid.NewGuid();

        SeedDatabaseAsync().GetAwaiter().GetResult();
    }

    private async Task SeedDatabaseAsync()
    {
        if (!await dbContext.Transactions.AnyAsync())
        {
            var transactions = Enumerable.Range(1, 10000).Select(i => new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = random.Next(10, 500),
                Type = TransactionType.Income,
                Date = DateTime.UtcNow.AddDays(-i),
                CurrencyCode = "USD",
                Category = Categories.Food,
            }).ToList();

            await dbContext.Transactions.AddRangeAsync(transactions);
            await dbContext.SaveChangesAsync();
        }
    }

    [Benchmark]
    public async Task AddTransactionAsync_Benchmark()
    {
        var transaction = new Data.Models.Transaction
        {
            Amount = 100,
            Type = TransactionType.Expense,
            Date = DateTime.UtcNow,
            CurrencyCode = "USD",
            Category = Categories.Entertainment,
        };

        await transactionService.AddTransactionAsync(transaction, userId);
    }

    [Benchmark]
    public async Task GetTransactionsAsync_Benchmark()
    {
        await transactionService.GetTransactionsAsync(userId, page: 1, pageSize: 50);
    }

    [Benchmark]
    public async Task GetUserTransactionsAsync_Benchmark()
    {
        await transactionService.GetUserTransactionsAsync(userId);
    }
}

/*class Program
{
    public void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<TransactionPerformanceTests>();
    }
}*/