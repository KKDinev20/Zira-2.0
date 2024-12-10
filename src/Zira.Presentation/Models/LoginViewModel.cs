using System.ComponentModel.DataAnnotations;

namespace Zira.Presentation.Models;

public class LoginViewModel
{
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }
    public bool RememberAccess { get; set; }
}