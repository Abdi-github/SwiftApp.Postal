using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SwiftApp.Postal.SharedKernel.Interfaces;

namespace SwiftApp.Postal.WebApi.Infrastructure;

/// <summary>
/// Reads the current user's identity from Keycloak JWT claims in the HttpContext.
/// Roles are extracted from the nested realm_access.roles claim and mapped to ClaimTypes.Role
/// by the JwtBearer OnTokenValidated event in Program.cs.
/// </summary>
public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var sub = httpContextAccessor.HttpContext?.User.FindFirstValue("sub")
                ?? httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public string? Username
        => httpContextAccessor.HttpContext?.User.FindFirstValue("preferred_username")
        ?? httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

    public bool IsAuthenticated
        => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public IReadOnlyList<string> Roles
        => httpContextAccessor.HttpContext?.User
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList()
            .AsReadOnly()
        ?? (IReadOnlyList<string>)[];

    public bool IsInRole(string role)
        => httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;
}
