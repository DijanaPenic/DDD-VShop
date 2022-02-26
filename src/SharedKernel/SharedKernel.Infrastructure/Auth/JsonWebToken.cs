using System;
using System.Collections.Generic;

namespace VShop.SharedKernel.Infrastructure.Auth;

public record JsonWebToken
{
    public string AccessToken { get; init; }
    public long AccessTokenExpiry { get; init; }
    public string RefreshToken { get; init; }
    public long RefreshTokenExpiry { get; init; }
    public Guid UserId { get; init; }
    public IList<string> Roles { get; init; }
    public string Email { get; init; }
}