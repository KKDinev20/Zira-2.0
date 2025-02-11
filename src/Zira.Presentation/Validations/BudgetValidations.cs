using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Zira.Common;
using Zira.Data.Enums;
using Zira.Data.Models;

namespace Zira.Presentation.Validations;

public class BudgetValidations
{
    public static void ValidateAmount(ModelStateDictionary modelState, decimal amount)
    {
        if (amount < 0)
        {
            modelState.AddModelError(nameof(Budget.Amount), @BudgetText.AmountNegative);
        }
    }

    public static void ValidateMonth(ModelStateDictionary modelState, DateTime month)
    {
        if (month > DateTime.UtcNow)
        {
            modelState.AddModelError(nameof(Budget.Month), @BudgetText.MonthFuture);
        }
    }
    
}