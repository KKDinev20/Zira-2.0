using System;
using System.ComponentModel.DataAnnotations;
using Zira.Data.Enums;

namespace Zira.Presentation.Models
{
    public class BudgetViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Month is required.")]
        public DateTime Month { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Category is required.")]
        public Categories? Category { get; set; }

        [Required(ErrorMessage = "Threshold is required.")]
        [Range(10, 100, ErrorMessage = "Warning threshold must be between 0 and 100.")]
        public decimal WarningThreshold { get; set; }

        public string? Remark { get; set; }
    }
}