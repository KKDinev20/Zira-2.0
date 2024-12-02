using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zira.Services.Common.Contracts;
using Zira.Services.Common.Models;

namespace Zira.Presentation.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService emailService;

    public HomeController(IEmailService emailService)
    {
        this.emailService = emailService;
    }

    public async Task<IActionResult> SendConfirmationEmail()
    {
        var emailModel = new EmailModel
        {
            ToEmail = "konstantindinv@gmail.com",
            Subject = "Confirmation Email",
            Body = "<h1>Thank you for registering!</h1><p>Your registration is complete.</p>",
        };

        await this.emailService.SendEmailAsync(emailModel);

        return this.Ok("Email Sent Successfully");
    }
}