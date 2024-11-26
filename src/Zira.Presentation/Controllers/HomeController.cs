using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Zira.Presentation.Models;
using Zira.Services.Common.Constants;
using Zira.Services.Common.Contracts;
using Microsoft.AspNetCore.Identity;
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
        var emailModel = new EmailModel
        {
            Subject = "Welcome to Zira!",
            Email = "recipient@zira.com",
            Message = $"This email was sent using the '{strategy}' strategy."
        };

        var result = await _emailService.SendEmailAsync(emailModel, strategy);
        return Ok(result);

    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}