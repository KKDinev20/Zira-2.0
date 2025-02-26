using Zira.Services.Analytics.Contracts;
using Zira.Services.Analytics.Internals;

namespace Zira.Services.Analytics;

using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddAnalyticsService(
        this IServiceCollection services)
    {
        services.AddScoped<IAnalyticsService, AnalyticsService>();

        return services;
    }
}