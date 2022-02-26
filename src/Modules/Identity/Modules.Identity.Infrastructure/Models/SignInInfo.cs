using VShop.Modules.Identity.Infrastructure.DAL.Entities;

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
        
    public SignInInfo(User user)
    {
        UserId = user.Id;
        Email = user.Email;
        PhoneNumber = user.PhoneNumber;
    }
}