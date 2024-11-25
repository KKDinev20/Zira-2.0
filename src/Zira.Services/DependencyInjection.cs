using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Zira.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentityServices();
        
        return services;
    }
}