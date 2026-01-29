using System;
using System.Collections.Generic;
using System.Security.Claims;
using CoreHub.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CoreHub.Tests;

/// <summary>
/// Base class for tests with authentication mocking support
/// </summary>
public abstract class TestBase : IDisposable
{
    protected readonly ApplicationDbContext Context;
    protected readonly Mock<IHttpContextAccessor> MockHttpContextAccessor;
    protected readonly Mock<HttpContext> MockHttpContext;
    protected ClaimsPrincipal? CurrentUser;

    protected Guid TestOrganizationId { get; set; }
    protected Guid TestUserId { get; set; }
    protected string TestUserEmail { get; set; }

    protected TestBase()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new ApplicationDbContext(options);

        // Setup HttpContext mocks
        MockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        MockHttpContext = new Mock<HttpContext>();

        // Set default test identities
        TestOrganizationId = Guid.NewGuid();
        TestUserId = Guid.NewGuid();
        TestUserEmail = "test@example.com";

        // Setup default authenticated user
        SetupAuthenticatedUser(TestUserId, TestUserEmail, TestOrganizationId, new List<string> { "User" });
    }

    /// <summary>
    /// Setup an authenticated user with claims
    /// </summary>
    protected void SetupAuthenticatedUser(Guid userId, string email, Guid organizationId, List<string> roles)
    {
        TestUserId = userId;
        TestUserEmail = email;
        TestOrganizationId = organizationId;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim("OrganizationId", organizationId.ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        CurrentUser = new ClaimsPrincipal(identity);

        MockHttpContext.Setup(c => c.User).Returns(CurrentUser);
        MockHttpContextAccessor.Setup(a => a.HttpContext).Returns(MockHttpContext.Object);
    }

    /// <summary>
    /// Setup an unauthenticated user
    /// </summary>
    protected void SetupUnauthenticatedUser()
    {
        CurrentUser = new ClaimsPrincipal(new ClaimsIdentity());
        MockHttpContext.Setup(c => c.User).Returns(CurrentUser);
        MockHttpContextAccessor.Setup(a => a.HttpContext).Returns(MockHttpContext.Object);
    }

    /// <summary>
    /// Setup user with specific organization
    /// </summary>
    protected void SetupUserWithOrganization(Guid organizationId)
    {
        SetupAuthenticatedUser(TestUserId, TestUserEmail, organizationId, new List<string> { "User" });
    }

    /// <summary>
    /// Setup user with specific role
    /// </summary>
    protected void SetupUserWithRole(string role)
    {
        SetupAuthenticatedUser(TestUserId, TestUserEmail, TestOrganizationId, new List<string> { role });
    }

    /// <summary>
    /// Setup user with multiple roles
    /// </summary>
    protected void SetupUserWithRoles(params string[] roles)
    {
        SetupAuthenticatedUser(TestUserId, TestUserEmail, TestOrganizationId, new List<string>(roles));
    }

    /// <summary>
    /// Get a new organization ID for multi-org testing
    /// </summary>
    protected Guid GetDifferentOrganizationId()
    {
        return Guid.NewGuid();
    }

    public virtual void Dispose()
    {
        Context?.Dispose();
    }
}
