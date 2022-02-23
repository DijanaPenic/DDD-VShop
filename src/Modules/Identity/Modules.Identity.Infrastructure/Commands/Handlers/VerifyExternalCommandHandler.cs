using Microsoft.AspNetCore.Identity;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class VerifyExternalCommandHandler : ICommandHandler<VerifyExternalCommand>
    {
        private readonly ApplicationUserManager _userManager;

        public VerifyExternalCommandHandler(ApplicationUserManager userManager) 
            => _userManager = userManager;

        public async Task<Result> Handle
        (
            VerifyExternalCommand command,
            CancellationToken cancellationToken
        )
        {
            (Guid userId, string token) = command;
            
            User user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result.NotFoundError("User not found.");

            string decodedToken = token.Base64Decode();

            if (!user.EmailConfirmed)
            {
                // External provider is authenticated source so we can confirm the email.
                IdentityResult confirmEmailResult = await _userManager.ConfirmEmailAsync(user, decodedToken);
                if (!confirmEmailResult.Succeeded) return Result.ValidationError(confirmEmailResult.Errors);
            }

            // Create a new external login for the user.
            IdentityResult confirmLoginResult = await _userManager.ConfirmLoginAsync(user, decodedToken);
            if (!confirmLoginResult.Succeeded) return Result.ValidationError(confirmLoginResult.Errors);
            
            return Result.Success;
        }
    }
}