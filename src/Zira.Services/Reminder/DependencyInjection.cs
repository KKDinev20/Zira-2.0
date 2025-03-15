using Microsoft.Extensions.DependencyInjection;
using Zira.Services.Reminder.Contracts;
using Zira.Services.Reminder.Internals;

namespace Zira.Services.Reminder;

public static class DependencyInjection
{
    public static IServiceCollection AddReminderService(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddSingleton<IPushNotificationService, PushNotificationService>();
        services.AddHostedService<ReminderService>();
        return services;
    }
}