using System;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Contexts.Contracts;

namespace VShop.SharedKernel.Infrastructure.Contexts;

public class IdentityContext : IIdentityContext
{
    public Guid Id { get; }
    public bool IsAuthenticated { get; }
    public string Role { get; }
    public IDictionary<string, IEnumerable<string>> Claims { get; }

    private IdentityContext()
    {
    }

    public IdentityContext(Guid? id)
    {
        Id = id ?? Guid.Empty;
        IsAuthenticated = id.HasValue;
    }

    public IdentityContext(ClaimsPrincipal principal)
    {
        if (principal?.Identity is null || string.IsNullOrWhiteSpace(principal.Identity.Name)) return;

        IsAuthenticated = principal.Identity?.IsAuthenticated is true;
        Id = IsAuthenticated ? Guid.Parse(principal.Identity.Name) : Guid.Empty;
        Role = principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        Claims = principal.Claims
            .GroupBy(c => c.Type)
            .ToDictionary(g => g.Key, g => g.Select(c => c.Value.ToString()));
    }
        
    public bool IsUser() => Role is "user";
        
    public bool IsAdmin() => Role is "admin";
    public static IIdentityContext Empty => new IdentityContext();
}