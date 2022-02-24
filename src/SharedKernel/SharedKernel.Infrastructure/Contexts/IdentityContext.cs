using System;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;

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

    public IdentityContext(ClaimsPrincipal principal, AuthenticationProperties authProperties)
    {
        if (principal?.Identity is null) return;

        IsAuthenticated = principal.Identity.IsAuthenticated;
        
        if(IsAuthenticated)
        {
            Claim userClaim = principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            UserId = Guid.TryParse(userClaim?.Value, out Guid userId) ? userId : Guid.Empty;
            
            Claim clientClaim = principal.Claims.SingleOrDefault(c => c.Type == ApplicationClaimTypes.ClientIdentifier);
            ClientId = Guid.TryParse(clientClaim?.Value ?? authProperties?.Items["Client"], out Guid clientId) ? clientId : Guid.Empty;
        }

        Roles = principal.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value.ToLowerInvariant())
            .ToList();
        
        Claims = principal.Claims
            .GroupBy(c => c.Type)
            .ToDictionary(g => g.Key, g => g.Select(c => c.Value.ToString()));
    }

    public static IIdentityContext Empty => new IdentityContext();
    public bool IsCurrentUser(Guid userId) => UserId == userId;
    public bool IsAuthorized(string policy)
        => Claims
            .Where(c => c.Key == "permission")
            .SelectMany(c => c.Value)
            .Any(c => Equals(c, policy));
}