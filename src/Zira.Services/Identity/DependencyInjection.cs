using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Zira.Data;

namespace Zira.Services.Identity;

internal static class DependencyInjection
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<EntityContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}