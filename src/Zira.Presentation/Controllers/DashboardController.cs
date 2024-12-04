using Microsoft.AspNetCore.Mvc;

namespace Zira.Presentation.Controllers;

[Route("/dashboard/")]
public class DashboardController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return this.View();
    }
}