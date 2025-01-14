using System;
using Zira.Data.Enums;
using Zira.Data.Models;

namespace Zira.Data;

public class Expense
{
    public Guid ExpenseId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public Categories Category { get; set; }
    public string? Description { get; set; }
    public DateTime DateSpent { get; set; }

    public ApplicationUser? User { get; set; }
}