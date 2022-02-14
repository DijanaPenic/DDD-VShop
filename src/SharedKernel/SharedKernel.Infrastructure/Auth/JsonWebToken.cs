using System;
using System.Collections.Generic;

namespace VShop.SharedKernel.Infrastructure.Auth;

public class JsonWebToken
{
    public string AccessToken { get; set; }
    public long AccessTokenExpiry { get; set; }
    public string RefreshToken { get; set; }
    public long RefreshTokenExpiry { get; set; }
    public Guid UserId { get; set; }
    public IList<string> Roles { get; set; }
    public string Email { get; set; }
    public IDictionary<string, IEnumerable<string>> Claims { get; set; }
}