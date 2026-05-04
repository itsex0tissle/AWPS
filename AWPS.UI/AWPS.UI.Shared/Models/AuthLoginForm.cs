using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AWPS.UI.Shared.Models;

public sealed class AuthLoginForm
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[A-Za-z\\d]{8,100}$", ErrorMessage = "Password must be at least 8 and at max 100 characters long and contain at least one uppercase letter, one lowercase letter, and one number")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DisplayName("Remember me")]
    public bool RememberMe { get; set; } = false;
}