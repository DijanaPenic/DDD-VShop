using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.Modules.Identity.Integration.Events;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class InitiatePasswordRecoveryCommandHandler : ICommandHandler<InitiatePasswordRecoveryCommand>
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IIntegrationEventService _integrationEventService;

        public InitiatePasswordRecoveryCommandHandler
        (
            ApplicationUserManager userManager,
            IIntegrationEventService integrationEventService
        )
        {
            _userManager = userManager;
            _integrationEventService = integrationEventService;
        }

        public async Task<Result> Handle
        (
            InitiatePasswordRecoveryCommand command,
            CancellationToken cancellationToken
        )
        {
            (string email, string confirmationUrl) = command;
            
            User user = await _userManager.FindByEmailAsync(email);
            if (user is null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return Result.NotFoundError("User not found.");
            
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            PasswordRecoveryRequestedIntegrationEvent integrationEvent = new()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Token = token.Base64Encode(),
                ConfirmationUrl = confirmationUrl
            };

            await _integrationEventService.SaveEventAsync(integrationEvent, cancellationToken);
            
            return Result.Success;
        }
    }
}