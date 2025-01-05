using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zira.Services.Identity.Constants;

namespace Zira.Presentation.Controllers;

[Route("/dashboard/")]
public class DashboardController : Controller
{
    [HttpGet("")]
    [Authorize(Policies.UserPolicy)]
    public IActionResult Index()
    {
        return this.View();
    }
}
