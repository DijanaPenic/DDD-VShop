using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

namespace VShop.SharedKernel.Infrastructure.Auth.Handlers;

public class ApplicationGoogleHandler : GoogleHandler
{
    public ApplicationGoogleHandler
    (
        IOptionsMonitor<GoogleOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    ) : base(options, logger, encoder, clock) { }
    
    protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        => base.BuildChallengeUrl(properties, properties.RedirectUri ?? redirectUri);
}