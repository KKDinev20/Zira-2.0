using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zira.Services.Common;
using Zira.Services.Identity;

namespace Zira.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCommonServices(configuration);
        services.AddIdentityServices();
        
        return services;
    }
}