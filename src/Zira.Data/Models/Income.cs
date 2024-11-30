using System;
using Zira.Data.Enums;

namespace Zira.Data;

public class Income
{
    public Guid IncomeId { get; set; } 
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public Sources Source { get; set; } 
    public DateTime DateReceived { get; set; }

    public ApplicationUser User { get; set; }
}