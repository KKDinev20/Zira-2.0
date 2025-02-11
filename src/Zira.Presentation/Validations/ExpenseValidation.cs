using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Zira.Common;
using Zira.Data.Models;

namespace Zira.Presentation.Validations
{
    public static class ExpenseValidations
    {
        public static void ValidateExpense(Expense expense, ModelStateDictionary modelState)
        {
            if (expense.Amount <= 0)
            {
                modelState.AddModelError("Amount", @ExpensesText.AmountValidation);
            }

            if (expense.DateSpent > DateTime.UtcNow)
            {
                modelState.AddModelError("DateSpent", @ExpensesText.DateValidation);
            }
        }
    }
}