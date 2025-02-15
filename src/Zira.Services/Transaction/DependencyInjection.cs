using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zira.Services.Transaction.Contracts;
using Zira.Services.Transaction.Internals;

namespace Zira.Services.Transaction
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTransactionService(
            this IServiceCollection services)
        {
            services.AddScoped<ITransactionService, TransactionService>();

            return services;
        }
    }
}