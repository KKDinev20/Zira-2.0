using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI;
using Zira.Data;

internal static class DependencyInjection
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<EntityContext>();
        
        return services;
    }
}