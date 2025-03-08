using System;
using Zira.Data.Enums;

namespace Zira.Data.Models;

public class Reminder
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Title { get; set; }
    public string? Remark { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public ReminderStatus Status { get; set; } = ReminderStatus.Pending;

    public ApplicationUser? User { get; set; }
}