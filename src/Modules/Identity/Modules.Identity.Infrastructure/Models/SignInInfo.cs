namespace VShop.Modules.Identity.Infrastructure.Models;

internal class SignInInfo
{
    public Guid UserId { get; }
    public string Email { get; }
    public string PhoneNumber { get; }
    public string[] Roles { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public AccountVerificationStep VerificationStep { get; set; }
        
    public SignInInfo(Guid userId, string email, string phoneNumber)
    {
        UserId = userId;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}

internal enum AccountVerificationStep
{
    None = 0,
    TwoFactor = 1,
    Email = 2,
    MobilePhone = 3,
}