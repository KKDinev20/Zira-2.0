using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Zira.Common;
using Zira.Data;

namespace Zira.Presentation.Validations
{
    public static class ExpenseValidations
    {
        public static void ValidateExpense(Expense expense, ModelStateDictionary modelState)
        {
            if (expense.Amount <= 0)
            {
                modelState.AddModelError(nameof(expense.Amount), @ExpensesText.AmountValidation);
            }

            if (expense.DateSpent > DateTime.UtcNow)
            {
                modelState.AddModelError(nameof(expense.DateSpent), @ExpensesText.DateValidation);
            }
        }
    }
}