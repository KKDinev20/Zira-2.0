using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zira.Common;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Data.Models;
using Zira.Presentation.Extensions;
using Zira.Presentation.Models;
using Zira.Presentation.Validations;
using Zira.Services.Identity.Constants;
using Zira.Services.Identity.Extensions;

namespace Zira.Presentation.Controllers;

[Authorize(Policies.UserPolicy)]
public class IncomesController : Controller
{
    private readonly EntityContext context;
    private readonly UserManager<ApplicationUser> userManager;

    public IncomesController(EntityContext context, UserManager<ApplicationUser> userManager)
    {
        this.context = context;
        this.userManager = userManager;
    }

    [HttpGet("/add-income/")]
    public async Task<IActionResult> AddIncome()
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.context);
        this.ViewBag.Sources = Enum.GetValues(typeof(Sources)).Cast<Sources>().ToList();
        return this.View();
    }

    [HttpPost("/add-income/")]
    public async Task<IActionResult> AddIncome(Income incomeModel)
    {
        IncomeValidations.ValidateAmount(this.ModelState, incomeModel.Amount);
        IncomeValidations.ValidateDateReceived(this.ModelState, incomeModel.DateReceived);

        if (!this.ModelState.IsValid)
        {
            this.TempData["IncomeErrorMessage"] = @IncomeText.IncomeError;
            this.ViewBag.Sources = Enum.GetValues(typeof(Sources)).Cast<Sources>().ToList();
            return this.RedirectToAction("IncomeList");
        }

        var userId = this.User.GetUserId();
        var user = await this.context.Users.FirstOrDefaultAsync(u => u.ApplicationUserId == userId);

        if (user == null)
        {
            this.ModelState.AddModelError("", @AuthenticationText.UserNotExisting);
            this.TempData["IncomeErrorMessage"] = @IncomeText.UserNotFound;
            this.ViewBag.Sources = Enum.GetValues(typeof(Sources)).Cast<Sources>().ToList();
            return this.RedirectToAction("IncomeList");
        }

        incomeModel.IncomeId = Guid.NewGuid();
        incomeModel.UserId = user.Id;
        incomeModel.DateReceived = incomeModel.DateReceived == default ? DateTime.UtcNow : incomeModel.DateReceived;

        this.context.Incomes.Add(incomeModel);
        await this.context.SaveChangesAsync();

        this.TempData["IncomeSuccessMessage"] = @IncomeText.UserSuccess;
        return this.RedirectToAction("IncomeList");
    }

    [HttpGet("/income-list/")]
    public async Task<IActionResult> IncomeList(int page = 1, int pageSize = 5)
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.context);

        var userId = this.User.GetUserId();
        var user = await this.context.Users
            .FirstOrDefaultAsync(u => u.ApplicationUserId == userId);

        if (user == null)
        {
            return this.NotFound("User not found.");
        }

        var totalRecords = await this.context.Incomes
            .Where(i => i.UserId == user.Id)
            .CountAsync();

        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        var incomes = await this.context.Incomes
            .Where(i => i.UserId == user.Id)
            .OrderBy(i => i.DateReceived)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var model = new IncomesListViewModel
        {
            Incomes = incomes,
            CurrentPage = page,
            TotalPages = totalPages,
        };

        return this.View(model);
    }

    [HttpGet("/edit-income/{id}")]
    public async Task<IActionResult> EditIncome(Guid id)
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.context);
        var income = await this.context.Incomes
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.IncomeId == id && i.User.ApplicationUserId == this.User.GetUserId());

        if (income == null)
        {
            return this.NotFound();
        }

        this.ViewBag.Sources = Enum.GetValues(typeof(Sources)).Cast<Sources>().ToList();

        return this.View(income);
    }

    [HttpPost("/edit-income/{id}")]
    public async Task<IActionResult> EditIncome(Guid id, Income incomeModel)
    {
        if (id != incomeModel.IncomeId)
        {
            return this.BadRequest();
        }

        IncomeValidations.ValidateAmount(this.ModelState, incomeModel.Amount);
        IncomeValidations.ValidateDateReceived(this.ModelState, incomeModel.DateReceived);

        if (!this.ModelState.IsValid)
        {
            this.TempData["ErrorMessage"] = @IncomeText.IncomeError;
            this.ViewBag.Sources = Enum.GetValues(typeof(Sources)).Cast<Sources>().ToList();
            return this.RedirectToAction("IncomeList");
        }

        var income = await this.context.Incomes
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.IncomeId == id && i.User.ApplicationUserId == this.User.GetUserId());

        if (income == null)
        {
            this.TempData["ErrorMessage"] = @IncomeText.IncomeNotFound;
            return this.RedirectToAction("IncomeList");
        }

        income.Source = incomeModel.Source;
        income.Amount = incomeModel.Amount;
        income.DateReceived = incomeModel.DateReceived;

        await this.context.SaveChangesAsync();
        this.TempData["SuccessMessage"] = @IncomeText.IncomeSuccess;
        return this.RedirectToAction("IncomeList");
    }

    [HttpGet("/delete-income/{id}")]
    public async Task<IActionResult> DeleteIncome(Guid id)
    {
        await this.SetGlobalUserInfoAsync(this.userManager, this.context);
        var income = await this.context.Incomes
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.IncomeId == id && i.User.ApplicationUserId == this.User.GetUserId());

        if (income == null)
        {
            return this.NotFound();
        }

        return this.View(income);
    }

    [HttpPost("/delete-income/{id}")]
    public async Task<IActionResult> DeleteIncomeAsync(Guid id)
    {
        var income = await this.context.Incomes
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.IncomeId == id && i.User.ApplicationUserId == this.User.GetUserId());

        if (income == null)
        {
            return this.NotFound();
        }

        this.context.Incomes.Remove(income);
        await this.context.SaveChangesAsync();

        return this.RedirectToAction("IncomeList");
    }
}