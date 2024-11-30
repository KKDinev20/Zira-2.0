using System;

namespace Zira.Data.Models;

public class Reminder
{
    public Guid ReminderId { get; set; }
    public Guid UserId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }

    public ApplicationUser? User { get; set; }
}