using System.ComponentModel.DataAnnotations;

namespace Zira.Presentation.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(
            ErrorMessageResourceType = typeof(Common.AuthenticationText),
            ErrorMessageResourceName = "EmailIsRequiredErrorMessage")]
        [EmailAddress(
            ErrorMessageResourceType = typeof(Common.AuthenticationText),
            ErrorMessageResourceName = "EmailIsInvalidErrorMessage")]
        public string? Email { get; set; }
    }
}