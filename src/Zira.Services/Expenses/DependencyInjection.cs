using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zira.Services.Expenses.Contracts;
using Zira.Services.Expenses.Internals;


namespace Zira.Services.Expenses;

public static class DependencyInjection
{
    public static IServiceCollection AddExpenseServices(
        this IServiceCollection services)
    {
        services.AddScoped<IExpenseService, ExpenseService>();

        return services;
    }
}