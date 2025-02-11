using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Zira.Common;
using Zira.Data.Models;

namespace Zira.Presentation.Validations;

public static class IncomeValidations
{
    public static void ValidateAmount(ModelStateDictionary modelState, decimal amount)
    {
        if (amount <= 0)
        {
            modelState.AddModelError(nameof(Income.Amount), @IncomeText.AmountValidation);
        }
    }

    public static void ValidateDateReceived(ModelStateDictionary modelState, DateTime dateReceived)
    {
        if (dateReceived > DateTime.UtcNow)
        {
            modelState.AddModelError(nameof(Income.DateReceived), @IncomeText.FutureDateValidation);
        }

        if (dateReceived < DateTime.UtcNow.AddYears(-10))
        {
            modelState.AddModelError(nameof(Income.DateReceived), @IncomeText.PastDateValidation);
        }
    }
}