using System.Collections.Generic;
using Zira.Data.Enums;
using Zira.Data.Models;

namespace Zira.Presentation.Models;

public class TransactionsListViewModel
{
    public List<Transaction> Transactions { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }

    public Categories? SelectedCategory { get; set; }
}