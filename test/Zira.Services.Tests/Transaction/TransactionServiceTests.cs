using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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

        // ✅ Check if budget is updated correctly
        var updatedBudget = await dbContext.Budgets.FirstOrDefaultAsync(b => b.UserId == userId && b.Category == category);
        updatedBudget.Should().NotBeNull();
        updatedBudget.Amount.Should().Be(150); // 200 - 50 = 150
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

    [Fact]
    public async Task GetRecentTransactions_ShouldReturnLastSixTransactions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();


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

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager
        );

        // Act
        var result = await service.GetRecentTransactions(userId);

        // Assert
        result.Should().HaveCount(6);
        result.First().Date.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromDays(1));
        result.Last().Date.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromDays(6));
    }
    
    [Fact]
    public async Task QuickAddTransactionAsync_ShouldAddExpenseTransaction()
    {
        // Arrange
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
        dbContext.Users.Add(new ApplicationUser { Id = userId });
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager
        );

        // Act
        var transaction = new Data.Models.Transaction {  Amount = 100,
            TransactionId = "20250320091998",
            Type = TransactionType.Expense,
            Date = DateTime.UtcNow,
            CurrencyCode = "USD", };
        await service.QuickAddTransactionAsync(transaction, userId);

        // Assert
        var addedTransaction = await dbContext.Transactions.FirstOrDefaultAsync(t => t.UserId == userId);
        addedTransaction.Should().NotBeNull();
        addedTransaction.Amount.Should().Be(100);
        addedTransaction.Type.Should().Be(TransactionType.Expense);
    }
    
    [Fact]
    public async Task GetUserTransactionsAsync_ShouldReturnAllUserTransactions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();

        await dbContext.Currencies.AddAsync(new Data.Models.Currency { Code = "USD", Name = "US Dollar", Symbol = "$" });
        await dbContext.SaveChangesAsync();

        // Add several transactions for the user
        for (int i = 0; i < 5; i++)
        {
            dbContext.Transactions.Add(new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = 100 + i * 25,
                Date = DateTime.UtcNow.AddDays(-i),
                CurrencyCode = "USD",
                Type = i % 2 == 0 ? TransactionType.Income : TransactionType.Expense
            });
        }
        
        // Add a transaction for a different user (should not be returned)
        dbContext.Transactions.Add(new Data.Models.Transaction
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Amount = 300,
            Date = DateTime.UtcNow,
            CurrencyCode = "USD"
        });
        
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager);

        // Act
        var result = await service.GetUserTransactionsAsync(userId);

        // Assert
        result.Should().HaveCount(5);
        result.All(t => t.UserId == userId).Should().BeTrue();
        result.Should().BeInDescendingOrder(t => t.Date);
    }

    [Fact]
    public async Task GetTotalTransactionRecordsAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();

        // Add several transactions for the user
        for (int i = 0; i < 8; i++)
        {
            dbContext.Transactions.Add(new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = 100,
                Date = DateTime.UtcNow
            });
        }
        
        // Add transactions for a different user (should not be counted)
        for (int i = 0; i < 3; i++)
        {
            dbContext.Transactions.Add(new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Amount = 200,
                Date = DateTime.UtcNow
            });
        }
        
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager);

        // Act
        var result = await service.GetTotalTransactionRecordsAsync(userId);

        // Assert
        result.Should().Be(8);
    }

    [Fact]
    public async Task GetCurrentMonthIncomeAsync_ShouldReturnCorrectSum()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();

        await dbContext.Currencies.AddAsync(new Data.Models.Currency { Code = "USD", Name = "US Dollar", Symbol = "$" });
        await dbContext.SaveChangesAsync();

        var now = DateTime.UtcNow;
        
        // Current month income transactions
        dbContext.Transactions.AddRange(
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Income,
                Amount = 100,
                Date = now,
                CurrencyCode = "USD"
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Income,
                Amount = 200,
                Date = now,
                CurrencyCode = "USD"
            }
        );
        
        // Previous month income (should not be counted)
        dbContext.Transactions.Add(new Data.Models.Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = TransactionType.Income,
            Amount = 500,
            Date = now.AddMonths(-1),
            CurrencyCode = "USD"
        });
        
        // Current month expense (should not be counted)
        dbContext.Transactions.Add(new Data.Models.Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = TransactionType.Expense,
            Amount = 50,
            Date = now,
            CurrencyCode = "USD"
        });
        
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager);

        // Act
        var result = await service.GetCurrentMonthIncomeAsync(userId);

        // Assert
        result.Should().Be(300); // 100 + 200 = 300
    }

    [Fact]
    public async Task GetCurrentMonthExpensesAsync_ShouldReturnCorrectSum()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();

        await dbContext.Currencies.AddAsync(new Data.Models.Currency { Code = "USD", Name = "US Dollar", Symbol = "$" });
        await dbContext.SaveChangesAsync();

        var now = DateTime.UtcNow;
        
        // Current month expense transactions
        dbContext.Transactions.AddRange(
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Expense,
                Amount = 75,
                Date = now,
                CurrencyCode = "USD"
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Expense,
                Amount = 125,
                Date = now,
                CurrencyCode = "USD"
            }
        );
        
        // Previous month expense (should not be counted)
        dbContext.Transactions.Add(new Data.Models.Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = TransactionType.Expense,
            Amount = 500,
            Date = now.AddMonths(-1),
            CurrencyCode = "USD"
        });
        
        // Current month income (should not be counted)
        dbContext.Transactions.Add(new Data.Models.Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = TransactionType.Income,
            Amount = 300,
            Date = now,
            CurrencyCode = "USD"
        });
        
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager);

        // Act
        var result = await service.GetCurrentMonthExpensesAsync(userId);

        // Assert
        result.Should().Be(200); // 75 + 125 = 200
    }

    [Fact]
    public async Task GetCurrentMonthFoodExpense_ShouldReturnCorrectSum()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();

        await dbContext.Currencies.AddAsync(new Data.Models.Currency { Code = "USD", Name = "US Dollar", Symbol = "$" });
        await dbContext.SaveChangesAsync();

        var now = DateTime.UtcNow;
        
        // Current month food expenses
        dbContext.Transactions.AddRange(
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Expense,
                Category = Categories.Food,
                Amount = 50,
                Date = now,
                CurrencyCode = "USD"
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Expense,
                Category = Categories.Food,
                Amount = 30,
                Date = now,
                CurrencyCode = "USD"
            }
        );
        
        // Other category expense (should not be counted)
        dbContext.Transactions.Add(new Data.Models.Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = TransactionType.Expense,
            Category = Categories.Entertainment,
            Amount = 100,
            Date = now,
            CurrencyCode = "USD"
        });
        
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager);

        // Act
        var result = await service.GetCurrentMonthFoodExpense(userId);

        // Assert
        result.Should().Be(80); // 50 + 30 = 80
    }
    
    [Fact]
    public async Task GetCurrentMonthUtilitiesExpense_ShouldReturnCorrectSum()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();

        await dbContext.Currencies.AddAsync(new Data.Models.Currency { Code = "USD", Name = "US Dollar", Symbol = "$" });
        await dbContext.SaveChangesAsync();

        var now = DateTime.UtcNow;
        
        // Current month food expenses
        dbContext.Transactions.AddRange(
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Expense,
                Category = Categories.Utilities,
                Amount = 50,
                Date = now,
                CurrencyCode = "USD"
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Expense,
                Category = Categories.Utilities,
                Amount = 30,
                Date = now,
                CurrencyCode = "USD"
            }
        );
        
        dbContext.Transactions.Add(new Data.Models.Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = TransactionType.Expense,
            Category = Categories.Entertainment,
            Amount = 100,
            Date = now,
            CurrencyCode = "USD"
        });
        
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager);

        // Act
        var result = await service.GetCurrentMonthUtilitiesExpense(userId);

        // Assert
        result.Should().Be(80);
    }

    [Fact]
    public async Task GetCurrentWeekTotalAsync_ShouldReturnCorrectSum()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();

        var today = DateTime.UtcNow;
        int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
        var monday = today.AddDays(-1 * diff).Date;
        
        // Transactions within current week
        dbContext.Transactions.AddRange(
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Income,
                Amount = 200,
                Date = monday.AddDays(1)
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Income,
                Amount = 300,
                Date = monday.AddDays(2)
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Expense,
                Amount = 150,
                Date = monday.AddDays(3)
            }
        );
        
        // Transaction from previous week (should not be counted)
        dbContext.Transactions.Add(new Data.Models.Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = TransactionType.Income,
            Amount = 500,
            Date = monday.AddDays(-1)
        });
        
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager);

        // Act
        var incomeResult = await service.GetCurrentWeekTotalAsync(userId, TransactionType.Income);
        var expenseResult = await service.GetCurrentWeekTotalAsync(userId, TransactionType.Expense);

        // Assert
        incomeResult.Should().Be(500); 
        expenseResult.Should().Be(150);
    }

    [Fact]
    public async Task GetTopExpenseCategoriesAsync_ShouldReturnTopCategories()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();

        await dbContext.Currencies.AddAsync(new Data.Models.Currency { Code = "USD", Name = "US Dollar", Symbol = "$" });
        await dbContext.SaveChangesAsync();
        
        dbContext.Transactions.AddRange(
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Expense,
                Category = Categories.Entertainment,
                Amount = 200,
                CurrencyCode = "USD",
                Date = DateTime.UtcNow
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Expense,
                Category = Categories.Food,
                Amount = 150,
                CurrencyCode = "USD",
                Date = DateTime.UtcNow
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Expense,
                Category = Categories.Entertainment,
                Amount = 300,
                CurrencyCode = "USD",
                Date = DateTime.UtcNow
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TransactionId = "20250320091998",
                Type = TransactionType.Expense,
                Category = Categories.Transportation,
                Amount = 120,
                CurrencyCode = "USD",
                Date = DateTime.UtcNow
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TransactionId = "20250320091998",
                Type = TransactionType.Expense,
                Category = Categories.Healthcare,
                Amount = 90,
                CurrencyCode = "USD",
                Date = DateTime.UtcNow
            }
        );
        
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager);

        // Act
        var top3Result = await service.GetTopExpenseCategoriesAsync(userId, 3);

        // Assert
        top3Result.Should().HaveCount(3);
        top3Result[0].Category.Should().Be(Categories.Entertainment);
        top3Result[0].TotalAmount.Should().Be(500);
        top3Result[1].Category.Should().Be(Categories.Food);
        top3Result[1].TotalAmount.Should().Be(150); 
        top3Result[2].Category.Should().Be(Categories.Transportation);
        top3Result[2].TotalAmount.Should().Be(120);
    }

    [Fact]
    public async Task GetMonthlyIncomeAndExpensesAsync_ShouldReturnCorrectValues()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();

        await dbContext.Currencies.AddAsync(new Data.Models.Currency { Code = "USD", Name = "US Dollar", Symbol = "$" });
        await dbContext.SaveChangesAsync();
        
        var year = 2025;
        
        // January transactions
        dbContext.Transactions.AddRange(
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Income,
                Amount = 1000,
                CurrencyCode = "USD",
                Date = new DateTime(year, 1, 15)
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Expense,
                Amount = 600,
                CurrencyCode = "USD",
                Date = new DateTime(year, 1, 20)
            }
        );
        
        // February transactions
        dbContext.Transactions.AddRange(
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Income,
                Amount = 1200,
                CurrencyCode = "USD",
                Date = new DateTime(year, 2, 10)
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Expense,
                Amount = 700,
                CurrencyCode = "USD",
                Date = new DateTime(year, 2, 25)
            }
        );
        
        // Different year (should not be included)
        dbContext.Transactions.Add(new Data.Models.Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = TransactionType.Income,
            Amount = 5000,
            CurrencyCode = "USD",
            Date = new DateTime(year - 1, 12, 31)
        });
        
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager);

        // Act
        var (incomes, expenses) = await service.GetMonthlyIncomeAndExpensesAsync(userId, year);

        // Assert
        incomes.Should().HaveCount(12);
        expenses.Should().HaveCount(12);
        
        incomes[0].Should().Be(1000); // January
        incomes[1].Should().Be(1200); // February
        expenses[0].Should().Be(600); // January
        expenses[1].Should().Be(700); // February
    }

    [Fact]
    public async Task GetNetWorthTrendAsync_ShouldCalculateCorrectNetWorth()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestHelpers.CreateDbContext();
        var idService = TestHelpers.CreateIdGenerationService();
        var currencyConverter = TestHelpers.CreateCurrencyConverterService();
        var mockUserManager = TestHelpers.CreateMockUserManager();

        await dbContext.Currencies.AddAsync(new Data.Models.Currency { Code = "USD", Name = "US Dollar", Symbol = "$" });
        await dbContext.SaveChangesAsync();
        
        dbContext.Transactions.AddRange(
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Income,
                Amount = 2000,
                CurrencyCode = "USD",
                Date = new DateTime(2025, 1, 15)
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Expense,
                Amount = 800,
                CurrencyCode = "USD",
                Date = new DateTime(2025, 1, 20)
            }
        );
        
        // February transactions
        dbContext.Transactions.AddRange(
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Income,
                Amount = 2200,
                CurrencyCode = "USD",
                Date = new DateTime(2025, 2, 10)
            },
            new Data.Models.Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = TransactionType.Expense,
                Amount = 1200,
                CurrencyCode = "USD",
                Date = new DateTime(2025, 2, 25)
            }
        );
        
        await dbContext.SaveChangesAsync();

        var service = new TransactionService(dbContext, idService, currencyConverter, mockUserManager);

        // Act
        var netWorthTrend = await service.GetNetWorthTrendAsync(userId);

        // Assert
        netWorthTrend.Should().HaveCount(2); // Two months
        
        netWorthTrend[0].Month.Should().Be(new DateTime(2025, 1, 1));
        netWorthTrend[0].NetWorth.Should().Be(1200); // 2000 - 800 = 1200
        
        netWorthTrend[1].Month.Should().Be(new DateTime(2025, 2, 1));
        netWorthTrend[1].NetWorth.Should().Be(1000); // 2200 - 1200 = 1000
    }
}