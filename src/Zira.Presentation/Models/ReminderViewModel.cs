using System;
using System.ComponentModel.DataAnnotations;
using Zira.Data.Enums;
using Zira.Data.Models;

namespace Zira.Presentation.Models
{
    public class ReminderViewModel
    {
        public Guid Id { get; set; }

        [StringLength(100)] public string? Title { get; set; }

        [Range(0.01, 100000)] public decimal Amount { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime DueDate { get; set; }

        public string? Remark { get; set; }

        public string? CurrencyCode { get; set; }
        public Currency? Currency { get; set; }
    }
}