using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreHub.Application.Authorization;
using CoreHub.Domain.Entities;
using CoreHub.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace CoreHub.Tests.Features.Auth;

public class AuthorizationTests : TestBase
{
    [Fact]
    public async Task OrganizationAccessHandler_AuthenticatedUserWithOrgClaim_Succeeds()
    {
        // Arrange
        var handler = new OrganizationAccessHandler();
        var requirement = new OrganizationAccessRequirement();
        
        SetupAuthenticatedUser(TestUserId, TestUserEmail, TestOrganizationId, new List<string> { "User" });
        
        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            CurrentUser!,
            null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task OrganizationAccessHandler_UnauthenticatedUser_Fails()
    {
        // Arrange
        var handler = new OrganizationAccessHandler();
        var requirement = new OrganizationAccessRequirement();
        
        SetupUnauthenticatedUser();
        
        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            CurrentUser!,
            null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
        context.HasFailed.Should().BeTrue();
    }

    [Fact]
    public async Task OrganizationAccessHandler_UserWithoutOrgClaim_Fails()
    {
        // Arrange
        var handler = new OrganizationAccessHandler();
        var requirement = new OrganizationAccessRequirement();
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, TestUserId.ToString()),
            new Claim(ClaimTypes.Email, TestUserEmail)
            // Missing OrganizationId claim
        };
        
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        CurrentUser = new ClaimsPrincipal(identity);
        
        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            CurrentUser,
            null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
        context.HasFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Repository_GetAllAsync_FiltersEntitiesByOrganization()
    {
        // Arrange
        var org1 = Guid.NewGuid();
        var org2 = Guid.NewGuid();

        // Create practitioners for different organizations
        var practitioner1 = new Practitioner
        {
            Id = Guid.NewGuid(),
            FirstName = "Dr. John",
            LastName = "Smith",
            Email = "john@org1.com",
            OrganizationId = org1,
            Specialty = "Cardiology",
            CreatedAt = DateTime.UtcNow
        };

        var practitioner2 = new Practitioner
        {
            Id = Guid.NewGuid(),
            FirstName = "Dr. Jane",
            LastName = "Doe",
            Email = "jane@org2.com",
            OrganizationId = org2,
            Specialty = "Neurology",
            CreatedAt = DateTime.UtcNow
        };

        Context.Practitioners.AddRange(practitioner1, practitioner2);
        await Context.SaveChangesAsync();

        // Setup user for org1
        SetupUserWithOrganization(org1);

        var repository = new PractitionerRepository(Context, MockHttpContextAccessor.Object);

        // Act
        var results = await repository.GetAllAsync();

        // Assert
        results.Should().HaveCount(1);
        results.First().OrganizationId.Should().Be(org1);
        results.Should().NotContain(p => p.OrganizationId == org2);
    }

    [Fact]
    public async Task Repository_GetByIdAsync_ReturnsOnlyOwnOrganizationEntity()
    {
        // Arrange
        var org1 = Guid.NewGuid();
        var org2 = Guid.NewGuid();

        var practitioner1 = new Practitioner
        {
            Id = Guid.NewGuid(),
            FirstName = "Dr. John",
            LastName = "Smith",
            Email = "john@org1.com",
            OrganizationId = org1,
            Specialty = "Cardiology",
            CreatedAt = DateTime.UtcNow
        };

        var practitioner2 = new Practitioner
        {
            Id = Guid.NewGuid(),
            FirstName = "Dr. Jane",
            LastName = "Doe",
            Email = "jane@org2.com",
            OrganizationId = org2,
            Specialty = "Neurology",
            CreatedAt = DateTime.UtcNow
        };

        Context.Practitioners.AddRange(practitioner1, practitioner2);
        await Context.SaveChangesAsync();

        // Setup user for org1
        SetupUserWithOrganization(org1);

        var repository = new PractitionerRepository(Context, MockHttpContextAccessor.Object);

        // Act - Try to get practitioner from org1
        var result1 = await repository.GetByIdAsync(practitioner1.Id);
        
        // Try to get practitioner from org2 (should not be accessible)
        var result2 = await repository.GetByIdAsync(practitioner2.Id);

        // Assert
        result1.Should().NotBeNull();
        result1!.Id.Should().Be(practitioner1.Id);
        
        result2.Should().BeNull(); // Cannot access other organization's entities
    }

    [Fact]
    public async Task Repository_AddAsync_AutomaticallyAssignsOrganizationId()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        SetupUserWithOrganization(orgId);

        var repository = new PractitionerRepository(Context, MockHttpContextAccessor.Object);

        var practitioner = new Practitioner
        {
            Id = Guid.NewGuid(),
            FirstName = "Dr. New",
            LastName = "Practitioner",
            Email = "new@example.com",
            Specialty = "General",
            // OrganizationId not set
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await repository.AddAsync(practitioner);
        await repository.SaveChangesAsync();

        // Assert
        practitioner.OrganizationId.Should().Be(orgId);
        
        var saved = await Context.Practitioners.FindAsync(practitioner.Id);
        saved!.OrganizationId.Should().Be(orgId);
    }

    [Fact]
    public async Task Repository_UpdateAsync_PreventsAccessToOtherOrganization()
    {
        // Arrange
        var org1 = Guid.NewGuid();
        var org2 = Guid.NewGuid();

        var practitioner = new Practitioner
        {
            Id = Guid.NewGuid(),
            FirstName = "Dr. Test",
            LastName = "User",
            Email = "test@org2.com",
            OrganizationId = org2,
            Specialty = "Test",
            CreatedAt = DateTime.UtcNow
        };

        Context.Practitioners.Add(practitioner);
        await Context.SaveChangesAsync();

        // Setup user for org1 (different organization)
        SetupUserWithOrganization(org1);

        var repository = new PractitionerRepository(Context, MockHttpContextAccessor.Object);

        // Act & Assert
        practitioner.FirstName = "Updated";
        
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await repository.UpdateAsync(practitioner));
    }

    [Fact]
    public async Task Repository_DeleteAsync_PreventsAccessToOtherOrganization()
    {
        // Arrange
        var org1 = Guid.NewGuid();
        var org2 = Guid.NewGuid();

        var practitioner = new Practitioner
        {
            Id = Guid.NewGuid(),
            FirstName = "Dr. Test",
            LastName = "User",
            Email = "test@org2.com",
            OrganizationId = org2,
            Specialty = "Test",
            CreatedAt = DateTime.UtcNow
        };

        Context.Practitioners.Add(practitioner);
        await Context.SaveChangesAsync();

        // Setup user for org1 (different organization)
        SetupUserWithOrganization(org1);

        var repository = new PractitionerRepository(Context, MockHttpContextAccessor.Object);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await repository.DeleteAsync(practitioner));
    }

    [Fact]
    public async Task Repository_CountAsync_CountsOnlyOrganizationEntities()
    {
        // Arrange
        var org1 = Guid.NewGuid();
        var org2 = Guid.NewGuid();

        Context.Practitioners.AddRange(
            new Practitioner { Id = Guid.NewGuid(), FirstName = "P1", LastName = "Org1", Email = "p1@org1.com", OrganizationId = org1, Specialty = "S", CreatedAt = DateTime.UtcNow },
            new Practitioner { Id = Guid.NewGuid(), FirstName = "P2", LastName = "Org1", Email = "p2@org1.com", OrganizationId = org1, Specialty = "S", CreatedAt = DateTime.UtcNow },
            new Practitioner { Id = Guid.NewGuid(), FirstName = "P3", LastName = "Org2", Email = "p3@org2.com", OrganizationId = org2, Specialty = "S", CreatedAt = DateTime.UtcNow }
        );
        await Context.SaveChangesAsync();

        SetupUserWithOrganization(org1);

        var repository = new PractitionerRepository(Context, MockHttpContextAccessor.Object);

        // Act
        var count = await repository.CountAsync();

        // Assert
        count.Should().Be(2); // Only org1 practitioners
    }
}
