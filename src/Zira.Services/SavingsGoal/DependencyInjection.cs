using Microsoft.Extensions.DependencyInjection;
using Zira.Services.SavingsGoal.Contracts;
using Zira.Services.SavingsGoal.Internals;

namespace Zira.Services.SavingsGoal;

public static class DependencyInjection
{
    public static IServiceCollection AddSavingsGoalServices(this IServiceCollection services)
    {
        services.AddScoped<ISavingsGoalService, SavingsGoalService>();

        return services;
    }
}