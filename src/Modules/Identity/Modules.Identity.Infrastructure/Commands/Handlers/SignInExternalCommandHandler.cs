using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.Modules.Identity.Integration.Events;
using VShop.Modules.Identity.Infrastructure.Models;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.Services.Contracts;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class SignInExternalCommandHandler : ICommandHandler<SignInExternalCommand, SignInInfo>
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly IIntegrationEventService _integrationEventService;
        private readonly IAuthenticationService _authService;

        public SignInExternalCommandHandler
        (
            ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            IIntegrationEventService integrationEventService,
            IAuthenticationService authService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _integrationEventService = integrationEventService;
            _authService = authService;
        }

        public async Task<Result<SignInInfo>> HandleAsync
        (
            SignInExternalCommand command,
            CancellationToken cancellationToken
        )
        {
            ExternalAuthInfo externalLoginInfo = await _signInManager.GetExternalAuthInfoAsync();
            if (externalLoginInfo is null) return Result.Unauthorized("External authentication has failed.");

            User user = await _userManager.FindByLoginAsync(externalLoginInfo);
            if (user is not null) return await ExternalLoginSignInAsync(user, externalLoginInfo);

            string userEmail = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Result.ValidationError("Email scope access is required to add external provider.");

            user = await _userManager.FindByEmailAsync(userEmail);
            if (user is null) return Result.NotFoundError("User not found.");
            if (!user.IsApproved) return Result.ValidationError("User is not allowed.");
            if (!user.EmailConfirmed)
            {
                return await VerifyAccountAsync
                (
                    user,
                    externalLoginInfo,
                    command.ConfirmationUrl,
                    cancellationToken
                );
            }

            IdentityResult result = await _userManager.AddLoginAsync(user, externalLoginInfo);
            if (!result.Succeeded) return Result.ValidationError(result.Errors);
            
            return await ExternalLoginSignInAsync(user, externalLoginInfo);
        }

        private async Task<Result<SignInInfo>> VerifyAccountAsync
        (
            User user,
            UserLoginInfo loginInfo,
            string confirmationUrl,
            CancellationToken cancellationToken
        )
        {
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            IdentityResult result = await _userManager.SetLoginAsync(user, loginInfo, token);
            if (!result.Succeeded) return Result.ValidationError(result.Errors);
                
            ExternalAccountConfirmationRequestedIntegrationEvent integrationEvent = new
            (
                user.Id, user.Email,
                token, confirmationUrl
            );
            await _integrationEventService.SaveEventAsync(integrationEvent, cancellationToken);

            return new SignInInfo(user) { VerificationStep = AccountVerificationStep.Email };
        }

        private async Task<Result<SignInInfo>> ExternalLoginSignInAsync(User user, ExternalAuthInfo loginInfo)
        {
            Guid clientId = Guid.Parse(loginInfo.ClientId);
            SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync
            (
                clientId,
                loginInfo.LoginProvider, 
                loginInfo.ProviderKey, 
                true
            );

            return await _authService.FinalizeSignInAsync(signInResult, user, loginInfo.LoginProvider);
        }
    }
}

// When you sign in with an external provider and no user with the email used in the provider found,
// the app should redirect the user o the register page giving two options. The first one is to provide a username
// and create a new account. The second one is to provide email and associate external login with the existing account.