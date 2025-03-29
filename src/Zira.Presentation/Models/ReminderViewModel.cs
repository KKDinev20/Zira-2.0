using System;
using System.ComponentModel.DataAnnotations;
using Zira.Data.Enums;
using Zira.Data.Models;

namespace Zira.Presentation.Models
{
    public class ReminderViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Името на напомнянето е необходимо.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Сумата е необходима.")]
        [Range(1, double.MaxValue, ErrorMessage = "Сумата трябва да е по-голяма от 0.")]
        public decimal Amount { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime DueDate { get; set; }

        public string? Remark { get; set; }

        public string? CurrencyCode { get; set; }
        public Currency? Currency { get; set; }
    }
}