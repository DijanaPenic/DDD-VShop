using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class RenewRecoveryCodesCommandHandler : ICommandHandler<RenewRecoveryCodesCommand, RecoveryCodes>
    {
        private readonly ApplicationUserManager _userManager;

        public RenewRecoveryCodesCommandHandler(ApplicationUserManager userManager)
            => _userManager = userManager;

        public async Task<Result<RecoveryCodes>> Handle
        (
            RenewRecoveryCodesCommand command,
            CancellationToken cancellationToken
        )
        {
            (Guid userId, int number) = command;
            
            User user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result.NotFoundError("User not found.");
            
            IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, number);

            return new RecoveryCodes(recoveryCodes.ToArray());
        }
    }
}