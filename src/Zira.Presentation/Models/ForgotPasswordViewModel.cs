using System.ComponentModel.DataAnnotations;

namespace Zira.Presentation.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(
            ErrorMessageResourceType = typeof(Common.Text),
            ErrorMessageResourceName = "EmailIsRequiredErrorMessage")]
        [EmailAddress(
            ErrorMessageResourceType = typeof(Common.Text),
            ErrorMessageResourceName = "EmailIsInvalidErrorMessage")]
        public string? Email { get; set; }
    }
}