using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Data.Models;
using Zira.Services.Identity.Constants;

namespace Zira.Presentation.Controllers;

[Route("/dashboard/")]
public class DashboardController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly EntityContext context;

    public DashboardController(EntityContext context, UserManager<ApplicationUser> userManager)
    {
        this.context = context;
        this.userManager = userManager;
    }

    [HttpGet("")]
    [Authorize(Policies.UserPolicy)]
    public async Task<IActionResult> Index()
    {
        var user = await this.userManager.GetUserAsync(this.User); 
        if (user != null)
        {
            var applicationUser = await this.context.Users
                .FirstOrDefaultAsync(u => u.ApplicationUserId == user.Id);

            this.ViewBag.UserName = applicationUser != null
                ? $"{applicationUser.FirstName} {applicationUser.LastName}"
                : "Unknown User";

            this.ViewBag.UserEmail = user.Email;

            this.ViewBag.UserAvatar = string.IsNullOrEmpty(applicationUser?.ImageUrl)
                ? "/dashboard/assets/img/avatars/default.jpg"
                : applicationUser.ImageUrl;
        }

        return this.View();
    }

}
