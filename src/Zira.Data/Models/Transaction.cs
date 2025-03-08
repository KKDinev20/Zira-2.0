using System;
using Zira.Data.Enums;

namespace Zira.Data.Models;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public Categories? Category { get; set; }
    public Sources? Source { get; set; }
    public string? Remark { get; set; }
    public string? Reference { get; set; }
    public DateTime Date { get; set; }
    public bool IsRecurring { get; set; } = false;
    public RecurrenceType? Recurrence { get; set; }

    public ApplicationUser? User { get; set; }
}