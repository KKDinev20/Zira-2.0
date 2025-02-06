using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Data.Models;

namespace Zira.Presentation.Extensions;

public static class ControllerExtensions
{
    public static bool IsUserAuthenticated(this Controller controller) =>
        controller.User?.Identity?.IsAuthenticated ?? false;

    public static IActionResult RedirectToDefault(this Controller controller) =>
        controller.RedirectToAction("Index", "Dashboard");

    public static IActionResult RedirectToProfile(this Controller controller) =>
        controller.RedirectToAction("CompleteProfile", "Account");

    public static IActionResult RedirectToLogin(this Controller controller) =>
        controller.RedirectToAction("Login", "Authentication");

    public static async Task SetGlobalUserInfoAsync(
        this Controller controller,
        UserManager<ApplicationUser> userManager,
        EntityContext context)
    {
        var user = await userManager.GetUserAsync(controller.User);
        if (user != null)
        {
            var applicationUser = await context.Users
                .FirstOrDefaultAsync(u => u.ApplicationUserId == user.Id);

            controller.ViewBag.UserName = applicationUser != null
                ? $"{applicationUser.FirstName} {applicationUser.LastName}"
                : "Unknown User";

            controller.ViewBag.FirstName = applicationUser.FirstName;
            controller.ViewBag.LastName = applicationUser.LastName;
            controller.ViewBag.BirthDate = applicationUser.Birthday;

            controller.ViewBag.UserEmail = user.Email;

            controller.ViewBag.UserAvatar = string.IsNullOrEmpty(applicationUser?.ImageUrl)
                ? "/dashboard/assets/img/avatars/default.jpg"
                : applicationUser.ImageUrl;
        }
    }
}