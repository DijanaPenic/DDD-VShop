using Microsoft.AspNetCore.Identity;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.Modules.Identity.Infrastructure.Models;
using VShop.Modules.Identity.Infrastructure.Services.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class SignInByTwoFactorCommandHandler : ICommandHandler<SignInByTwoFactorCommand, SignInInfo>
    {
        private readonly ApplicationSignInManager _signInManager;
        private readonly IAuthenticationService _authService;

        public SignInByTwoFactorCommandHandler
        (
            ApplicationSignInManager signInManager,
            IAuthenticationService authService
        )
        {
            _signInManager = signInManager;
            _authService = authService;
        }

        public async Task<Result<SignInInfo>> Handle
        (
            SignInByTwoFactorCommand command,
            CancellationToken cancellationToken
        )
        {
            (string code, bool useRecoveryCode) = command;
            
            User user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user is null) return Result.NotFoundError("User not found.");

            TwoFactorAuthenticationInfo twoFactorInfo = await _signInManager.GetTwoFactorInfoAsync();
            Guid clientId = Guid.Parse(twoFactorInfo.ClientId);
            
            SignInResult signInResult;
            
            if (useRecoveryCode)
                signInResult = await _signInManager.TwoFactorRecoveryCodeSignInAsync(code);
            else
            {
                //Note: rememberClient: false - don't want to suppress future two-factor auth requests.
                signInResult = await _signInManager.TwoFactorAuthenticatorSignInAsync(clientId, code, false);
            }

            return await _authService.FinalizeSignInAsync(signInResult, user);
        }
    }
}