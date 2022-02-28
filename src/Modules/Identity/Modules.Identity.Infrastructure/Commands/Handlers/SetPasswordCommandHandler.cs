using Microsoft.AspNetCore.Identity;
using VShop.SharedKernel.Infrastructure;

using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class SetPasswordCommandHandler : ICommandHandler<SetPasswordCommand>
    {
        private readonly ApplicationUserManager _userManager;

        public SetPasswordCommandHandler(ApplicationUserManager userManager) 
            => _userManager = userManager;

        public async Task<Result> HandleAsync
        (
            SetPasswordCommand command,
            CancellationToken cancellationToken
        )
        {
            (Guid userId, string password) = command;
            
            User user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result.NotFoundError("User not found.");

            IdentityResult result = await _userManager.AddPasswordAsync(user, password);
            if (!result.Succeeded) return Result.ValidationError(result.Errors);

            return Result.Success;
        }
    }
}