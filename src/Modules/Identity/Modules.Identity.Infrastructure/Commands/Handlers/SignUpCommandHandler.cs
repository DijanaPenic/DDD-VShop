using Microsoft.AspNetCore.Identity;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Constants;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Integration.Events;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;

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
            User user = new()
            {
                Id = SequentialGuid.Create(),
                Email = command.Email,
                UserName = command.UserName,
                IsApproved = true
            };

            IdentityResult userResult = await _userManager.CreateAsync(user, command.Password);
            if (!userResult.Succeeded) return Result.ValidationError(userResult.Errors);
            
            IdentityResult roleResult = await _userManager.AddToRoleAsync(user, Roles.Customer);
            if (!roleResult.Succeeded) return Result.ValidationError(roleResult.Errors);

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            IIntegrationEvent integrationEvent = new CustomerSignedUpIntegrationEvent(user.Id, token);
            
            await _integrationEventService.SaveEventAsync(integrationEvent, cancellationToken);
            
            return Result.Success;
        }
    }
}