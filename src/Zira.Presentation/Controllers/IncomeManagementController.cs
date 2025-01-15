using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zira.Data;
using Zira.Data.Enums;
using Zira.Services.Identity.Constants;
using Zira.Services.Identity.Extensions;

namespace Zira.Presentation.Controllers;

[Authorize(Policies.UserPolicy)]
public class IncomeManagementController : Controller
{
    private readonly EntityContext context;

    public IncomeManagementController(EntityContext context)
    {
        this.context = context;
    }

    [HttpGet("/add-income/")]
    public IActionResult AddIncome()
    {
        this.ViewBag.Sources = Enum.GetValues(typeof(Sources)).Cast<Sources>().ToList();
        return this.View();
    }

    [HttpPost("/add-income/")]
    public async Task<IActionResult> AddIncome(Income incomeModel)
    {
        if (this.ModelState.IsValid)
        {
            var userId = this.User.GetUserId();

            var user = await this.context.Users
                .FirstOrDefaultAsync(u => u.ApplicationUserId == userId);

            if (user == null)
            {
                this.ModelState.AddModelError("", "User does not exist.");
                return this.View(incomeModel);
            }

            incomeModel.IncomeId = Guid.NewGuid();
            incomeModel.UserId = user.Id;

            if (incomeModel.DateReceived == default)
            {
                incomeModel.DateReceived = DateTime.UtcNow;
            }

            this.context.Incomes.Add(incomeModel);
            await this.context.SaveChangesAsync();

            return this.RedirectToAction("IncomeList");
        }

        this.ViewBag.Sources = Enum.GetValues(typeof(Sources)).Cast<Sources>().ToList();
        return this.View(incomeModel);
    }

    [HttpGet("/income-list/")]
    public async Task<IActionResult> IncomeList()
    {
        var userId = this.User.GetUserId();

        var user = await this.context.Users
            .FirstOrDefaultAsync(u => u.ApplicationUserId == userId);

        if (user == null)
        {
            return this.NotFound("User not found.");
        }

        var incomes = await this.context.Incomes
            .Where(i => i.UserId == user.Id)
            .ToListAsync();

        return this.View(incomes);
    }

    [HttpGet("/edit-income/{id}")]
    public async Task<IActionResult> EditIncome(Guid id)
    {
        var income = await this.context.Incomes
            .Include(i => i.User)
            .FirstOrDefaultAsync(
                i => i.IncomeId == id && i.User.ApplicationUserId == this.User.GetUserId());

        if (income == null)
        {
            return this.NotFound();
        }

        this.ViewBag.Sources = Enum.GetValues(typeof(Sources)).Cast<Sources>().ToList();

        return this.View(income);
    }

    [HttpPost("/edit-income/{id}")]
    public async Task<IActionResult> EditIncome(Guid id, Income model)
    {
        if (id != model.IncomeId)
        {
            return this.BadRequest();
        }

        if (this.ModelState.IsValid)
        {
            var income = await this.context.Incomes
                .Include(i => i.User)
                .FirstOrDefaultAsync(
                    i => i.IncomeId == id && i.User.ApplicationUserId == this.User.GetUserId());

            if (income == null)
            {
                return this.NotFound();
            }

            income.Source = model.Source;
            income.Amount = model.Amount;
            income.DateReceived = model.DateReceived;

            await this.context.SaveChangesAsync();

            return this.RedirectToAction("IncomeList");
        }

        this.ViewBag.Sources = Enum.GetValues(typeof(Sources)).Cast<Sources>().ToList();
        return this.View(model);
    }

    [HttpGet("/delete-income/{id}")]
    public async Task<IActionResult> DeleteIncome(Guid id)
    {
        var income = await this.context.Incomes
            .Include(i => i.User)
            .FirstOrDefaultAsync(
                i => i.IncomeId == id && i.User.ApplicationUserId == this.User.GetUserId());

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
            .FirstOrDefaultAsync(
                i => i.IncomeId == id && i.User.ApplicationUserId == this.User.GetUserId());

        if (income == null)
        {
            return this.NotFound();
        }

        this.context.Incomes.Remove(income);
        await this.context.SaveChangesAsync();

        return this.RedirectToAction("IncomeList");
    }
}