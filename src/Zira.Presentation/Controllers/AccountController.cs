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

        var viewModel = new ProfileViewModel
        {
            FirstName = user!.FirstName,
            LastName = user!.LastName,
            Email = user!.Email,
            BirthDate = user.Birthday,
            AvatarUrl = string.IsNullOrEmpty(user.ImageUrl)
                ? "/dashboard/assets/img/avatars/default.jpg"
                : user.ImageUrl,
        };

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
        bool resetAvatar = false)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Authentication");
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = email;
        user.NormalizedEmail = email.ToUpper();
        user.Birthday = birthDate;

        // Handle Avatar Reset
        if (resetAvatar)
        {
            user.ImageUrl = "/dashboard/assets/img/avatars/default.jpg";
        }
        // Handle Avatar Upload
        else if (avatarFile != null && avatarFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(avatarFile.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("AvatarFile", "Invalid file type. Allowed: JPG, PNG, GIF.");
                return View("Profile");
            }

            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var uploadPath = Path.Combine(webHostEnvironment.WebRootPath, "dashboard/assets/img/avatars", uniqueFileName);

            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await avatarFile.CopyToAsync(stream);
            }

            user.ImageUrl = $"/dashboard/assets/img/avatars/{uniqueFileName}";
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Failed to update profile.");
            return View("Profile");
        }

        TempData["SuccessMessage"] = "Profile updated successfully!";
        return RedirectToAction("Profile");
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

                this.context.Users.Update(applicationUser);
                await this.context.SaveChangesAsync();

                return this.RedirectToDefault();
            }
        }

        return this.View(model);
    }
}