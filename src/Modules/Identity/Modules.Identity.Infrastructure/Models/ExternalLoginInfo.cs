namespace VShop.Modules.Identity.Infrastructure.Models;

internal record ExternalLoginInfo(string LoginProvider, string ProviderKey, string ProviderDisplayName);