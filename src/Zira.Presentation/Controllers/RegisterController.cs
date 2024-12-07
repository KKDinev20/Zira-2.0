using Microsoft.AspNetCore.Mvc;

namespace Zira.Presentation.Controllers;

public class RegisterController : Controller
{
    // GET
    public IActionResult Index()
    {
        return this.View("Pages/Register/Index");
    }
}