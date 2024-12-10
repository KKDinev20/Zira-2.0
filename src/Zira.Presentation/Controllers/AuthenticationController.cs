using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zira.Data;
using Zira.Presentation.Models;

namespace EduSystem.Presentation.Controllers;

public class AuthenticationController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;

    public AuthenticationController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    [HttpGet("/login")]
    public IActionResult Login()
    {
        var model = new LoginViewModel();
        return this.View(model);
    }

    [HttpPost("/login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (this.ModelState.IsValid)
        {
            var result = await this.signInManager.PasswordSignInAsync(
                model.Email!,
                model.Password!,
                model.RememberAccess,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                this.TempData["MessageText"] = "Login successful!";
                this.TempData["MessageVariant"] = "success";
                return this.RedirectToAction("Index", "Home");
            }

            this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return this.View(model);
    }

    [HttpGet("/register")]
    public IActionResult Register()
    {
        var model = new RegisterViewModel();
        return this.View(model);
    }

    [HttpPost("/register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (this.ModelState.IsValid)
        {
            var result = await this.userManager.CreateAsync(
                new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                },
                model.Password!);

            if (result.Succeeded)
            {
                this.TempData["MessageText"] = "User created a new account with password.";
                this.TempData["MessageVariant"] = "success";
                return this.RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return this.View(model);
    }

    [HttpGet("/forgot-password")]
    public IActionResult ForgotPassword()
    {
        var model = new ForgotPasswordViewModel();
        return this.View(model);
    }

    [HttpPost("/forgot-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (this.ModelState.IsValid)
        {
            var user = await this.userManager.FindByEmailAsync(model.Email!);
            if (user == null || !(await this.userManager.IsEmailConfirmedAsync(user)))
            {
                this.TempData["MessageText"] = "If the email exists, a password reset link has been sent.";
                this.TempData["MessageVariant"] = "info";
                return this.RedirectToAction("ForgotPassword");
            }

            var token = await this.userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = this.Url.Action(
                "ResetPassword",
                "Authentication",
                new { token, email = model.Email },
                this.Request.Scheme);

            // TODO: Send the email containing `resetLink`.
            this.TempData["MessageText"] = "Password reset link sent. Check your email.";
            this.TempData["MessageVariant"] = "success";
            return this.RedirectToAction("ForgotPassword");
        }

        return this.View(model);
    }
}
