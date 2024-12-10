using System.ComponentModel.DataAnnotations;

namespace Zira.Presentation.Models;

public class RegisterViewModel
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

    [Compare(
        nameof(Password),
        ErrorMessageResourceType = typeof(Common.Text),
        ErrorMessageResourceName = "PasswordIsDifferentThanConfirmedErrorMessage")]
    public string? ConfirmPassword { get; set; }
}