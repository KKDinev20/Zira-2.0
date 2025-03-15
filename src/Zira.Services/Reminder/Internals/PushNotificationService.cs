using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Zira.Services.Reminder.Contracts;

namespace Zira.Services.Reminder.Internals
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public PushNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendPushNotificationAsync(Guid userId, string message)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
        }
    }
}