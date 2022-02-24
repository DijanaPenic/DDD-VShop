using Microsoft.AspNetCore.Identity;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.Modules.Identity.Infrastructure.Commands.Shared;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class VerifyCommandHandler : ICommandHandler<VerifyCommand>
    {
        private readonly ApplicationUserManager _userManager;

        public VerifyCommandHandler(ApplicationUserManager userManager) 
            => _userManager = userManager;

        public async Task<Result> Handle
        (
            VerifyCommand command,
            CancellationToken cancellationToken
        )
        {
            (Guid userId, AccountVerificationType type, string token, string phoneNumber) = command;
            
            User user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result.NotFoundError("User not found.");

            IdentityResult verificationResult;
            
            if (type is AccountVerificationType.Email)
                verificationResult = await _userManager.ConfirmEmailAsync(user, token);
            else
                verificationResult = await _userManager.ChangePhoneNumberAsync(user, phoneNumber.GetDigits(), token);

            if (!verificationResult.Succeeded) return Result.ValidationError(verificationResult.Errors);
            
            return Result.Success;
        }
    }
}