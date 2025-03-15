using System;
using System.Threading.Tasks;

namespace Zira.Services.Reminder.Contracts
{
    public interface IPushNotificationService
    {
        Task SendPushNotificationAsync(Guid userId, string message);
    }
}