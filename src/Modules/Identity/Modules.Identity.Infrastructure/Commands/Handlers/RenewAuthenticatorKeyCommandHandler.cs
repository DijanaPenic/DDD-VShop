using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.Modules.Identity.Infrastructure.Models;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class RenewAuthenticatorKeyCommandHandler : ICommandHandler<RenewAuthenticatorKeyCommand, AuthenticatorKey>
    {
        private readonly ApplicationUserManager _userManager;

        public RenewAuthenticatorKeyCommandHandler(ApplicationUserManager userManager) 
            => _userManager = userManager;

        public async Task<Result<AuthenticatorKey>> HandleAsync
        (
            RenewAuthenticatorKeyCommand command,
            CancellationToken cancellationToken
        )
        {
            User user = await _userManager.FindByIdAsync(command.UserId.ToString());
            if (user is null) return Result.NotFoundError("User not found.");
            
            // Set a new AuthenticatorKey.
            await _userManager.ResetAuthenticatorKeyAsync(user);

            // Retrieve created AuthenticatorKey.
            string authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(authenticatorKey)) 
                return Result.InternalServerError("Something went wrong - authenticator key not found.");
            
            return new AuthenticatorKey(authenticatorKey);
        }
    }
}