using System;
using System.ComponentModel.DataAnnotations;
using Zira.Data.Enums;

namespace Zira.Presentation.Models
{
    public class ReminderViewModel
    {
        public Guid Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; }

        [Required, Range(0.01, 100000)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public string? Remark { get; set; }

        public bool IsRecurring { get; set; }
        public ReminderFrequency Frequency { get; set; }
        public NotificationType NotificationMethod { get; set; }
    }
}