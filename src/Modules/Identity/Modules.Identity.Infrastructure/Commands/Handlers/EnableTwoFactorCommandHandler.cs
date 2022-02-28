using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class EnableTwoFactorCommandHandlerCommandHandler : ICommandHandler<EnableTwoFactorCommand>
    {
        private readonly ApplicationUserManager _userManager;

        public EnableTwoFactorCommandHandlerCommandHandler(ApplicationUserManager userManager) 
            => _userManager = userManager;

        public async Task<Result> HandleAsync
        (
            EnableTwoFactorCommand command,
            CancellationToken cancellationToken
        )
        {
            (Guid userId, string code) = command;
            
            User user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result.NotFoundError("User not found.");

            bool isTwoFactorTokenValid = await _userManager.VerifyTwoFactorTokenAsync
            (
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                code
            );

            if (isTwoFactorTokenValid) await _userManager.SetTwoFactorEnabledAsync(user, true);
            else return Result.ValidationError("Verification Code is invalid.");

            return Result.Success;
        }
    }
}