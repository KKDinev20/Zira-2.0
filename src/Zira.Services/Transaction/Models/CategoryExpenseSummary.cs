using Zira.Data.Enums;

namespace Zira.Services.Transaction.Models;

public class CategoryExpenseSummary
{
    public Categories Category { get; set; }
    public decimal TotalAmount { get; set; }
}
