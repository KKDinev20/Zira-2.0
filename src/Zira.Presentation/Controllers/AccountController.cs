using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zira.Common;
using Zira.Data;
using Zira.Data.Models;
using Zira.Presentation.Extensions;
using Zira.Presentation.Models;

namespace Zira.Presentation.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly EntityContext context;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        EntityContext context)
    {
        this.userManager = userManager;
        this.context = context;
    }

    [HttpGet("/complete-profile")]
    public async Task<IActionResult> CompleteProfile()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.RedirectToAction(nameof(AuthenticationController.Login), "Authentication");
        }

        var applicationUser = await this.context.Users
            .Include(u => u.ApplicationUser)
            .FirstOrDefaultAsync(u => u.ApplicationUserId == user.Id);

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
    public async Task<IActionResult> CompleteProfile(CompleteProfileViewModel model, IFormFile AvatarUrl)
    {
        if (this.ModelState.IsValid)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.RedirectToAction(nameof(AuthenticationController.Login), "Authentication");
            }

            var applicationUser = await this.context.Users
                .Include(u => u.ApplicationUser)
                .FirstOrDefaultAsync(u => u.ApplicationUserId == user.Id);

            if (applicationUser != null)
            {
                applicationUser.FirstName = model.FirstName;
                applicationUser.LastName = model.LastName;
                applicationUser.Birthday = model.BirthDate;

                if (AvatarUrl.Length > 0)
                {
                    var filePath = Path.Combine("wwwroot\\dashboard\\assets\\img\\avatars", AvatarUrl.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await AvatarUrl.CopyToAsync(stream);
                    }

                    applicationUser.ImageUrl = $"wwwroot\\dashboard\\assets\\img\\avatars\\{AvatarUrl.FileName}";
                }
                else
                {
                    applicationUser.ImageUrl = $"wwwroot\\dashboard\\assets\\img\\avatars\\default.jpg";
                }

                this.context.Users.Update(applicationUser);
                await this.context.SaveChangesAsync();
            }

            return this.RedirectToDefault();
        }

        return this.View(model);
    }

    [HttpGet("/change-email")]
    public IActionResult ChangeEmail()
    {
        var model = new ChangeEmailViewModel();
        return this.View(model);
    }

    [HttpPost("/change-email")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
    {
        if (this.ModelState.IsValid)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.RedirectToDefault();
            }

            var token = await this.userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail!);
            var result = await this.userManager.ChangeEmailAsync(user, model.NewEmail!, token);

            if (result.Succeeded)
            {
                user.UserName = model.NewEmail!;
                var usernameUpdateResult = await this.userManager.UpdateAsync(user);

                if (usernameUpdateResult.Succeeded)
                {
                    await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    this.TempData["MessageText"] = @AuthenticationText.EmailChangeSuccess;
                    this.TempData["MessageVariant"] = "success";
                }
                else
                {
                    await this.userManager.ChangeEmailAsync(user, user.Email!, token);
                    this.ModelState.AddModelError(string.Empty, @AuthenticationText.UsernameChangeFailed);
                }
            }

            this.ModelState.AddModelError(string.Empty, @AuthenticationText.EmailChangeFailed);
        }

        return this.View(model);
    }

    [HttpGet("/change-password")]
    public IActionResult ChangePassword()
    {
        var model = new ChangePasswordViewModel();
        return this.View(model);
    }

    [HttpPost("/change-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (this.ModelState.IsValid)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.RedirectToDefault();
            }

            var result = await this.userManager.ChangePasswordAsync(user, model.OldPassword!, model.NewPassword!);
            if (result.Succeeded)
            {
                await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                this.TempData["MessageText"] = @AuthenticationText.PasswordChangeSuccess;
                this.TempData["MessageVariant"] = "success";
            }

            this.ModelState.AddModelError(string.Empty, @AuthenticationText.PasswordChangeFailed);
        }

        return this.View(model);
    }
}