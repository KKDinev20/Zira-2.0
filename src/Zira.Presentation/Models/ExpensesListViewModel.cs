using System.Collections.Generic;
using Zira.Data.Models;

namespace Zira.Presentation.Models;

public class ExpensesListViewModel
{
    public List<Expense> Expenses { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
