namespace VShop.Modules.Identity.Infrastructure.Models;

internal enum AccountVerificationStep
{
    None = 0,
    TwoFactor = 1,
    Email = 2,
    MobilePhone = 3,
}