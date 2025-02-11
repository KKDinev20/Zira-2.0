using System.Collections.Generic;
using Zira.Data.Models;

namespace Zira.Presentation.Models;

public class IncomesListViewModel
{
    public List<Income> Incomes { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}