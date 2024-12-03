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