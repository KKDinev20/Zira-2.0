using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Zira.Data.Enums;
using Zira.Data.Models;
using Zira.Services.Transaction.Internals;

namespace Zira.Services.Tests.Transaction;

public class TransactionServiceTests
{
    [Fact]
    public async Task GetTransactionsAsync_ShouldReturnFilteredTransactions()
    {
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();

        dbContext.Transactions.AddRange(
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(), UserId = userId, Type = TransactionType.Expense, Category = Categories.Food,
                Amount = 50, Date = DateTime.UtcNow
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

        var service = new TransactionService(dbContext);

        var result = await service.GetTransactionsAsync(userId, 1, 10, Categories.Food);
        result.Should().HaveCount(1);
        result[0].Category.Should().Be(Categories.Food);
    }

    [Fact]
    public async Task GetTransactionByIdAsync_OnExistingTransaction_ShouldReturnTransaction()
    {
        var userId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();

        dbContext.Transactions.Add(new Data.Models.Transaction
            { Id = transactionId, UserId = userId, Amount = 200, Date = DateTime.UtcNow });
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext);
        var result = await service.GetTransactionByIdAsync(transactionId, userId);

        result.Should().NotBeNull();
        result.Id.Should().Be(transactionId);
    }

    [Fact]
    public async Task GetTransactionByIdAsync_OnNonExistingTransaction_ShouldReturnNull()
    {
        var dbContext = TestHelpers.CreateDbContext();
        var service = new TransactionService(dbContext);

        var result = await service.GetTransactionByIdAsync(Guid.NewGuid(), Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddTransactionAsync_ShouldAddTransaction()
    {
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();

        var service = new TransactionService(dbContext);
        var transaction = new Data.Models.Transaction
            { Amount = 100, Type = TransactionType.Income, Date = DateTime.UtcNow };

        await service.AddTransactionAsync(transaction, userId);
        var addedTransaction = await dbContext.Transactions.FirstOrDefaultAsync(t => t.UserId == userId);

        addedTransaction.Should().NotBeNull();
        addedTransaction.Amount.Should().Be(100);
    }

    [Fact]
    public async Task DeleteTransactionAsync_ShouldRemoveTransaction()
    {
        var userId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();

        dbContext.Transactions.Add(new Data.Models.Transaction { Id = transactionId, UserId = userId, Amount = 50 });
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext);
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

        dbContext.Transactions.Add(new Data.Models.Transaction
            { Id = transactionId, UserId = userId, Amount = 50, Description = "Old Description" });
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext);
        var updatedTransaction = new Data.Models.Transaction
            { Id = transactionId, UserId = userId, Amount = 100, Description = "New Description" };

        await service.UpdateTransactionAsync(updatedTransaction);

        var modifiedTransaction = await dbContext.Transactions.FindAsync(transactionId);
        modifiedTransaction.Should().NotBeNull();
        modifiedTransaction.Amount.Should().Be(100);
        modifiedTransaction.Description.Should().Be("New Description");
    }

    [Fact]
    public async Task GetCurrentMonthIncomeAsync_ShouldReturnCorrectIncome()
    {
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dbContext = TestHelpers.CreateDbContext();

        dbContext.Transactions.AddRange(
            new Data.Models.Transaction
                { Id = Guid.NewGuid(), UserId = userId, Type = TransactionType.Income, Amount = 500, Date = now },
            new Data.Models.Transaction
                { Id = Guid.NewGuid(), UserId = userId, Type = TransactionType.Income, Amount = 200, Date = now }
        );

        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext);
        var income = await service.GetCurrentMonthIncomeAsync(userId);

        income.Should().Be(700);
    }

    [Fact]
    public async Task GetCurrentMonthExpensesAsync_ShouldReturnCorrectExpenses()
    {
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dbContext = TestHelpers.CreateDbContext();

        dbContext.Transactions.AddRange(
            new Data.Models.Transaction
                { Id = Guid.NewGuid(), UserId = userId, Type = TransactionType.Expense, Amount = 300, Date = now },
            new Data.Models.Transaction
                { Id = Guid.NewGuid(), UserId = userId, Type = TransactionType.Expense, Amount = 150, Date = now }
        );

        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext);
        var expenses = await service.GetCurrentMonthExpensesAsync(userId);

        expenses.Should().Be(450);
    }

    [Fact]
    public async Task GetRecentTransactions_ShouldReturnLastSixTransactions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();

        var transactions = Enumerable.Range(0, 10)
            .Select(i => new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = i * 100,
                Date = DateTime.UtcNow.AddDays(-i)
            });

        dbContext.Transactions.AddRange(transactions);
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext);

        // Act
        var result = await service.GetRecentTransactions(userId);

        // Assert
        result.Should().HaveCount(6);
        result.First().Date.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromDays(1));
        result.Last().Date.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromDays(6));
    }

    [Fact]
    public async Task GetLastSixMonthsDataAsync_ShouldReturnMonthlyTotalsByType()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();

        var currentMonth = DateTime.UtcNow;
        for (int i = 0; i < 6; i++)
        {
            var monthDate = currentMonth.AddMonths(-i);
            dbContext.Transactions.AddRange(
                new Data.Models.Transaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = TransactionType.Income,
                    Amount = 1000 + (i * 100),
                    Date = monthDate
                },
                new Data.Models.Transaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = TransactionType.Expense,
                    Amount = 500 + (i * 50),
                    Date = monthDate
                }
            );
        }

        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext);

        // Act
        var (totals, labels) = await service.GetLastSixMonthsDataAsync(userId, TransactionType.Income);

        // Assert
        totals.Should().HaveCount(6);
        labels.Should().HaveCount(6);
    }

    [Fact]
    public async Task GetTopExpenseCategoriesAsync_ShouldReturnSortedCategories()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();

        dbContext.Transactions.AddRange(
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(), UserId = userId, Type = TransactionType.Expense, Category = Categories.Food,
                Amount = 300
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(), UserId = userId, Type = TransactionType.Expense, Category = Categories.Food,
                Amount = 200
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(), UserId = userId, Type = TransactionType.Expense, Category = Categories.Utilities,
                Amount = 150
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(), UserId = userId, Type = TransactionType.Expense,
                Category = Categories.Transportation, Amount = 100
            }
        );

        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext);

        // Act
        var topCategories = await service.GetTopExpenseCategoriesAsync(userId, 2);

        // Assert
        topCategories.Should().HaveCount(2);
        topCategories.First().Category.Should().Be(Categories.Food);
        topCategories.First().TotalAmount.Should().Be(500);
        topCategories.Last().Category.Should().Be(Categories.Utilities);
        topCategories.Last().TotalAmount.Should().Be(150);
    }

    [Fact]
    public async Task QuickAddTransactionAsync_ShouldAddExpenseTransaction()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();

        dbContext.Users.Add(new ApplicationUser { Id = userId });
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext);

        // Act
        var transaction = new Data.Models.Transaction { Amount = 100, Category = Categories.Food };
        await service.QuickAddTransactionAsync(transaction, userId);

        // Assert
        var addedTransaction = await dbContext.Transactions.FirstOrDefaultAsync(t => t.UserId == userId);
        addedTransaction.Should().NotBeNull();
        addedTransaction.Amount.Should().Be(100);
        addedTransaction.Type.Should().Be(TransactionType.Expense);
    }
}