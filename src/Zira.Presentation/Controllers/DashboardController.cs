using Microsoft.AspNetCore.Mvc;

namespace Zira.Presentation.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return this.View("Pages/Dashboard/Index");
    }
}