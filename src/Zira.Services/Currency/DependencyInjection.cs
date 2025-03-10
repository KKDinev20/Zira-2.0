using Microsoft.Extensions.DependencyInjection;
using Zira.Services.Currency.Contracts;
using Zira.Services.Currency.Internals;

namespace Zira.Services.Currency;

public static class DependencyInjection
{
    public static IServiceCollection AddCurrencyServices(
        this IServiceCollection services)
    {
        services.AddScoped<ICurrencyConverter, CurrencyConverter>();

        return services;
    }
}