using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zira.Presentation.Models;
using Zira.Services.Common.Constants;
using Zira.Services.Common.Contracts;
using Zira.Services.Common.Models;

namespace Zira.Presentation.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService emailService;
    private readonly ILogger<HomeController> logger;

    public HomeController(
        IEmailService emailService,
        ILogger<HomeController> logger)
    {
        this.emailService = emailService;
        this.logger = logger;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index(string strategy = EmailSenderStrategies.NoOps)
    {
        var emailSent = await this.emailService.SendEmailAsync(
            new EmailModel
            {
                Subject = "Welcome to Zira!",
                Email = "example_user@zira.dev",
                Message = $"You have received email with strategy {strategy}.",
            },
            strategy);

        return this.Ok(emailSent);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
    }
}