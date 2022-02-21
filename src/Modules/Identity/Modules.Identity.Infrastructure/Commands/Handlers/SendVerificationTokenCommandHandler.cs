using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.Modules.Identity.Infrastructure.Commands.Shared;
using VShop.Modules.Identity.Integration.Events;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class SendVerificationTokenCommandHandler : ICommandHandler<SendVerificationTokenCommand>
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IIntegrationEventService _integrationEventService;

        public SendVerificationTokenCommandHandler
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
            SendVerificationTokenCommand command,
            CancellationToken cancellationToken
        )
        {
            User user = await _userManager.FindByIdAsync(command.UserId.ToString());
            if (user is null) return Result.NotFoundError("User not found.");

            IIntegrationEvent integrationEvent;
            if (command.Type is AccountVerificationType.Email)
            {
                if (user.EmailConfirmed) return Result.ValidationError("Email is already confirmed.");
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                integrationEvent = new EmailVerificationTokenRequestedIntegrationEvent
                (
                    user.Id, token.Base64Encode(),
                    user.Email, command.ConfirmationUrl
                );
            }
            else
            {
                string phoneNumber = string.Concat(command.CountryCodeNumber, command.PhoneNumber);
                string token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber.GetDigits());
                
                integrationEvent = new PhoneNumberVerificationTokenRequestedIntegrationEvent
                (
                    user.Id, token,
                    phoneNumber, command.IsVoiceCall
                );
            }
            
            await _integrationEventService.SaveEventAsync(integrationEvent, cancellationToken);
            
            return Result.Success;
        }
    }
}