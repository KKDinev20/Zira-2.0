using System.ComponentModel.DataAnnotations;

namespace Zira.Presentation.Models
{
    public class ChangePasswordViewModel
    {
        [Required(
            ErrorMessageResourceType = typeof(Common.AuthenticationText),
            ErrorMessageResourceName = "CurrentPasswordIsRequiredErrorMessage")]
        [Display(
            Name = "CurrentPasswordLabel",
            ResourceType = typeof(Common.AuthenticationText))]
        public string? OldPassword { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Common.AuthenticationText),
            ErrorMessageResourceName = "NewPasswordIsRequiredErrorMessage")]
        [Display(
            Name = "NewPasswordLabel",
            ResourceType = typeof(Common.AuthenticationText))]
        public string? NewPassword { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Common.AuthenticationText),
            ErrorMessageResourceName = "ConfirmNewPasswordIsRequiredErrorMessage")]
        [Compare(
            nameof(NewPassword),
            ErrorMessageResourceType = typeof(Common.AuthenticationText),
            ErrorMessageResourceName = "PasswordsDoNotMatchErrorMessage")]
        [Display(
            Name = "ConfirmNewPasswordLabel",
            ResourceType = typeof(Common.AuthenticationText))]
        public string? ConfirmNewPassword { get; set; }
    }
}