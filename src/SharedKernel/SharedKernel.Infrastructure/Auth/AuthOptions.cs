using System.Collections.Generic;

namespace VShop.SharedKernel.Infrastructure.Auth;

public class AuthOptions
{
    public bool AuthenticationDisabled { get; set; }
    public string Issuer { get; set; }
    public string IssuerSigningKey { get; set; }
    public string Authority { get; set; }
    public string Audience { get; set; }
    public string Challenge { get; set; } = "Bearer";
    public string MetadataAddress { get; set; }
    public bool SaveToken { get; set; } = true;
    public bool SaveSigninToken { get; set; }
    public bool RequireAudience { get; set; } = true;
    public bool RequireHttpsMetadata { get; set; } = true;
    public bool RequireExpirationTime { get; set; } = true;
    public bool RequireSignedTokens { get; set; } = true;
    public string ValidAudience { get; set; }
    public IEnumerable<string> ValidAudiences { get; set; }
    public string ValidIssuer { get; set; }
    public IEnumerable<string> ValidIssuers { get; set; }
    public bool ValidateActor { get; set; }
    public bool ValidateAudience { get; set; } = true;
    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateLifetime { get; set; } = true;
    public bool ValidateTokenReplay { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }
    public bool RefreshOnIssuerKeyNotFound { get; set; } = true;
    public bool IncludeErrorDetails { get; set; } = true;
    public string AuthenticationType { get; set; }
    public string NameClaimType { get; set; }
    public string RoleClaimType { get; set; }
    public CookieOptions Cookie { get; set; }

    public class CookieOptions
    {
        public bool HttpOnly { get; set; }
        public bool Secure { get; set; }
        public string SameSite { get; set; }
    }
}