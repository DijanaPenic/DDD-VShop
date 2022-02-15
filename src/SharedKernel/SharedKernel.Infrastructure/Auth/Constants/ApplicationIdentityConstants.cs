namespace VShop.SharedKernel.Infrastructure.Auth.Constants;

public static class ApplicationIdentityConstants
{
    private const string CookiePrefix = "Identity";

    public const string AccountVerificationScheme = CookiePrefix + ".AccountVerification";
    public const string AccessTokenScheme = CookiePrefix + ".AccessToken";
    public const string AuthorizationHeader = "Authorization";
}