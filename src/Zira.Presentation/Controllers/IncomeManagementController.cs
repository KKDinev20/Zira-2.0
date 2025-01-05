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
}