using System.ComponentModel.DataAnnotations;

namespace Zira.Presentation.Models
{
    public class ChangeEmailViewModel
    {
        [Required(
            ErrorMessageResourceType = typeof(Common.AuthenticationText),
            ErrorMessageResourceName = "NewEmailIsRequiredErrorMessage")]
        [EmailAddress(
            ErrorMessageResourceType = typeof(Common.AuthenticationText),
            ErrorMessageResourceName = "NewEmailIsInvalidErrorMessage")]
        [Display(
            Name = "NewEmailLabel",
            ResourceType = typeof(Common.AuthenticationText))]
        public string? NewEmail { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Common.AuthenticationText),
            ErrorMessageResourceName = "CurrentPasswordIsRequiredErrorMessage")]
        [Display(
            Name = "CurrentPasswordLabel",
            ResourceType = typeof(Common.AuthenticationText))]
        public string? CurrentPassword { get; set; }
    }
}