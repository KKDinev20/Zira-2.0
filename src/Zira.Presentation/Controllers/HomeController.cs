using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zira.Data;
using Zira.Data.Models;
using Zira.Services.Common.Contracts;
using Zira.Services.Common.Models;
using Zira.Services.Identity.Constants;
using Zira.Services.Identity.Contracts;

namespace Zira.Presentation.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService emailService;
    private readonly ICurrentUser currentUser;
    private readonly UserManager<ApplicationUser> userManager;

    public HomeController(
        IEmailService emailService,
        ICurrentUser currentUser,
        UserManager<ApplicationUser> userManager)
    {
        this.emailService = emailService;
        this.currentUser = currentUser;
        this.userManager = userManager;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index()
    {
        return this.View();
    }
}