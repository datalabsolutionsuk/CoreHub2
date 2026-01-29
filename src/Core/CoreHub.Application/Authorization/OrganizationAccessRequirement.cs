using Microsoft.AspNetCore.Authorization;

namespace CoreHub.Application.Authorization;

/// <summary>
/// Authorization requirement for organization-level access control
/// </summary>
public class OrganizationAccessRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Initializes a new instance of the OrganizationAccessRequirement
    /// </summary>
    public OrganizationAccessRequirement()
    {
    }
}
