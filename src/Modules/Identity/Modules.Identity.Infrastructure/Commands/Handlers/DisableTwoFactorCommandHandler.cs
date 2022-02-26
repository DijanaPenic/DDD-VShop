using Microsoft.AspNetCore.Identity;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class DisableTwoFactorCommandHandler : ICommandHandler<DisableTwoFactorCommand>
    {
        private readonly ApplicationUserManager _userManager;

        public DisableTwoFactorCommandHandler(ApplicationUserManager userManager) 
            => _userManager = userManager;

        public async Task<Result> Handle
        (
            DisableTwoFactorCommand command,
            CancellationToken cancellationToken
        )
        {
            User user = await _userManager.FindByIdAsync(command.UserId.ToString());
            if (user is null) return Result.NotFoundError("User not found.");

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
                return Result.ValidationError("Two-factor authentication is not enabled, so cannot disable it.");

            IdentityResult result = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!result.Succeeded) return Result.ValidationError(result.Errors);

            return Result.Success;
        }
    }
}