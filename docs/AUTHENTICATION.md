# Authentication & Authorization System

## Overview

CoreHub2 implements a comprehensive authentication and authorization system using ASP.NET Identity, JWT tokens, and role-based access control with organization-level data isolation.

## Architecture

### Components

1. **ASP.NET Identity**: User management, password hashing, and role management
2. **JWT Tokens**: Stateless authentication with claims-based identity
3. **Authorization Policies**: Role-based and organization-based access control
4. **Organization Isolation**: Automatic filtering of data by organization
5. **ClaimsPrincipal Extensions**: Helper methods for accessing user claims

## JWT Configuration

### Token Structure

JWT tokens include the following claims:

- `sub` (Subject): User ID
- `email`: User's email address
- `firstName`: User's first name
- `lastName`: User's last name
- `organizationId`: User's organization ID (critical for multi-tenancy)
- `role`: User's roles (can be multiple)
- `jti`: Unique token identifier
- `iat`: Issued at timestamp
- `exp`: Expiration timestamp

### Configuration

JWT settings are configured in `appsettings.json`:

```json
{
  "Jwt": {
    "SecretKey": "your-secret-key-min-32-characters",
    "Issuer": "CoreHub",
    "Audience": "CoreHubAPI",
    "ExpirationMinutes": "60"
  }
}
```

### Token Generation

The `JwtTokenService` generates access tokens and refresh tokens:

```csharp
var accessToken = _tokenService.GenerateAccessToken(
    userId: user.Id,
    email: user.Email,
    firstName: user.FirstName,
    lastName: user.LastName,
    organizationId: user.OrganizationId,
    roles: userRoles
);

var refreshToken = _tokenService.GenerateRefreshToken();
```

## Authorization Policies

### Built-in Policies

#### 1. RequireOrganizationAccess

Ensures the user has a valid `OrganizationId` claim. This is used for multi-tenant data isolation.

```csharp
[Authorize(Policy = "RequireOrganizationAccess")]
public class PatientsController : ControllerBase
{
    // All actions require organization access
}
```

#### 2. RequireAdminRole

Restricts access to users with the "Admin" role.

```csharp
[Authorize(Policy = "RequireAdminRole")]
public IActionResult AdminAction()
{
    // Only admins can access
}
```

#### 3. RequirePractitionerRole

Allows access to users with either "Practitioner" or "Admin" roles.

```csharp
[Authorize(Policy = "RequirePractitionerRole")]
public IActionResult PractitionerAction()
{
    // Practitioners and admins can access
}
```

### Policy Registration

Policies are registered in `Program.cs`:

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireOrganizationAccess", policy =>
        policy.Requirements.Add(new OrganizationAccessRequirement()));

    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("RequirePractitionerRole", policy =>
        policy.RequireRole("Practitioner", "Admin"));
});
```

## Organization-Level Isolation

### How It Works

The `Repository<T>` base class automatically filters queries by organization when:

1. The entity has an `OrganizationId` property
2. The user is authenticated with an `OrganizationId` claim

### Automatic Features

#### Query Filtering

All read operations automatically filter by organization:

```csharp
// User from Organization A cannot see Organization B's data
var practitioners = await _practitionerRepository.GetAllAsync();
// Returns only practitioners from user's organization
```

#### Organization Assignment

When creating new entities, the `OrganizationId` is automatically set:

```csharp
var practitioner = new Practitioner
{
    FirstName = "Dr. Smith",
    // OrganizationId not set
};

await _repository.AddAsync(practitioner);
// OrganizationId is automatically set to user's organization
```

#### Update/Delete Protection

Users cannot modify or delete entities from other organizations:

```csharp
await _repository.UpdateAsync(otherOrgEntity);
// Throws UnauthorizedAccessException
```

### Affected Methods

Organization filtering is applied to:

- `GetByIdAsync()`
- `GetAllAsync()`
- `FindAsync()`
- `CountAsync()`
- `ExistsAsync()`
- All specialized repository methods

## ClaimsPrincipal Extensions

### Available Methods

```csharp
using CoreHub.Application.Extensions;

// Get user ID
Guid userId = User.GetUserId();

// Get organization ID
Guid orgId = User.GetOrganizationId();

// Get email
string email = User.GetEmail();

// Get all roles
List<string> roles = User.GetRoles();

// Check role
bool isAdmin = User.IsInRole("Admin");

// Safe variants (no exceptions)
if (User.TryGetUserId(out Guid id))
{
    // Use id
}

if (User.TryGetOrganizationId(out Guid orgId))
{
    // Use orgId
}
```

## Roles

### Default Roles

1. **User**: Basic authenticated user
2. **Admin**: Full administrative access
3. **Practitioner**: Healthcare practitioner access

### Role Assignment

Roles are assigned during registration or can be modified later:

```csharp
await _userManager.AddToRoleAsync(user, "Admin");
await _userManager.RemoveFromRoleAsync(user, "User");
```

### Role Seeding

Default roles are created during database initialization in `DbInitializer`:

```csharp
private async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
{
    string[] roleNames = { "Admin", "Practitioner", "User" };
    
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}
```

## Adding New Policies

### 1. Create a Requirement

```csharp
public class CustomRequirement : IAuthorizationRequirement
{
    // Add any parameters needed
}
```

### 2. Create a Handler

```csharp
public class CustomHandler : AuthorizationHandler<CustomRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CustomRequirement requirement)
    {
        // Implement logic
        if (/* condition met */)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
        
        return Task.CompletedTask;
    }
}
```

### 3. Register in Program.cs

```csharp
// Register handler
builder.Services.AddScoped<IAuthorizationHandler, CustomHandler>();

// Register policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CustomPolicy", policy =>
        policy.Requirements.Add(new CustomRequirement()));
});
```

### 4. Apply to Controllers/Actions

```csharp
[Authorize(Policy = "CustomPolicy")]
public IActionResult CustomAction()
{
    // Protected by custom policy
}
```

## Security Best Practices

### 1. Always Use HTTPS in Production

```csharp
options.RequireHttpsMetadata = true; // Set to true in production
```

### 2. Strong Secret Keys

Use at least 32-character random keys:

```bash
openssl rand -base64 32
```

### 3. Token Expiration

Set appropriate expiration times:

- Access tokens: 15-60 minutes
- Refresh tokens: 7-30 days (store securely in database)

### 4. Password Requirements

Strong password policies are enforced:

```csharp
options.Password.RequireDigit = true;
options.Password.RequireLowercase = true;
options.Password.RequireUppercase = true;
options.Password.RequireNonAlphanumeric = true;
options.Password.RequiredLength = 8;
options.Password.RequiredUniqueChars = 4;
```

### 5. Account Lockout

Failed login attempts trigger temporary lockout:

```csharp
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
options.Lockout.MaxFailedAccessAttempts = 5;
options.Lockout.AllowedForNewUsers = true;
```

## Testing Authentication

### Creating Mock Users

```csharp
public class MyTests : TestBase
{
    [Fact]
    public void MyTest()
    {
        // Setup authenticated user
        SetupAuthenticatedUser(
            userId: Guid.NewGuid(),
            email: "test@example.com",
            organizationId: Guid.NewGuid(),
            roles: new List<string> { "User", "Admin" }
        );
        
        // Test code
    }
}
```

### Testing Organization Isolation

```csharp
[Fact]
public async Task Repository_FiltersBy Organization()
{
    var org1 = Guid.NewGuid();
    var org2 = Guid.NewGuid();
    
    // Create entities for different orgs
    Context.Practitioners.AddRange(
        new Practitioner { OrganizationId = org1 },
        new Practitioner { OrganizationId = org2 }
    );
    await Context.SaveChangesAsync();
    
    // Setup user for org1
    SetupUserWithOrganization(org1);
    
    var repo = new PractitionerRepository(Context, MockHttpContextAccessor.Object);
    var results = await repo.GetAllAsync();
    
    // Should only return org1 entities
    results.Should().HaveCount(1);
    results.First().OrganizationId.Should().Be(org1);
}
```

## Troubleshooting

### Common Issues

#### 1. "OrganizationId claim not found"

**Cause**: User token doesn't include organization ID
**Solution**: Ensure user is assigned to an organization during registration

#### 2. "Unauthorized access to entity"

**Cause**: Trying to access/modify entity from different organization
**Solution**: Check that entity belongs to user's organization

#### 3. "Token validation failed"

**Cause**: Invalid or expired token
**Solution**: Use refresh token to get new access token

## API Endpoints

See [API.md](../API.md) for detailed authentication endpoint documentation including:

- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login and get tokens
- `POST /api/auth/refresh-token` - Refresh expired token
- `GET /api/auth/me` - Get current user info

## References

- [ASP.NET Identity Documentation](https://docs.microsoft.com/aspnet/core/security/authentication/identity)
- [JWT.IO](https://jwt.io/)
- [OAuth 2.0 RFC](https://tools.ietf.org/html/rfc6749)
