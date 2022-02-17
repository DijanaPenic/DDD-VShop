using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Auth;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace VShop.Modules.Identity.Infrastructure.Services;

internal class AuthService : IAuthService
{
    private readonly ApplicationAuthManager _authManager;
    private readonly ApplicationSignInManager _signInManager;
    private readonly IContext _context;

    public AuthService(ApplicationAuthManager authManager, ApplicationSignInManager signInManager, IContext context)
    {
        _authManager = authManager;
        _signInManager = signInManager;
        _context = context;
    }

    public async Task<Result<SignInResponse>> ProcessAuthAsync(SignInResult signInResult, User user)
    {
        SignInResponse signInResponse = new(user.Id, user.Email, user.PhoneNumber);
        
        if (signInResult.Succeeded)
        {
            JsonWebToken token = await _authManager.CreateTokenAsync(user.Id, _context.Identity.ClientId);
            await _signInManager.SignInWithJsonWebTokenAsync(user.Id.ToString(), token.AccessToken); // TODO - save directly to cookie; without auth.

            signInResponse.Roles = token.Roles.ToArray();
            signInResponse.AccessToken = token.AccessToken;
            signInResponse.RefreshToken = token.RefreshToken;
            signInResponse.VerificationStep = VerificationStep.None;

            return signInResponse;
        }

        if (signInResult.IsLockedOut) return Result.Unauthorized($"User [{user.UserName}] has been locked out.");
        if (signInResult.RequiresTwoFactor)
        {
            signInResponse.VerificationStep = VerificationStep.TwoFactor;
            return signInResponse;
        }
        if (signInResult.IsNotAllowed)
        {
            if (!user.EmailConfirmed) signInResponse.VerificationStep = VerificationStep.Email;
            else if (!user.PhoneNumberConfirmed) signInResponse.VerificationStep = VerificationStep.MobilePhone;
            else return Result.ValidationError($"User [{user.UserName}] is not allowed to log in.");

            return signInResponse;
        }

        return Result.Unauthorized($"Failed to log in [{user.UserName}].");
    }
}

internal class SignInResponse
{
    public Guid UserId { get; }
    public string Email { get; }
    public string PhoneNumber { get; }
    public string[] Roles { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public VerificationStep VerificationStep { get; set; }
        
    public SignInResponse(Guid userId, string email, string phoneNumber)
    {
        UserId = userId;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}
    
internal enum VerificationStep
{
    None = 0,
    TwoFactor = 1,
    Email = 2,
    MobilePhone = 3,
}

internal interface IAuthService
{
    Task<Result<SignInResponse>> ProcessAuthAsync(SignInResult signInResult, User user);
}