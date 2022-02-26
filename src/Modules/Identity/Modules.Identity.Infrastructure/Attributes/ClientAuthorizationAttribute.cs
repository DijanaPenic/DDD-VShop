using Microsoft.AspNetCore.Authorization;

using VShop.SharedKernel.Infrastructure.Auth.Constants;

namespace VShop.Modules.Identity.Infrastructure.Attributes;

public class ClientAuthorizationAttribute : AuthorizeAttribute
{
    public ClientAuthorizationAttribute()
    {
        AuthenticationSchemes = ApplicationAuthSchemes.ClientAuthenticationScheme;
    }
}