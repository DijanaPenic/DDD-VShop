using Microsoft.AspNetCore.Identity;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class SignInExternalCommandHandler : ICommandHandler<SignInExternalCommand>
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly IIntegrationEventService _integrationEventService;

        public SignInExternalCommandHandler
        (
            ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            IIntegrationEventService integrationEventService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _integrationEventService = integrationEventService;
        }

        public async Task<Result> Handle
        (
            SignInExternalCommand command,
            CancellationToken cancellationToken
        )
        {
            ExternalLoginInfo externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo is null) return Result.Unauthorized("External authentication has failed.");

            return  Result.Success;
        }
    }
}