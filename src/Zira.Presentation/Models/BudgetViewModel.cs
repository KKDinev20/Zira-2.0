using System;
using System.ComponentModel.DataAnnotations;
using Zira.Data.Enums;

namespace Zira.Presentation.Models
{
    public class BudgetViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Сумата е необходима.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Сумата трябва да е по-голяма от 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Месецът е задължително поле.")]
        public DateTime Month { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Категорията на бюджета е задължително поле.")]
        public Categories? Category { get; set; }

        [Required(ErrorMessage = "Прагът на бюджета е задължително поле.")]
        [Range(10, 100, ErrorMessage = "Прагът на бюджета трябва да е между 0 и 100.")]
        public decimal WarningThreshold { get; set; }

        public string? Remark { get; set; }

        [Required]
        public string CurrencyCode { get; set; } = "BGN";
    }
}