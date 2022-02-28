using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;

namespace VShop.Modules.Identity.Infrastructure.Queries.Handlers
{
    internal class IsClientAuthenticatedQueryHandler : IQueryHandler<IsClientAuthenticatedQuery, bool>
    {
        private readonly ApplicationAuthManager _authManager;

        public IsClientAuthenticatedQueryHandler(ApplicationAuthManager authManager)
            => _authManager = authManager;

        public async Task<Result<bool>> HandleAsync
        (
            IsClientAuthenticatedQuery query,
            CancellationToken cancellationToken
        )
        {
            (Guid clientId, string clientSecret) = query;
            
            string clientAuthResult = await _authManager.AuthenticateClientAsync
            (
                clientId,
                clientSecret,
                cancellationToken
            );
            
            if (!string.IsNullOrEmpty(clientAuthResult)) return Result.ValidationError(clientAuthResult);

            return true;
        }
    }
}