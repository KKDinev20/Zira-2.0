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

        // Add dbCOntext, emailServices and add Users with enabled settings
        // Set body for email, if notif is enabled, add body for upcoming bills, if budget alerts are enabled add body for budgets
        // Delay the task for 1 day
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

                        string emailBody = $@"
    <html>
    <head>
        <style>
            body {{ font-family: Arial, sans-serif; }}
            .container {{ padding: 20px; }}
            .highlight {{ color: #d9534f; font-weight: bold; }}
            .section-title {{ font-size: 18px; font-weight: bold; margin-top: 10px; }}
            ul {{ padding-left: 20px; }}
            li {{ margin-bottom: 5px; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <h2>Здравейте, {user.UserName},</h2>
            <p>Надяваме се, че сте добре! Ето вашите финансови известия:</p>";

                        if (setting.EnableBillReminders)
                        {
                            var upcomingBills = await dbContext.Reminders
                                .Where(
                                    r => r.UserId == user.Id && !r.IsNotified &&
                                         r.DueDate <= DateTime.UtcNow.AddDays(7))
                                .ToListAsync();

                            if (upcomingBills.Any())
                            {
                                emailBody += $@"
            <p class='section-title'>📌 Предстоящи сметки</p>
            <ul>";

                                foreach (var bill in upcomingBills)
                                {
                                    emailBody +=
                                        $"<li><b>{bill.Title}</b>: Дата на плащане: <span class='highlight'>{bill.DueDate:yyyy-MM-dd}</span>, Сума: <span class='highlight'>{bill.Amount} лв.</span></li>";
                                    bill.IsNotified = true;
                                }

                                emailBody += "</ul>";
                            }
                        }

                        if (setting.EnableBudgetAlerts)
                        {
                            emailBody +=
                                "<p class='section-title'>💰 Напомняне за бюджет</p><p>Не забравяйте да следите бюджета си и да останете на правилния път!</p>";
                        }

                        emailBody += @"
            <p>🔗 Проверете вашите сметки за електричество и вода:</p>
            <ul>
                <li><a href='https://evn.bg/Online/Login.aspx?returnurl=%2fOnline%2fLogin%2fInfo.aspx' target='_blank'>EVN - Електричество</a></li>
                <li><a href='https://vik-burgas.com/' target='_blank'>ВиК Бургас - Вода</a></li>
            </ul>
        </div>
    </body>
    </html>";

                        if (!string.IsNullOrWhiteSpace(emailBody))
                        {
                            var emailModel = new EmailModel
                            {
                                ToEmail = user.Email,
                                Subject = "📢 Важни финансови актуализации",
                                Body = emailBody,
                            };
                            await emailService.SendEmailAsync(emailModel);
                        }
                    }

                    await dbContext.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}