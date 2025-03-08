using System;
using System.ComponentModel.DataAnnotations;

namespace Zira.Presentation.Models
{
    public class SavingsGoalViewModel
    {
        public Guid Id { get; set; }

        [Required] public string Name { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Target amount must be greater than zero.")]
        public decimal TargetAmount { get; set; }

        public decimal CurrentAmount { get; set; } = 0;

        public DateTime? TargetDate { get; set; }

        public string? Remark { get; set; }
    }
}