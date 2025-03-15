using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Zira.Services.Reminder.Internals
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(Guid userId, string message)
        {
            await this.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
        }
    }
}