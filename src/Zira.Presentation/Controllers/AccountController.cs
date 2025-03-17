using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zira.Common;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Data.Models;
using Zira.Presentation.Extensions;
using Zira.Presentation.Models;

namespace Zira.Presentation.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly EntityContext context;
    private readonly IWebHostEnvironment webHostEnvironment;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        EntityContext context,
        IWebHostEnvironment webHostEnvironment)
    {
        this.userManager = userManager;
        this.context = context;
        this.webHostEnvironment = webHostEnvironment;
    }

    [HttpGet("/profile")]
    public async Task<IActionResult> Profile()
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.context);

        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.RedirectToAction("Login", "Authentication");
        }

        var currencies = await this.context.Currencies.ToListAsync();
        this.ViewBag.Currencies = currencies;

        var viewModel = new ProfileViewModel
        {
            FirstName = user!.FirstName,
            LastName = user!.LastName,
            Email = user!.Email,
            BirthDate = user.Birthday,
            AvatarUrl = string.IsNullOrEmpty(user.ImageUrl)
                ? "/dashboard/assets/img/avatars/default.jpg"
                : user.ImageUrl,
            PreferredCurrency = user.PreferredCurrency,
            PreferredCurrencyCode = user.PreferredCurrencyCode,
        };

        user.PreferredCurrency.Symbol = "лв.";

        return this.View(viewModel);
    }

    [HttpPost("/profile/update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(
        string firstName,
        string lastName,
        string email,
        DateTime birthDate,
        IFormFile? avatarFile,
        string preferredCurrency,
        bool resetAvatar = false)
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.RedirectToAction("Login", "Authentication");
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = email;
        user.NormalizedEmail = email.ToUpper();
        user.Birthday = birthDate;

        if (string.IsNullOrEmpty(preferredCurrency))
        {
            user.PreferredCurrencyCode = "BGN";
        }
        else
        {
            user.PreferredCurrencyCode = preferredCurrency;
        }

        var currency = await this.context.Currencies
            .FirstOrDefaultAsync(c => c.Code == user.PreferredCurrencyCode);

        if (currency != null)
        {
            user.PreferredCurrency = currency;
        }
        else
        {
            var defaultCurrency = await this.context.Currencies.FirstOrDefaultAsync(c => c.Code == "BGN");
            if (defaultCurrency != null)
            {
                user.PreferredCurrency = defaultCurrency;
                user.PreferredCurrencyCode = "BGN";
            }
        }

        if (resetAvatar)
        {
            user.ImageUrl = "/dashboard/assets/img/avatars/default.jpg";
        }
        else if (avatarFile != null && avatarFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(avatarFile.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                this.ModelState.AddModelError("AvatarFile", "Invalid file type. Allowed: JPG, PNG, GIF.");
                return this.View("Profile");
            }

            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var uploadPath = Path.Combine(
                this.webHostEnvironment.WebRootPath,
                "dashboard/assets/img/avatars",
                uniqueFileName);

            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await avatarFile.CopyToAsync(stream);
            }

            user.ImageUrl = $"/dashboard/assets/img/avatars/{uniqueFileName}";
        }

        var result = await this.userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            this.ModelState.AddModelError(" ", "Failed to update profile.");
            return this.View("Profile");
        }

        this.TempData["SuccessMessage"] = @AccountText.ProfileSuccess;
        return this.RedirectToAction("Profile");
    }

    [HttpGet("/complete-profile")]
    public async Task<IActionResult> CompleteProfile()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.RedirectToDefault();
        }

        var applicationUser = await this.context.Users
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        var viewModel = new CompleteProfileViewModel
        {
            FirstName = applicationUser?.FirstName,
            LastName = applicationUser?.LastName,
            BirthDate = applicationUser?.Birthday ?? DateTime.MinValue,
            AvatarUrl = applicationUser?.ImageUrl,
        };

        return this.View(viewModel);
    }

    [HttpPost("/complete-profile")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompleteProfile(CompleteProfileViewModel model, IFormFile? avatarUrl)
    {
        if (this.ModelState.IsValid)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.RedirectToAction(nameof(AuthenticationController.Login), "Authentication");
            }

            var applicationUser = await this.context.Users
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (applicationUser != null)
            {
                applicationUser.FirstName = model.FirstName;
                applicationUser.LastName = model.LastName;
                applicationUser.Birthday = model.BirthDate;

                if (avatarUrl != null && avatarUrl.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(avatarUrl.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        this.ModelState.AddModelError("AvatarUrl", "Invalid file type. Please upload an image file.");
                        return this.View(model);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                    var uploadPath = Path.Combine(
                        this.webHostEnvironment.WebRootPath,
                        "dashboard/assets/img/avatars",
                        uniqueFileName);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await avatarUrl.CopyToAsync(stream);
                    }

                    applicationUser.ImageUrl = $"/dashboard/assets/img/avatars/{uniqueFileName}";
                }
                else if (string.IsNullOrEmpty(applicationUser.ImageUrl))
                {
                    applicationUser.ImageUrl = "/dashboard/assets/img/avatars/default.jpg";
                }

                applicationUser.PreferredCurrencyCode = "BGN";

                var defaultCurrency = await this.context.Currencies.FirstOrDefaultAsync(c => c.Code == "BGN");
                if (defaultCurrency != null)
                {
                    applicationUser.PreferredCurrency = defaultCurrency;
                }


                this.context.Users.Update(applicationUser);
                await this.context.SaveChangesAsync();

                return this.RedirectToDefault();
            }
        }

        return this.View(model);
    }

    [HttpGet("/profile/change-password")]
    public IActionResult ChangePassword(ChangePasswordViewModel model)
    {
        return this.View(model);
    }

    [HttpPost("/profile/change-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.RedirectToAction("Login", "Authentication");
        }

        var result = await this.userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            return this.View(model);
        }

        this.TempData["SuccessMessage"] = @AccountText.PasswordSuccess;
        return this.RedirectToAction("Profile");
    }

    [HttpGet("/profile/notification-preferences")]
    public async Task<IActionResult> NotificationPreferences()
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.context);
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.RedirectToAction("Login", "Authentication");
        }

        var settings = await this.context.ReminderSettings
            .FirstOrDefaultAsync(x => x.UserId == user.Id);

        if (settings == null)
        {
            settings = new ReminderSettings
            {
                EnableBillReminders = true,
                EnableBudgetAlerts = true,
            };
        }

        return this.View(settings);
    }

    [HttpPost("/profile/update-notifications")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateNotificationPreferences(
        bool enableBillReminders,
        bool enableBudgetAlerts,
        NotificationType preferredNotification)
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.RedirectToAction("Login", "Authentication");
        }

        await this.UpdateReminderSettings(user.Id, enableBillReminders, enableBudgetAlerts, preferredNotification);

        this.TempData["SuccessMessage"] = "Notification preferences updated successfully.";
        return this.RedirectToAction("NotificationPreferences");
    }

    private async Task UpdateReminderSettings(
        Guid userId,
        bool billReminders,
        bool budgetAlerts,
        NotificationType notificationType)
    {
        var settings = await this.context.ReminderSettings.FirstOrDefaultAsync(x => x.UserId == userId);

        if (settings == null)
        {
            settings = new ReminderSettings
            {
                UserId = userId,
                EnableBillReminders = billReminders,
                EnableBudgetAlerts = budgetAlerts,
            };
            await this.context.ReminderSettings.AddAsync(settings);
        }
        else
        {
            settings.EnableBillReminders = billReminders;
            settings.EnableBudgetAlerts = budgetAlerts;
            this.context.ReminderSettings.Update(settings);
        }

        await this.context.SaveChangesAsync();
    }
}