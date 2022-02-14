using System.Security.Claims;
using System.Collections.Generic;

namespace VShop.SharedKernel.Infrastructure.Auth;

public class ClaimsComparer : IEqualityComparer<Claim>
{
    public bool Equals(Claim x, Claim y)
        => y is not null && x is not null && x.Value == y.Value;

    public int GetHashCode(Claim claim)
        => claim.Value?.GetHashCode() ?? 0;
}