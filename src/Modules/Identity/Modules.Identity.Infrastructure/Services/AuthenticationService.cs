using Microsoft.AspNetCore.Http;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Auth;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.Modules.Identity.Infrastructure.Models;
using VShop.Modules.Identity.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Infrastructure.Auth.Constants;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace VShop.Modules.Identity.Infrastructure.Services;

internal class AuthenticationService : IAuthenticationService
{
    private readonly ApplicationAuthManager _authManager;
    private readonly HttpContext _httpContext;
    private readonly IIdentityContext _identityContext;
    private readonly CookieOptions _cookieOptions;

    public AuthenticationService
    (
        ApplicationAuthManager authManager,
        IHttpContextAccessor httpContextAccessor,
        IContext context,
        CookieOptions cookieOptions
    )
    {
        _authManager = authManager;
        _httpContext = httpContextAccessor.HttpContext;
        _identityContext = context.Identity;
        _cookieOptions = cookieOptions;
    }

    public async Task<Result<SignInInfo>> FinalizeSignInAsync
    (
        SignInResult signInResult,
        User user,
        string loginProvider = null
    )
    {
        SignInInfo signInResponse = new(user);
        if (signInResult.Succeeded)
        {
            Guid clientId = _identityContext.ClientId;
            JsonWebToken token = await _authManager.CreateTokenAsync(user.Id, clientId, loginProvider);
            
            AddCookie(ApplicationIdentityConstants.AccessTokenScheme, token.AccessToken);

            signInResponse.Roles = token.Roles.ToArray();
            signInResponse.AccessToken = token.AccessToken;
            signInResponse.RefreshToken = token.RefreshToken;
            signInResponse.VerificationStep = AccountVerificationStep.None;

            return signInResponse;
        }

        if (signInResult.IsLockedOut) return Result.Unauthorized($"User [{user.UserName}] has been locked out.");
        if (signInResult.RequiresTwoFactor)
        {
            signInResponse.VerificationStep = AccountVerificationStep.TwoFactor;
            return signInResponse;
        }
        if (signInResult.IsNotAllowed)
        {
            if (!user.EmailConfirmed) signInResponse.VerificationStep = AccountVerificationStep.Email;
            else if (!user.PhoneNumberConfirmed) signInResponse.VerificationStep = AccountVerificationStep.MobilePhone;
            else return Result.ValidationError($"User [{user.UserName}] is not allowed to log in.");

            return signInResponse;
        }

        return Result.Unauthorized($"Failed to log in [{user.UserName}].");
    }

    public Task FinalizeSignOutAsync()
    {
        DeleteCookie(ApplicationIdentityConstants.AccessTokenScheme);
        return Task.CompletedTask;
    }

    private void AddCookie(string key, string value)
        => _httpContext.Response.Cookies.Append(key, value, _cookieOptions);
    
    private void DeleteCookie(string key)
        => _httpContext.Response.Cookies.Delete(key, _cookieOptions);
}