using System.ComponentModel.DataAnnotations;

namespace CoreHub.Application.DTOs;

/// <summary>
/// DTO for user registration
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// User's email address (used for login)
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Password confirmation (must match Password)
    /// </summary>
    [Required]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// User's first name
    /// </summary>
    [Required]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    [Required]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Organization ID the user belongs to
    /// </summary>
    [Required]
    public Guid OrganizationId { get; set; }
}
