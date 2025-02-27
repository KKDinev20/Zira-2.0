using System;
using System.Collections.Generic;
using Zira.Data.Enums;
using Zira.Data.Models;

namespace Zira.Presentation.Models;

public class TransactionViewModel
{
    public Guid Id { get; set; }
    public Guid? SelectedSavingsGoalId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public Categories? Category { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public decimal? SavingsGoalAmount { get; set; }

    public List<SavingsGoal> AvailableSavingsGoals { get; set; } = new();
}