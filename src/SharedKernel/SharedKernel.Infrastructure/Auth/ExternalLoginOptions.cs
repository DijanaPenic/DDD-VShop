namespace VShop.SharedKernel.Infrastructure.Auth;

public static class ExternalLoginProviders
{
    public const string Google = "Google";
    public const string Facebook = "Facebook";
}

public class ExternalLoginOptions
{
    public const string SectionName = "ExternalLogin";
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}