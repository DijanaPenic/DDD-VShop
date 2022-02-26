using Microsoft.AspNetCore.Authentication;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;

namespace VShop.Modules.Identity.Infrastructure.Queries.Handlers
{
    internal class GetExternalProvidersQueryHandler : IQueryHandler<GetExternalProvidersQuery, List<string>>
    {
        private readonly ApplicationSignInManager _signInManager;

        public GetExternalProvidersQueryHandler(ApplicationSignInManager signInManager)
            => _signInManager = signInManager;

        public async Task<Result<List<string>>> Handle
        (
            GetExternalProvidersQuery query,
            CancellationToken cancellationToken
        )
        {
            IEnumerable<AuthenticationScheme> schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
            List<string> providerNames = schemes.Select(s => s.DisplayName).ToList();

            return providerNames;
        }
    }
}