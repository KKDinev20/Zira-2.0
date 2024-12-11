using System.ComponentModel.DataAnnotations;

namespace Zira.Presentation.Models;

public class LoginViewModel
{
    [Required(
        ErrorMessageResourceType = typeof(Common.Text),
        ErrorMessageResourceName = "EmailIsRequiredErrorMessage")]
    [EmailAddress(
        ErrorMessageResourceType = typeof(Common.Text),
        ErrorMessageResourceName = "EmailIsInvalidErrorMessage")]
    public string? Email { get; set; }

    [Required(
        ErrorMessageResourceType = typeof(Common.Text),
        ErrorMessageResourceName = "PasswordIsRequiredErrorMessage")]
    public string? Password { get; set; }
    public bool RememberAccess { get; set; }
}