using System.Collections.Generic;
using Zira.Data.Models;

namespace Zira.Presentation.Models;

public class SavingsGoalListViewModel
{
    public List<SavingsGoal> Goals { get; set; }

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}