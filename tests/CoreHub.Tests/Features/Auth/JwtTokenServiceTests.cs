using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using CoreHub.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace CoreHub.Tests.Features.Auth;

public class JwtTokenServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly JwtTokenService _tokenService;
    private const string TestSecretKey = "ThisIsAVerySecureSecretKeyForJWTTokenGeneration123!";
    private const string TestIssuer = "TestIssuer";
    private const string TestAudience = "TestAudience";
    private const string TestExpirationMinutes = "60";

    public JwtTokenServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c["Jwt:SecretKey"]).Returns(TestSecretKey);
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns(TestIssuer);
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns(TestAudience);
        _mockConfiguration.Setup(c => c["Jwt:ExpirationMinutes"]).Returns(TestExpirationMinutes);

        _tokenService = new JwtTokenService(_mockConfiguration.Object);
    }

    [Fact]
    public void GenerateAccessToken_ValidParameters_ReturnsValidToken()
    {
        // Arrange
        var userId = "user123";
        var email = "test@example.com";
        var firstName = "John";
        var lastName = "Doe";
        var organizationId = Guid.NewGuid();
        var roles = new List<string> { "User", "Admin" };

        // Act
        var token = _tokenService.GenerateAccessToken(userId, email, firstName, lastName, organizationId, roles);

        // Assert
        token.Should().NotBeNullOrEmpty();

        // Validate token structure
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
        jwtToken.Claims.Should().Contain(c => c.Type == "firstName" && c.Value == firstName);
        jwtToken.Claims.Should().Contain(c => c.Type == "lastName" && c.Value == lastName);
        jwtToken.Claims.Should().Contain(c => c.Type == "organizationId" && c.Value == organizationId.ToString());
        jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Should().HaveCount(2);
    }

    [Fact]
    public void GenerateAccessToken_EmptyRoles_ReturnsTokenWithoutRoleClaims()
    {
        // Arrange
        var userId = "user123";
        var email = "test@example.com";
        var firstName = "Jane";
        var lastName = "Smith";
        var organizationId = Guid.NewGuid();
        var roles = new List<string>();

        // Act
        var token = _tokenService.GenerateAccessToken(userId, email, firstName, lastName, organizationId, roles);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Should().BeEmpty();
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsNonEmptyString()
    {
        // Act
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Assert
        refreshToken.Should().NotBeNullOrEmpty();
        refreshToken.Length.Should().BeGreaterThan(20);
    }

    [Fact]
    public void GenerateRefreshToken_MultipleCallsReturnDifferentTokens()
    {
        // Act
        var token1 = _tokenService.GenerateRefreshToken();
        var token2 = _tokenService.GenerateRefreshToken();

        // Assert
        token1.Should().NotBe(token2);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ValidExpiredToken_ReturnsClaimsPrincipal()
    {
        // Arrange
        var userId = "user123";
        var email = "test@example.com";
        var firstName = "Test";
        var lastName = "User";
        var organizationId = Guid.NewGuid();
        var roles = new List<string> { "User" };

        var token = _tokenService.GenerateAccessToken(userId, email, firstName, lastName, organizationId, roles);

        // Act
        var principal = _tokenService.GetPrincipalFromExpiredToken(token);

        // Assert
        principal.Should().NotBeNull();
        principal!.Identity.Should().NotBeNull();
        principal.Identity!.IsAuthenticated.Should().BeTrue();
        
        // Check that principal has claims (the exact claim types may vary)
        principal.Claims.Should().NotBeEmpty();
        principal.Claims.Any(c => c.Value == userId).Should().BeTrue();
        principal.Claims.Any(c => c.Value == email).Should().BeTrue();
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_InvalidToken_ReturnsNull()
    {
        // Arrange
        var invalidToken = "invalid.token.string";

        // Act
        var principal = _tokenService.GetPrincipalFromExpiredToken(invalidToken);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_NullToken_ReturnsNull()
    {
        // Act
        var principal = _tokenService.GetPrincipalFromExpiredToken(null!);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void Constructor_NullConfiguration_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<NullReferenceException>(() => new JwtTokenService(null!));
    }
}
