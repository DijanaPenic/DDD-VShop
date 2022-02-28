using Microsoft.AspNetCore.Identity;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
    {
        private readonly ApplicationUserManager _userManager;

        public ResetPasswordCommandHandler(ApplicationUserManager userManager)
            => _userManager = userManager;

        public async Task<Result> HandleAsync
        (
            ResetPasswordCommand command,
            CancellationToken cancellationToken
        )
        {
            User user = await _userManager.FindByIdAsync(command.UserId.ToString());
            if (user is null) return Result.NotFoundError("User not found.");

            IdentityResult result = await _userManager.ResetPasswordAsync
            (
                user,
                command.Token,
                command.Password
            );
            if (!result.Succeeded) return Result.ValidationError(result.Errors);

            return Result.Success;
        }
    }
}