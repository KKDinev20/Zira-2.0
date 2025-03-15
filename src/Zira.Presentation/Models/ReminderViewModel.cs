using System;
using System.ComponentModel.DataAnnotations;
using Zira.Data.Enums;

namespace Zira.Presentation.Models
{
    public class ReminderViewModel
    {
        public Guid Id { get; set; }

        [StringLength(100)] public string? Title { get; set; }

        [Range(0.01, 100000)] public decimal Amount { get; set; }

        [Required] public DateTime DueDate { get; set; }

        public string? Remark { get; set; }
    }
}