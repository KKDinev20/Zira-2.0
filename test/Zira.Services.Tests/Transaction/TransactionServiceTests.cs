using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Zira.Data.Enums;
using Zira.Services.Transaction.Internals;

namespace Zira.Services.Tests.Transaction;

public class TransactionServiceTests
{
    [Fact]
    public async Task GetTransactionsAsync_ShouldReturnFilteredTransactions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();

        dbContext.Transactions.AddRange(
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(), UserId = userId, Type = TransactionType.Expense, Category = Categories.Food,
                Amount = 50, Date = DateTime.UtcNow,
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(), UserId = userId, Type = TransactionType.Income, Amount = 100,
                Date = DateTime.UtcNow
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(), UserId = userId, Type = TransactionType.Expense, Category = Categories.Utilities,
                Amount = 30, Date = DateTime.UtcNow
            }
        );
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager);

        // Act
        var result = await service.GetTransactionsAsync(userId, 1, 10, Categories.Food);

        // Assert
        result.Should().HaveCount(1);
        result[0].Category.Should().Be(Categories.Food);
    }


    [Fact]
    public async Task GetTransactionByIdAsync_OnExistingTransaction_ShouldReturnTransaction()
    {
        var userId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();


        dbContext.Transactions.Add(new Data.Models.Transaction
            { Id = transactionId, TransactionId = "20250320091998", UserId = userId, Amount = 200, Date = DateTime.UtcNow });
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager
        );
        var result = await service.GetTransactionByIdAsync(transactionId, userId);

        result.Should().NotBeNull();
        result.Id.Should().Be(transactionId);
    }

    [Fact]
    public async Task GetTransactionByIdAsync_OnNonExistingTransaction_ShouldReturnNull()
    {
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager
        );

        var result = await service.GetTransactionByIdAsync(Guid.NewGuid(), Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddTransactionAsync_ShouldAddTransaction()
    {
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();

        await dbContext.Currencies.AddAsync(new Data.Models.Currency
        {
            Code = "USD",
            Name = "US Dollar",
            Symbol = "$"
        });
        await dbContext.SaveChangesAsync();
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager);
        

        var transaction = new Data.Models.Transaction
        {
            Amount = 100,
            TransactionId = "20250320091998",
            Type = TransactionType.Income,
            Date = DateTime.UtcNow,
            CurrencyCode = "USD",
        };

        await service.AddTransactionAsync(transaction, userId);
        var addedTransaction = await dbContext.Transactions.FirstOrDefaultAsync(t => t.UserId == userId);

        addedTransaction.Should().NotBeNull();
        addedTransaction.Amount.Should().Be(100);
        addedTransaction.CurrencyCode.Should().Be("USD"); 
    }
    
    [Fact]
    public async Task AddExpenseTransaction_ShouldReduceBudget()
    {
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();

        await dbContext.Currencies.AddAsync(new Data.Models.Currency
        {
            Code = "USD",
            Name = "US Dollar",
            Symbol = "$"
        });
        await dbContext.SaveChangesAsync();

        var category = Categories.Food;
        var budget = new Data.Models.Budget
        {
            UserId = userId,
            Amount = 200,
            Category = category,
            Month = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
            CurrencyCode = "USD"
        };
        await dbContext.Budgets.AddAsync(budget);
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager);

        var expense = new Data.Models.Transaction
        {
            Amount = 50,
            Type = TransactionType.Expense,
            Category = category,
            Date = DateTime.UtcNow,
            CurrencyCode = "USD",
        };

        await service.AddTransactionAsync(expense, userId);

        var updatedBudget = await dbContext.Budgets.FirstOrDefaultAsync(b => b.UserId == userId && b.Category == category);
        updatedBudget.Should().NotBeNull();
        updatedBudget.Amount.Should().Be(150);
    }

    
    [Fact]
    public async Task AddExpenseTransaction_ShouldFailIfOverBudget()
    {
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();

        await dbContext.Currencies.AddAsync(new Data.Models.Currency
        {
            Code = "USD",
            Name = "US Dollar",
            Symbol = "$"
        });
        await dbContext.SaveChangesAsync();

        var category = Categories.Entertainment;
        var budget = new Data.Models.Budget
        {
            UserId = userId,
            Amount = 30, 
            Category = category,
            Month = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
            CurrencyCode = "USD"
        };
        await dbContext.Budgets.AddAsync(budget);
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager);

        var expensiveTransaction = new Data.Models.Transaction
        {
            Amount = 100, 
            Type = TransactionType.Expense,
            Category = category,
            Date = DateTime.UtcNow,
            CurrencyCode = "USD",
        };

        Func<Task> action = async () => await service.AddTransactionAsync(expensiveTransaction, userId);
        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Expense exceeds budget!");
    }



    [Fact]
    public async Task DeleteTransactionAsync_ShouldRemoveTransaction()
    {
        var userId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();


        dbContext.Transactions.Add(new Data.Models.Transaction { Id = transactionId, UserId = userId, Amount = 50 });
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager
        );
        await service.DeleteTransactionAsync(transactionId, userId);

        var deletedTransaction = await dbContext.Transactions.FindAsync(transactionId);
        deletedTransaction.Should().BeNull();
    }

    [Fact]
    public async Task UpdateTransactionAsync_ShouldModifyTransaction()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();


        dbContext.Transactions.Add(new Data.Models.Transaction
            { Id = transactionId, UserId = userId, Amount = 50, Remark = "Old Remark" });
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager
        );
        var updatedTransaction = new Data.Models.Transaction
            { Id = transactionId,             TransactionId = "20250320091998", UserId = userId, Amount = 100, Remark = "New Remark" };

        await service.UpdateTransactionAsync(updatedTransaction);

        var modifiedTransaction = await dbContext.Transactions.FindAsync(transactionId);
        modifiedTransaction.Should().NotBeNull();
        modifiedTransaction.Amount.Should().Be(100);
        modifiedTransaction.Remark.Should().Be("New Remark");
    }
}