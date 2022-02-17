using Microsoft.AspNetCore.Authorization;

namespace VShop.Modules.Identity.Infrastructure.Attributes;

public class ClientAuthorizationAttribute : AuthorizeAttribute
{
    public ClientAuthorizationAttribute()
    {
        Policy = "ClientAuthentication";
    }
}