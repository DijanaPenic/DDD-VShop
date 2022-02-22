namespace VShop.Modules.Identity.Infrastructure.Models;

internal record ExternalLoginInfo
{
    public string LoginProvider { get; }
    public string ProviderKey { get; }
    public string ProviderDisplayName { get; }
    
    public ExternalLoginInfo(string loginProvider, string providerKey, string providerDisplayName)
    {
        LoginProvider = loginProvider;
        ProviderKey = providerKey;
        ProviderDisplayName = providerDisplayName;
    }
}