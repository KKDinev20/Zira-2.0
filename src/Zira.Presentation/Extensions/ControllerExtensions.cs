using Microsoft.AspNetCore.Mvc;

namespace Zira.Presentation.Extensions;

public static class ControllerExtensions
{
    public static bool IsUserAuthenticated(this Controller controller) =>
        controller.User?.Identity?.IsAuthenticated ?? false;

    public static IActionResult RedirectToDefault(this Controller controller) =>
        controller.RedirectToAction("Index", "Dashboard");

    public static IActionResult RedirectToProfile(this Controller controller) =>
        controller.RedirectToAction("CompleteProfile", "Account");
}