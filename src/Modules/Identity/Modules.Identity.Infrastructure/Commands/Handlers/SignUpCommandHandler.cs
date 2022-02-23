using Microsoft.AspNetCore.Identity;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.Modules.Identity.Integration.Events;
using VShop.Modules.Identity.Infrastructure.Constants;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.Modules.Identity.Infrastructure.Services;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class SignUpCommandHandler : ICommandHandler<SignUpCommand>
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IIntegrationEventService _integrationEventService;

        public SignUpCommandHandler
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
            SignUpCommand command,
            CancellationToken cancellationToken
        )
        {
            (string userName, string email, string password, _, string confirmationUrl) = command;
            
            User existingUser = await _userManager.FindByNameAsync(userName);
            if (existingUser is not null) 
                return Result.ValidationError($"A user with {userName} username already exists in the system.");
            
            User user = new()
            {
                Id = SequentialGuid.Create(),
                Email = email,
                UserName = userName,
                IsApproved = true
            };

            IdentityResult userResult = await _userManager.CreateAsync(user, password);
            if (!userResult.Succeeded) return Result.ValidationError(userResult.Errors);
            
            IdentityResult roleResult = await _userManager.AddToRoleAsync(user, Roles.Customer);
            if (!roleResult.Succeeded) return Result.ValidationError(roleResult.Errors);

            CustomerSignedUpIntegrationEvent customerSignedUpIntegrationEvent = new(user.Id);
            
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            AccountConfirmationRequestedIntegrationEvent confirmationRequestedIntegrationEvent = new
            (
                user.Id, email, 
                token, confirmationUrl
            );
            
            await _integrationEventService.SaveEventAsync(customerSignedUpIntegrationEvent, cancellationToken);
            await _integrationEventService.SaveEventAsync(confirmationRequestedIntegrationEvent, cancellationToken);

            return Result.Success;
        }
    }
}