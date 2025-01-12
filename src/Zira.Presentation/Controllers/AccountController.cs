using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zira.Data;
using Zira.Presentation.Extensions;
using Zira.Presentation.Models;

namespace Zira.Presentation.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly ILogger<AuthenticationController> logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AuthenticationController> logger)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.logger = logger;
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
                    await this.signInManager.RefreshSignInAsync(user);
                    this.TempData["MessageText"] = "Your email and username have been changed successfully.";
                    this.TempData["MessageVariant"] = "success";
                    return this.RedirectToLogin();
                }
                else
                {
                    await this.userManager.ChangeEmailAsync(user, user.Email!, token);
                    this.ModelState.AddModelError(string.Empty, "Failed to update username. Please try again.");
                }
            }

            this.ModelState.AddModelError(string.Empty, "Failed to change email. Please try again.");
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
                await this.signInManager.RefreshSignInAsync(user);
                this.TempData["MessageText"] = "Your password has been changed successfully.";
                this.TempData["MessageVariant"] = "success";
                return this.RedirectToLogin();
            }

            this.ModelState.AddModelError(string.Empty, "Failed to change password. Please try again.");
        }

        return this.View(model);
    }
}