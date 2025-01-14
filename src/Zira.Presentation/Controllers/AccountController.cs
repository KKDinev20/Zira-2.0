using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

    public AccountController(
        UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager;
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
                    return this.RedirectToLogin();
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
                return this.RedirectToLogin();
            }

            this.ModelState.AddModelError(string.Empty, @AuthenticationText.PasswordChangeFailed);
        }

        return this.View(model);
    }
}