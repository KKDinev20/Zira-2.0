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
    private readonly IEmailService _emailService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IEmailService emailService,
        ILogger<HomeController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index(string strategy = EmailSenderStrategies.NoOps)
    {
        var emailSent = await _emailService.SendEmailAsync(
            new EmailModel
            {
                Subject = "Welcome to EduSystem!",
                Email = "example_user@edusystem.dev",
                Message = $"You have received email with strategy {strategy}."
            },
            strategy);
        
        return Ok(emailSent);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}