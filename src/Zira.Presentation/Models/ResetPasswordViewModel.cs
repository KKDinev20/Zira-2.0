﻿using System.ComponentModel.DataAnnotations;

namespace Zira.Presentation.Models;

public class ResetPasswordViewModel
{
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Token { get; set; }

    [Required(
        ErrorMessageResourceType = typeof(Common.AuthenticationText),
        ErrorMessageResourceName = "PasswordIsRequiredErrorMessage")]
    public string? Password { get; set; }

    [Compare(
        nameof(Password),
        ErrorMessageResourceType = typeof(Common.AuthenticationText),
        ErrorMessageResourceName = "PasswordIsDifferentThanConfirmedErrorMessage")]
    public string? ConfirmPassword { get; set; }
}