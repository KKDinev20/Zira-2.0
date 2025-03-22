using System;
using System.ComponentModel.DataAnnotations;
using Zira.Data.Models;

namespace Zira.Presentation.Models
{
    public class SavingsGoalViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Името на целта е необходимо.")] 
        public string Name { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Целевата сума трябва да е по-голяма от 0.")]
        public decimal TargetAmount { get; set; }

        public decimal CurrentAmount { get; set; } = 0;

        [Required]
        public DateTime? TargetDate { get; set; }

        public string? Remark { get; set; }
        
        public Currency? Currency { get; set; }
        public string CurrencyCode { get; set; }
        
    }
}