using Microsoft.AspNetCore.Authentication;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;
using VShop.Modules.Identity.Infrastructure.Models;
using VShop.Modules.Identity.Infrastructure.Services;

namespace VShop.Modules.Identity.Infrastructure.Queries.Handlers
{
    internal class GetExternalProvidersQueryHandler : IQueryHandler<GetExternalProvidersQuery, List<LoginProvider>>
    {
        private readonly ApplicationSignInManager _signInManager;

        public GetExternalProvidersQueryHandler(ApplicationSignInManager signInManager)
            => _signInManager = signInManager;

        public async Task<Result<List<LoginProvider>>> Handle
        (
            GetExternalProvidersQuery query,
            CancellationToken cancellationToken
        )
        {
            IEnumerable<AuthenticationScheme> schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
            List<LoginProvider> providerNames = schemes
                .Select(s => new LoginProvider(s.DisplayName))
                .ToList();

            return providerNames;
        }
    }
}