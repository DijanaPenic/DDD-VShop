using Microsoft.AspNetCore.Identity;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class DisconnectExternalLoginCommandHandler : ICommandHandler<DisconnectExternalLoginCommand>
    {
        private readonly ApplicationUserManager _userManager;

        public DisconnectExternalLoginCommandHandler(ApplicationUserManager userManager) 
            => _userManager = userManager;

        public async Task<Result> HandleAsync
        (
            DisconnectExternalLoginCommand command,
            CancellationToken cancellationToken
        )
        {
            (Guid userId, string loginProvider, string providerKey) = command;
            
            User user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result.NotFoundError("User not found.");
            
            IdentityResult result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded) return Result.ValidationError(result.Errors);

            return Result.Success;
        }
    }
}