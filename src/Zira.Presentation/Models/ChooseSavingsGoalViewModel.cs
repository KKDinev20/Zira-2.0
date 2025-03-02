using System;
using System.Collections.Generic;
using Zira.Data.Models;

namespace Zira.Presentation.Models;

public class ChooseSavingsGoalViewModel
{
    public Guid TransactionId { get; set; }
    public decimal AmountToSetAside { get; set; }

    public List<SavingsGoal> SavingsGoals { get; set; }
}