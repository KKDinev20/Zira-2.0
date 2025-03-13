using Microsoft.Extensions.DependencyInjection;
using Zira.Services.Currency.Contracts;

namespace Zira.Services.Currency;

public static class DependencyInjection
{
    public static IServiceCollection AddCurrencyServices(this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddScoped<ICurrencyConverter, CurrencyConverter>();

        return services;
    }
}