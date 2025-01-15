using Microsoft.Extensions.DependencyInjection;
using Zira.Services.Accounts.Contracts;
using Zira.Services.Accounts.Internals;

namespace Zira.Services.Accounts;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountsService(
        this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        return services;
    }
}