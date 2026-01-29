using System.Security.Claims;

namespace CoreHub.Application.Extensions;

/// <summary>
/// Extension methods for ClaimsPrincipal to extract common claims
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user ID from claims
    /// </summary>
    /// <param name="principal">ClaimsPrincipal</param>
    /// <returns>User ID as Guid</returns>
    /// <exception cref="InvalidOperationException">Thrown when user ID claim is missing or invalid</exception>
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim))
            throw new InvalidOperationException("User ID claim not found");

        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new InvalidOperationException("User ID claim is not a valid GUID");

        return userId;
    }

    /// <summary>
    /// Gets the organization ID from claims
    /// </summary>
    /// <param name="principal">ClaimsPrincipal</param>
    /// <returns>Organization ID as Guid</returns>
    /// <exception cref="InvalidOperationException">Thrown when organization ID claim is missing or invalid</exception>
    public static Guid GetOrganizationId(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));

        var orgIdClaim = principal.FindFirst("OrganizationId")?.Value;
        
        if (string.IsNullOrEmpty(orgIdClaim))
            throw new InvalidOperationException("OrganizationId claim not found");

        if (!Guid.TryParse(orgIdClaim, out var organizationId))
            throw new InvalidOperationException("OrganizationId claim is not a valid GUID");

        return organizationId;
    }

    /// <summary>
    /// Gets the user's email from claims
    /// </summary>
    /// <param name="principal">ClaimsPrincipal</param>
    /// <returns>Email address</returns>
    /// <exception cref="InvalidOperationException">Thrown when email claim is missing</exception>
    public static string GetEmail(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));

        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        
        if (string.IsNullOrEmpty(email))
            throw new InvalidOperationException("Email claim not found");

        return email;
    }

    /// <summary>
    /// Gets all roles assigned to the user
    /// </summary>
    /// <param name="principal">ClaimsPrincipal</param>
    /// <returns>List of role names</returns>
    public static List<string> GetRoles(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));

        return principal.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
    }

    /// <summary>
    /// Checks if the user is in a specific role
    /// </summary>
    /// <param name="principal">ClaimsPrincipal</param>
    /// <param name="role">Role name to check</param>
    /// <returns>True if user is in the role, false otherwise</returns>
    public static bool IsInRole(this ClaimsPrincipal principal, string role)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));

        if (string.IsNullOrEmpty(role))
            return false;

        return principal.IsInRole(role);
    }

    /// <summary>
    /// Tries to get the user ID from claims without throwing an exception
    /// </summary>
    /// <param name="principal">ClaimsPrincipal</param>
    /// <param name="userId">Output user ID</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool TryGetUserId(this ClaimsPrincipal principal, out Guid userId)
    {
        userId = Guid.Empty;
        
        if (principal == null)
            return false;

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim))
            return false;

        return Guid.TryParse(userIdClaim, out userId);
    }

    /// <summary>
    /// Tries to get the organization ID from claims without throwing an exception
    /// </summary>
    /// <param name="principal">ClaimsPrincipal</param>
    /// <param name="organizationId">Output organization ID</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool TryGetOrganizationId(this ClaimsPrincipal principal, out Guid organizationId)
    {
        organizationId = Guid.Empty;
        
        if (principal == null)
            return false;

        var orgIdClaim = principal.FindFirst("OrganizationId")?.Value;
        
        if (string.IsNullOrEmpty(orgIdClaim))
            return false;

        return Guid.TryParse(orgIdClaim, out organizationId);
    }
}
