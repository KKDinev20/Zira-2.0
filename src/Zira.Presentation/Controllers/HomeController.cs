using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Zira.Data;
using Zira.Data.Models;
using Zira.Services.Common.Contracts;
using Zira.Services.Common.Models;
using Zira.Services.Identity.Constants;
using Zira.Services.Identity.Contracts;

namespace Zira.Presentation.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService emailService;
    private readonly ICurrentUser currentUser;
    private readonly UserManager<ApplicationUser> userManager;

    public HomeController(
        IEmailService emailService,
        ICurrentUser currentUser,
        UserManager<ApplicationUser> userManager)
    {
        this.emailService = emailService;
        this.currentUser = currentUser;
        this.userManager = userManager;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index()
    {
        return this.View();
    }

    [HttpPost("/set-language")]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        if (culture is not ("en" or "bg"))
        {
            culture = "en";
        }

        this.Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
            });

        return this.LocalRedirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
    }
}