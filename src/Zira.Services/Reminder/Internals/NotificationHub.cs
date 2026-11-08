using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Zira.Services.Reminder.Internals
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public async Task SendNotification(Guid userId, string message)
        {
            await this.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
        }
    }
}