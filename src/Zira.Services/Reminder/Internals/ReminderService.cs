using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Zira.Data;
using Zira.Services.Common.Contracts;
using Zira.Services.Common.Models;

namespace Zira.Services.Reminder.Internals
{
    public class ReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public ReminderService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<EntityContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var usersWithSettings = await dbContext.ReminderSettings
                        .Include(rs => rs.User)
                        .Where(rs => rs.EnableBillReminders || rs.EnableBudgetAlerts)
                        .ToListAsync();

                    foreach (var setting in usersWithSettings)
                    {
                        var user = setting.User;
                        if (user == null)
                        {
                            continue;
                        }

                        string emailBody = $"Hello {user.UserName},\n\n";

                        if (setting.EnableBillReminders)
                        {
                            var upcomingBills = await dbContext.Reminders
                                .Where(r => r.UserId == user.Id && !r.IsNotified && r.DueDate <= DateTime.UtcNow.AddDays(7))
                                .ToListAsync();

                            if (upcomingBills.Any())
                            {
                                emailBody += $"📌 You have {upcomingBills.Count} upcoming bill(s) due soon:\n";
                                foreach (var bill in upcomingBills)
                                {
                                    emailBody += $"- {bill.Title}: Due {bill.DueDate:yyyy-MM-dd}, Amount: ${bill.Amount}\n";
                                    bill.IsNotified = true;
                                }
                            }
                        }

                        if (setting.EnableBudgetAlerts)
                        {
                            emailBody += "\n💰 Don't forget to check your budget and stay on track!\n";
                        }

                        emailBody += "\n🔗 More info in your account at: [YOUR_WEBSITE_LINK]";

                        if (!string.IsNullOrWhiteSpace(emailBody))
                        {
                            var emailModel = new EmailModel
                            {
                                ToEmail = user.Email,
                                Subject = "📢 Important Financial Updates",
                                Body = emailBody,
                            };
                            await emailService.SendEmailAsync(emailModel);
                        }
                    }

                    await dbContext.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromDays(3), stoppingToken);
            }
        }
    }
}
