using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.Services.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class SignOutCommandHandler : ICommandHandler<SignOutCommand>
    {
        private readonly ApplicationSignInManager _signInManager;
        private readonly IAuthenticationService _authService;

        public SignOutCommandHandler
        (
            ApplicationSignInManager signInManager,
            IAuthenticationService authService
        )
        {
            _signInManager = signInManager;
            _authService = authService;
        }

        public async Task<Result> HandleAsync
        (
            SignOutCommand command,
            CancellationToken cancellationToken
        )
        {
            await _signInManager.SignOutAsync();
            await _authService.FinalizeSignOutAsync();

            return Result.Success;
        }
    }
}