using CoreHub.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CoreHub.Infrastructure.Identity;

/// <summary>
/// Application user extending ASP.NET Identity
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Organization the user belongs to
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Associated practitioner ID (if user is a practitioner)
    /// </summary>
    public Guid? PractitionerId { get; set; }

    /// <summary>
    /// When the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the user was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Whether the user account is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Organization? Organization { get; set; }
}
