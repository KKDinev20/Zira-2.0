using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Zira.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddData(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}