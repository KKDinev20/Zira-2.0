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
    
    
    [HttpPost("/budget-alert")]
    public async Task<IActionResult> BudgetAlert()
    {
        var emailModel = new EmailModel
        {
            ToEmail = "konstantindinv@gmail.com",
            Subject = "Budget Alert",
            Body = @"
                <h1>Warning!</h1>
                <p>You seem to be exceeding your monthly budget limit.</p>
                <p>Consider changing your limit in the settings.</p>
            ",
        };

        await this.emailService.SendEmailAsync(emailModel);

        return this.Ok("Email Sent Successfully");
    }
}