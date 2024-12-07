using Microsoft.AspNetCore.Mvc;

namespace Zira.Presentation.Controllers;

public class LoginController : Controller
{
    public IActionResult Index()
    {
        return this.View("Pages/Login/Index");
    }
}