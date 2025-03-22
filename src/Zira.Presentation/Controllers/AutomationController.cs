using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Zira.Common;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Data.Models;
using Zira.Presentation.Extensions;
using Zira.Presentation.Models;
using Zira.Services.Currency.Contracts;
using Zira.Services.Identity.Constants;
using Zira.Services.Identity.Extensions;
using Zira.Services.Reminder.Internals;

namespace Zira.Presentation.Controllers
{
    [Authorize(Policies.UserPolicy)]
    public class AutomationController : Controller
    {
        private readonly EntityContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHubContext<NotificationHub> hubContext;
        private readonly ICurrencyConverter currencyConverter;

        public AutomationController(
            EntityContext context,
            UserManager<ApplicationUser> userManager,
            IHubContext<NotificationHub> hubContext,
            ICurrencyConverter currencyConverter)
        {
            this.context = context;
            this.userManager = userManager;
            this.hubContext = hubContext;
            this.currencyConverter = currencyConverter;
        }

        [HttpGet("/bill-reminders")]
        public async Task<IActionResult> BillReminders(int page = 1, int pageSize = 5)
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);

            var user = await this.userManager.GetUserAsync(this.User);

            var totalReminders = await this.context.Reminders
                .Where(r => r.UserId == user.Id)
                .CountAsync();

            var reminders = await this.context.Reminders
                .Where(r => r.UserId == user.Id)
                .Include(r => r.Currency)
                .OrderBy(r => r.DueDate)
                .Select(
                    r => new ReminderViewModel
                    {
                        Id = r.Id,
                        Title = r.Title,
                        Amount = r.Amount,
                        DueDate = r.DueDate,
                        Currency = r.Currency,
                        CurrencyCode = r.CurrencyCode
                    })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            var totalPages = (int)Math.Ceiling((double)totalReminders / pageSize);

            var viewModel = new PaginatedViewModel<ReminderViewModel>
            {
                Items = reminders,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                ReminderCount = await this.context.Reminders.CountAsync(r => !r.IsNotified),
            };

            return this.View(viewModel);
        }

        [HttpGet("/create-reminder")]
        public async Task<IActionResult> CreateReminder()
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);
            var availableCurrencies = await this.context.Currencies
                .Select(c => c.Code)
                .ToListAsync();
            var userId = this.User.GetUserId();
            var user = await this.userManager.FindByIdAsync(userId.ToString());
            this.ViewBag.Currencies = availableCurrencies;
            this.ViewBag.DefaultCurrency = user?.PreferredCurrencyCode ?? "BGN";

            return this.View(new ReminderViewModel());
        }

        [HttpPost("/create-reminder")]
        public async Task<IActionResult> CreateReminder(ReminderViewModel model)
        {
            await this.SetGlobalUserInfoAsync(this.userManager, this.context);

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                this.TempData["ErrorMessage"] = @AccountText.UserNotFound;
                return this.View(model);
            }
            
            model.Currency = await this.context.Currencies
                .FirstOrDefaultAsync(c => c.Code == model.CurrencyCode);

            try
            {
                var reminder = new Reminder
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Title = model.Title,
                    Remark = model.Remark,
                    Amount = model.Amount,
                    DueDate = model.DueDate,
                    Status = ReminderStatus.Pending,
                    IsNotified = false,
                    Currency = model.Currency,
                    CurrencyCode = model.CurrencyCode,
                };

                this.context.Reminders.Add(reminder);
                await this.context.SaveChangesAsync();

                this.TempData["ReminderTitle"] = reminder.Title;
                this.TempData["ReminderRemark"] = reminder.Remark;
                this.TempData["ReminderDueDate"] = reminder.DueDate.ToString("yyyy-MM-dd");
                this.TempData["ReminderAmount"] = reminder.Amount.ToString("N2");

                this.TempData["SuccessMessage"] = @ReminderText.ReminderSuccess;
                return this.RedirectToAction("BillReminders");
            }
            catch (Exception ex)
            {
                this.TempData["ErrorMessage"] = $"Грешка присъздаването на известие: {ex.Message}";
                return this.View(model);
            }
        }

        [HttpPost("/send-notification/{reminderId}")]
        public async Task<IActionResult> SendNotification(Guid reminderId)
        {
            var reminder = await this.context.Reminders.FirstOrDefaultAsync(r => r.Id == reminderId);

            if (reminder == null)
            {
                this.TempData["ErrorMessage"] = @ReminderText.ReminderNotFound;
                return this.RedirectToAction("BillReminders");
            }

            var user = await this.userManager.FindByIdAsync(reminder.UserId.ToString());
            if (user != null)
            {
                var message = $"Известие: {reminder.Title} - {reminder.DueDate:yyyy-MM-dd} със сума ${reminder.Amount}.";
                await this.hubContext.Clients.User(user.Id.ToString()).SendAsync("ReceiveNotification", message);

                this.TempData["SuccessMessage"] = @ReminderText.NotificationSuccess;
            }
            else
            {
                this.TempData["ErrorMessage"] = @AccountText.UserNotFound;
            }

            return this.RedirectToAction("BillReminders");
        }
    }
}