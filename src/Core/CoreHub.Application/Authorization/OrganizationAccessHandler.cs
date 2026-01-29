using CoreHub.Application.Extensions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CoreHub.Application.Authorization;

/// <summary>
/// Authorization handler for organization access requirement
/// Ensures user has an OrganizationId claim for multi-tenant isolation
/// </summary>
public class OrganizationAccessHandler : AuthorizationHandler<OrganizationAccessRequirement>
{
    /// <summary>
    /// Handles the authorization requirement
    /// </summary>
    /// <param name="context">Authorization context</param>
    /// <param name="requirement">The requirement to handle</param>
    /// <returns>Task</returns>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        OrganizationAccessRequirement requirement)
    {
        // Check if user is authenticated
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // Try to get OrganizationId claim
        if (context.User.TryGetOrganizationId(out var organizationId) && organizationId != Guid.Empty)
        {
            // User has valid organization claim
            context.Succeed(requirement);
        }
        else
        {
            // User doesn't have organization claim
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
