using Microsoft.AspNetCore.Mvc.ModelBinding;
using Zira.Data.Enums;
using Zira.Data.Models;

namespace Zira.Presentation.Validations;

public static class TransactionValidator
{
    public static void ValidateTransaction(Transaction transaction, ModelStateDictionary modelState)
    {
        if (transaction.Amount <= 0)
        {
            modelState.AddModelError("Amount", "Amount must be positive.");
        }

        if (transaction.Type == TransactionType.Expense && transaction.Category == null)
        {
            modelState.AddModelError("Category", "Category is required for expenses.");
        }

        if (transaction.Type == TransactionType.Income && transaction.Source == null)
        {
            modelState.AddModelError("Source", "Source is required for incomes.");
        }
    }
}