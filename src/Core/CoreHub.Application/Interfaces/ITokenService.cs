using System.Security.Claims;

namespace CoreHub.Application.Interfaces;

/// <summary>
/// Interface for JWT token generation and validation
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates an access token for a user
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <param name="email">The user's email</param>
    /// <param name="firstName">The user's first name</param>
    /// <param name="lastName">The user's last name</param>
    /// <param name="organizationId">The user's organization ID</param>
    /// <param name="roles">List of roles assigned to the user</param>
    /// <returns>JWT access token</returns>
    string GenerateAccessToken(string userId, string email, string firstName, string lastName, Guid organizationId, IEnumerable<string> roles);

    /// <summary>
    /// Generates a refresh token
    /// </summary>
    /// <returns>Random refresh token string</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Gets the ClaimsPrincipal from an expired token for refresh purposes
    /// </summary>
    /// <param name="token">The expired JWT token</param>
    /// <returns>ClaimsPrincipal from the token</returns>
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
