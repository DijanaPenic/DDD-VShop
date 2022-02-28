using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class DeleteExpiredRefreshTokensCommandHandler : ICommandHandler<DeleteExpiredRefreshTokensCommand>
    {
        private readonly ApplicationAuthManager _authManager;

        public DeleteExpiredRefreshTokensCommandHandler(ApplicationAuthManager authManager) 
            => _authManager = authManager;

        public async Task<Result> HandleAsync
        (
            DeleteExpiredRefreshTokensCommand command,
            CancellationToken cancellationToken
        )
        {
            await _authManager.RemoveExpiredRefreshTokensAsync(cancellationToken);
            return Result.Success;
        }
    }
}