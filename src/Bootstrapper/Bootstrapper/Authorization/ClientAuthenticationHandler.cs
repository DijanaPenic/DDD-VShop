using System.Text;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Auth.Constants;
using VShop.Modules.Identity.Infrastructure;
using VShop.Modules.Identity.Infrastructure.Queries;

namespace VShop.Bootstrapper.Authorization;

internal class ClientAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IIdentityDispatcher _identityDispatcher;

    public ClientAuthenticationHandler
    (
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IIdentityDispatcher identityDispatcher
    )
        : base(options, logger, encoder, clock)
        => _identityDispatcher = identityDispatcher;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Response.Headers.Add("WWW-Authenticate", "Basic");

        if (!Request.Headers.ContainsKey(ApplicationHeaders.AuthorizationHeader))
            return AuthenticateResult.Fail("Authorization header missing.");

        string authorizationHeader = Request.Headers[ApplicationHeaders.AuthorizationHeader].ToString();
        Regex authHeaderRegex = new(@"Basic (.*)");
        
        if (!authHeaderRegex.IsMatch(authorizationHeader))
            return AuthenticateResult.Fail("Authorization header not formatted properly.");

        string headerValue = authHeaderRegex.Replace(authorizationHeader, "$1");
        if (!headerValue.IsBase64String())
            return AuthenticateResult.Fail("Authorization header not formatted properly.");

        string authBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(headerValue));
        string[] authSplit = authBase64.Split(':', 2);
        string authClientId = authSplit[0];
        string authClientSecret = authSplit.Length > 1 ? authSplit[1] : string.Empty;

        if (!Guid.TryParse(authClientId, out Guid clientId) || SequentialGuid.IsNullOrEmpty(clientId))
            return AuthenticateResult.Fail($"Client '{clientId}' format is invalid.");

        Result<bool> authResult = (Result<bool>)await _identityDispatcher
            .QueryAsync(new IsClientAuthenticatedQuery(clientId, authClientSecret));
        
        if (authResult.IsError) return AuthenticateResult.Fail(authResult.Error.ToString());

        ClaimsIdentity identity = new("ClientAuthentication");
        identity.AddClaim(new Claim(ApplicationClaimTypes.ClientIdentifier, authClientId));
        
        ClaimsPrincipal claimsPrincipal = new(identity);
        AuthenticationTicket authenticationTicket = new(claimsPrincipal, Scheme.Name);

        return AuthenticateResult.Success(authenticationTicket);
    }
}