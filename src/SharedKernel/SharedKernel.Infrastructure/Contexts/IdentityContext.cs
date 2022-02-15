using System;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Auth.Constants;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;

namespace VShop.SharedKernel.Infrastructure.Contexts;

public class IdentityContext : IIdentityContext
{
    public Guid UserId { get; }
    public Guid ClientId { get; }
    public bool IsAuthenticated { get; }
    public IList<string> Roles { get; }
    public IDictionary<string, IEnumerable<string>> Claims { get; }

    private IdentityContext() { }

    public IdentityContext(Guid? userId)
    {
        UserId = userId ?? Guid.Empty;
        IsAuthenticated = userId.HasValue;
    }

    public IdentityContext(ClaimsPrincipal principal)
    {
        if (principal?.Identity is null || string.IsNullOrWhiteSpace(principal.Identity.Name)) return;

        IsAuthenticated = principal.Identity?.IsAuthenticated is true;
        
        UserId = IsAuthenticated 
            ? Guid.Parse(principal.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier)!.Value) 
            : Guid.Empty;
        
        ClientId = IsAuthenticated 
            ? Guid.Parse(principal.Claims.Single(c => c.Type == ApplicationClaimTypes.ClientIdentifier)!.Value) 
            : Guid.Empty;
        
        Roles = principal.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value.ToLowerInvariant())
            .ToList();
        
        Claims = principal.Claims
            .GroupBy(c => c.Type)
            .ToDictionary(g => g.Key, g => g.Select(c => c.Value.ToString()));
    }

    public bool IsAdmin() => Roles.Contains("admin");
    public static IIdentityContext Empty => new IdentityContext();
}