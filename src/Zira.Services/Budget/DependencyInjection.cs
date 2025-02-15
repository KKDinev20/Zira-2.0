using Microsoft.Extensions.DependencyInjection;
using Zira.Services.Budget.Contracts;

namespace Zira.Services.Budget;

public static class DependencyInjection
{
    public static IServiceCollection AddBudgetServices(
        this IServiceCollection services)
    {
        services.AddScoped<IBudgetService, BudgetService>();

        return services;
    }
}