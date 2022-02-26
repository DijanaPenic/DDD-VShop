using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.Modules.Identity.Integration.Events;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.Constants;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class SignUpExternalCommandHandler : ICommandHandler<SignUpExternalCommand>
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly IIntegrationEventService _integrationEventService;

        public SignUpExternalCommandHandler
        (
            ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            IIntegrationEventService integrationEventService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _integrationEventService = integrationEventService;
        }

        public async Task<Result> Handle
        (
            SignUpExternalCommand command,
            CancellationToken cancellationToken
        )
        {
            ExternalLoginInfo externalLoginInfo = await _signInManager.GetExternalAuthInfoAsync();
            if (externalLoginInfo is null) return Result.Unauthorized("External authentication has failed.");

            (string userName, string associateEmail, bool associate, string confirmationUrl) = command;
            
            if (associate) return await AssociateToUserAsync(externalLoginInfo, associateEmail, confirmationUrl, cancellationToken);
            
            return await CreateUserAsync(externalLoginInfo, userName, cancellationToken);
        }

        private async Task<Result> AssociateToUserAsync
        (
            UserLoginInfo loginInfo,
            string associateEmail,
            string confirmationUrl,
            CancellationToken cancellationToken = default
        )
        {
            User existingUser = await _userManager.FindByEmailAsync(associateEmail);
            
            if (existingUser is null) return Result.NotFoundError("User not found.");
            if (!existingUser.IsApproved) return Result.ValidationError("User not approved.");
            if (!existingUser.EmailConfirmed) return Result.ValidationError("User email not confirmed.");

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(existingUser);

            IdentityResult result = await _userManager.SetLoginAsync(existingUser, loginInfo, token);
            if (!result.Succeeded) return Result.ValidationError(result.Errors);

            ExternalAccountConfirmationRequestedIntegrationEvent integrationEvent = new
            (
                existingUser.Id, existingUser.Email,
                token, confirmationUrl
            );
            await _integrationEventService.SaveEventAsync(integrationEvent, cancellationToken);

            return Result.Success;
        }

        private async Task<Result> CreateUserAsync
        (
            ExternalLoginInfo loginInfo,
            string userName,
            CancellationToken cancellationToken = default
        )
        {
            User existingUser = await _userManager.FindByNameAsync(userName);
            if (existingUser is not null) 
                return Result.ValidationError($"A user with {userName} username already exists in the system.");
            
            string email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            string phoneNumber = loginInfo.Principal.FindFirstValue(ClaimTypes.MobilePhone);

            // Email and phone number confirmation is not required because email was obtained from
            // the secure source (external provider).
            User newUser = new()
            {
                UserName = userName,
                Email = email,
                PhoneNumber = phoneNumber,
                IsApproved = true,
                EmailConfirmed = true,
                PhoneNumberConfirmed = !string.IsNullOrEmpty(phoneNumber)
            };

            // Create a new user.
            IdentityResult createUserResult = await _userManager.CreateAsync(newUser);
            if (!createUserResult.Succeeded) return Result.ValidationError(createUserResult.Errors);

            // Add user to role.
            IdentityResult addToRoleResult = await _userManager.AddToRoleAsync(newUser, Roles.Customer);
            if (!addToRoleResult.Succeeded) return Result.ValidationError(addToRoleResult.Errors);
            
            // Add external login for the user.
            IdentityResult createLoginResult = await _userManager.AddLoginAsync(newUser, loginInfo);
            if (!createLoginResult.Succeeded) return Result.ValidationError(createLoginResult.Errors);
            
            CustomerSignedUpIntegrationEvent integrationEvent = new(newUser.Id);
            await _integrationEventService.SaveEventAsync(integrationEvent, cancellationToken);
            
            return Result.Success;
        }
    }
}