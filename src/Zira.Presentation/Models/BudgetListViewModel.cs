using System.Collections.Generic;
using Zira.Data.Models;

namespace Zira.Presentation.Models;

public class BudgetListViewModel
{
    public List<Budget> Budgets { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}