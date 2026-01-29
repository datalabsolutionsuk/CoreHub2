namespace CoreHub.Application.DTOs;

/// <summary>
/// DTO for user information
/// </summary>
public class UserInfoDto
{
    /// <summary>
    /// User's unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Organization ID the user belongs to
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// List of roles assigned to the user
    /// </summary>
    public IList<string> Roles { get; set; } = new List<string>();
}
