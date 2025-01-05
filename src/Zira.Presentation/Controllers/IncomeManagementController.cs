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
        this.ViewBag.Sources = Enum.GetValues(typeof(Sources));
        return this.View();
    }

    [HttpPost("/add-income/")]
    public async Task<IActionResult> AddIncome(Income model)
    {
        if (this.ModelState.IsValid)
        {
            model.IncomeId = Guid.NewGuid();
            model.UserId = this.User.GetUserId();

            if (model.DateReceived == default)
            {
                model.DateReceived = DateTime.Now;
            }

            this.context.Incomes.Add(model);
            await this.context.SaveChangesAsync();

            return this.RedirectToAction("IncomeList");
        }

        this.ViewBag.Sources = Enum.GetValues(typeof(Sources));
        return this.View(model);
    }

    [HttpGet("/income-list/")]
    public async Task<IActionResult> IncomeList()
    {
        var userId = this.User.GetUserId();
        var incomes = await this.context.Incomes
            .Where(i => i.UserId == userId)
            .ToListAsync();

        return this.View(incomes);
    }

    public async Task<bool> DeleteIncome(Guid id)
    {
        var user = await this.context.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }

        this.context.Users.Remove(user);
        await this.context.SaveChangesAsync();
        return true;
    }

    [HttpGet("/edit-income/{id}")]
    public async Task<IActionResult> EditIncome(Guid id)
    {
        var income = await this.context.Incomes
            .FirstOrDefaultAsync(i => i.IncomeId == id && i.UserId == this.User.GetUserId());

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
                .FirstOrDefaultAsync(i => i.IncomeId == id && i.UserId == this.User.GetUserId());

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

        this.ViewBag.Sources = Enum.GetValues(typeof(Sources));
        return this.View(model);
    }
}