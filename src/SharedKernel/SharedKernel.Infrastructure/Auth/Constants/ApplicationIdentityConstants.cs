namespace VShop.SharedKernel.Infrastructure.Auth.Constants;

/// <summary>
/// Represents all the custom options you can use to configure the cookies middleware used by the identity system.
/// </summary>
public static class ApplicationIdentityConstants
{
    private const string CookiePrefix = "Identity";

    public const string AccountVerificationScheme = CookiePrefix + ".AccountVerification";
    public const string AccessTokenScheme = CookiePrefix + ".AccessToken";
}