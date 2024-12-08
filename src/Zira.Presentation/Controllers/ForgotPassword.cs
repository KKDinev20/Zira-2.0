using Microsoft.AspNetCore.Mvc;

namespace Zira.Presentation.Controllers;

public class ForgotPassword : Controller
{
    // GET
    public IActionResult Index()
    {
        return this.View("Index");
    }
}