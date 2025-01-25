using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Data.Models;
using Zira.Presentation.Extensions;
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
        await this.SetGlobalUserInfoAsync(userManager, context);


        return this.View();
    }

}
