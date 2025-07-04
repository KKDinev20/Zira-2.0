﻿using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Essentials.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zira.Common;
using Zira.Data;
using Zira.Data.Models;
using Zira.Presentation.Extensions;
using Zira.Presentation.Models;
using Zira.Services.Common.Contracts;
using Zira.Services.Identity.Extensions;

namespace Zira.Presentation.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailService emailService;
        private readonly EntityContext context;
        private readonly ILogger<AuthenticationController> logger;

        public AuthenticationController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            ILogger<AuthenticationController> logger,
            EntityContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
            this.logger = logger;
            this.context = context;
        }

        [HttpGet("/login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (this.IsUserAuthenticated())
            {
                return this.RedirectToDefault();
            }

            var model = new LoginViewModel();
            return this.View(model);
        }

        [HttpPost("/login")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Find user and check application user for valid data then Sign in successfully; If AU data null send to CompleteProfile
            if (this.IsUserAuthenticated())
            {
                return this.RedirectToDefault();
            }

            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.FindByEmailAsync(model.Email!);
                if (user == null || !(await this.userManager.CheckPasswordAsync(user, model.Password!)))
                {
                    this.ModelState.AddModelError(string.Empty, AuthenticationText.InvalidLoginErrorMessage);
                    return this.View(model);
                }

                if (await this.userManager.IsLockedOutAsync(user))
                {
                    this.ModelState.AddModelError(string.Empty, AuthenticationText.UserLockedOutErrorMessage);
                    return this.View(model);
                }

                var applicationUser = await this.context.Users
                    .FirstOrDefaultAsync(u => u.Id == user.Id);

                if (applicationUser != null &&
                    (string.IsNullOrEmpty(applicationUser.FirstName) ||
                     string.IsNullOrEmpty(applicationUser.LastName) ||
                     applicationUser.Birthday == DateTime.MinValue))
                {
                    await this.SignInAsync(user, model.RememberAccess);
                    return this.RedirectToAction("CompleteProfile", "Account");
                }

                await this.SignInAsync(user, model.RememberAccess);
                return this.RedirectToDefault();
            }

            return this.View(model);
        }

        [HttpGet("/register")]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (this.IsUserAuthenticated())
            {
                return this.RedirectToDefault();
            }

            var model = new RegisterViewModel();
            return this.View(model);
        }

        // Create new application user and create a user with userManager
        [HttpPost("/register")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (this.IsUserAuthenticated())
            {
                return this.RedirectToDefault();
            }

            if (this.ModelState.IsValid)
            {
                var applicationUser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = null,
                    LastName = null,
                    Birthday = DateTime.MinValue,
                    ImageUrl = null,
                };

                var result = await this.userManager.CreateAsync(applicationUser, model.Password!);

                if (result.Succeeded)
                {
                    await this.context.SaveChangesAsync();
                    this.TempData["MessageText"] = AuthenticationText.RegisterSuccessMessage;
                    this.TempData["MessageVariant"] = "success";
                    return this.RedirectToAction("Login");
                }

                this.ModelState.AssignIdentityErrors(result.Errors);
            }

            return this.View(model);
        }

        [HttpGet("/forgot-password")]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            if (this.IsUserAuthenticated())
            {
                return this.RedirectToDefault();
            }

            var model = new ForgotPasswordViewModel();
            return this.View(model);
        }

        // Set tokens and url for the reset password action
        [HttpPost("/forgot-password")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (this.IsUserAuthenticated())
            {
                return this.RedirectToDefault();
            }

            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.FindByEmailAsync(model.Email!);
                if (user != null)
                {
                    var resetPasswordToken = await this.userManager.GeneratePasswordResetTokenAsync(user);
                    var resetPasswordEncodedToken = UrlEncoder.Default.Encode(resetPasswordToken);
                    var resetPasswordUrl = this.HttpContext
                        .GetAbsoluteRoute($"/reset-password?email={user.Email}&token={resetPasswordEncodedToken}");

                    var result = await this.emailService.SendResetPasswordEmailAsync(
                        user.Email!,
                        resetPasswordUrl);

                    this.logger.LogInformation("Reset password email send result {Result}", result);
                }

                this.TempData["MessageText"] = AuthenticationText.ForgotPasswordSuccessMessage;
                this.TempData["MessageVariant"] = "success";

                return this.RedirectToAction(nameof(this.Login));
            }

            return this.View(model);
        }

        [AllowAnonymous]
        [HttpGet("/reset-password")]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            if (this.IsUserAuthenticated())
            {
                return this.RedirectToDefault();
            }

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                return this.NotFound();
            }

            var user = await this.userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return this.NotFound();
            }

            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email,
            };

            return this.View(model);
        }

        // Find user and reset its password using userManage(user, token, password of the model)
        [HttpPost("/reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (this.IsUserAuthenticated())
            {
                return this.RedirectToDefault();
            }

            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.FindByEmailAsync(model.Email!);
                if (user == null)
                {
                    return this.NotFound();
                }

                var result = await this.userManager.ResetPasswordAsync(user, model.Token!, model.Password!);
                if (result.Succeeded)
                {
                    return this.RedirectToAction(nameof(this.Login));
                }

                this.ModelState.AssignIdentityErrors(result.Errors);
            }

            return this.View(model);
        }

        [HttpGet("/access-denied")]
        public IActionResult AccessDenied()
        {
            return this.View();
        }

        [HttpPost("/logout")]
        public async Task<IActionResult> Logout()
        {
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.RedirectToAction(nameof(this.Login));
        }

        // Use ClaimsFactory to make a user and set CookieAuthenticationDefaults.AuthenticationScheme
        private async Task SignInAsync(
            ApplicationUser user,
            bool rememberMe)
        {
            var claimsPrinciple = await this.signInManager
                .ClaimsFactory
                .CreateAsync(user);

            await this.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrinciple,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                    IsPersistent = rememberMe,
                });
        }
    }
}