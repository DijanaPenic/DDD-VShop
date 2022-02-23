using Microsoft.AspNetCore.Authentication;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class InitiateExternalLoginCommandHandler : ICommandHandler<InitiateExternalLoginCommand, AuthenticationProperties>
    {
        private readonly ApplicationSignInManager _signInManager;

        public InitiateExternalLoginCommandHandler(ApplicationSignInManager signInManager) 
            => _signInManager = signInManager;

        public async Task<Result<AuthenticationProperties>> Handle
        (
            InitiateExternalLoginCommand command,
            CancellationToken cancellationToken
        )
        {
            (string provider, string returnUrl) = command;
            
            IEnumerable<AuthenticationScheme> schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
        
            AuthenticationScheme authScheme = schemes.SingleOrDefault(s => s.Name.Equals(provider));
            if (authScheme is null) return Result.NotFoundError("Provider not found.");

            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties
            (
                authScheme.Name,
                returnUrl
            );

            return properties;
        }
    }
}