using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Data.Models;
using Zira.Presentation.Models;
using Zira.Services.Identity.Constants;
using Zira.Services.Reminder.Internals;

namespace Zira.Presentation.Controllers
{
    [Authorize(Policies.UserPolicy)]
    public class AutomationController : Controller
    {
        private readonly EntityContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHubContext<NotificationHub> _hubContext;

        public AutomationController(
            EntityContext context,
            UserManager<ApplicationUser> userManager,
            IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            this.userManager = userManager;
            this._hubContext = hubContext;
        }

        [HttpGet("/bill-reminders")]
        public async Task<IActionResult> BillReminders()
        {
            var user = await this.userManager.GetUserAsync(this.User);

            var reminders = await _context.Reminders
                .Where(r => r.UserId == user.Id)
                .OrderBy(r => r.DueDate)
                .Select(
                    r => new ReminderViewModel
                    {
                        Id = r.Id,
                        Title = r.Title,
                        Amount = r.Amount,
                        DueDate = r.DueDate,
                    })
                .ToListAsync();

            return View(reminders);
        }

        [HttpGet("/create-reminder")]
        public IActionResult CreateReminder()
        {
            return View(new ReminderViewModel());
        }

        [HttpPost("/create-reminder")]
        public async Task<IActionResult> CreateReminder(ReminderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            var user = await userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            try
            {
                var reminder = new Reminder
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Title = model.Title,
                    Remark = model.Remark ?? string.Empty,
                    Amount = model.Amount,
                    DueDate = model.DueDate,
                    Status = ReminderStatus.Pending,
                    IsNotified = false,
                };

                _context.Reminders.Add(reminder);
                await _context.SaveChangesAsync();

                await _hubContext.Clients.User(user.Id.ToString())
                    .SendAsync("ReceiveNotification", $"New Reminder: {reminder.Title} - {reminder.DueDate:yyyy-MM-dd}");

                TempData["ReminderTitle"] = reminder.Title;
                TempData["ReminderRemark"] = reminder.Remark;
                TempData["ReminderDueDate"] = reminder.DueDate.ToString("yyyy-MM-dd");
                TempData["ReminderAmount"] = reminder.Amount.ToString("C");

                return RedirectToAction("BillReminders");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating reminder: {ex.Message}";
                return View(model);
            }
        }


        [HttpPost("/send-notification/{reminderId}")]
        public async Task<IActionResult> SendNotification(Guid reminderId)
        {
            var reminder = await _context.Reminders.FirstOrDefaultAsync(r => r.Id == reminderId);

            if (reminder == null)
            {
                TempData["ErrorMessage"] = "Reminder not found.";
                return RedirectToAction("BillReminders");
            }

            var user = await userManager.FindByIdAsync(reminder.UserId.ToString());
            if (user != null)
            {
                var message = $"Reminder: {reminder.Title} - {reminder.DueDate:yyyy-MM-dd} to pay ${reminder.Amount}.";
                await _hubContext.Clients.User(user.Id.ToString()).SendAsync("ReceiveNotification", message);

                TempData["SuccessMessage"] = "Notification sent!";
            }
            else
            {
                TempData["ErrorMessage"] = "User not found.";
            }

            return RedirectToAction("BillReminders");
        }
    }
}