using System.ComponentModel.DataAnnotations;

namespace Zira.Presentation.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}