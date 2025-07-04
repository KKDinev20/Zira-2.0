using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zira.Services.Accounts;
using Zira.Services.Analytics;
using Zira.Services.Budget;
using Zira.Services.Common;
using Zira.Services.Currency;
using Zira.Services.Identity;
using Zira.Services.Reminder;
using Zira.Services.SavingsGoal;
using Zira.Services.Transaction;

namespace Zira.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCommonServices(configuration);
        services.AddIdentityServices();
        services.AddAccountsService();
        services.AddTransactionService();
        services.AddBudgetServices();
        services.AddAnalyticsService();
        services.AddSavingsGoalServices();
        services.AddCurrencyServices();
        services.AddReminderService();

        return services;
    }
}