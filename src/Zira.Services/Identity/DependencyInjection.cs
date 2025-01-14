using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Zira.Data;
using Zira.Data.Models;
using Zira.Services.Identity.Contracts;
using Zira.Services.Identity.Internals;

namespace Zira.Services.Identity;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services)
    {
        services
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<EntityContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }
}