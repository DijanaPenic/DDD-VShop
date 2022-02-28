using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.Modules.Identity.Infrastructure.Models;

namespace VShop.Modules.Identity.Infrastructure.Queries.Handlers
{
    internal class GetAuthenticatorKeyQueryHandler : IQueryHandler<GetAuthenticatorKeyQuery, AuthenticatorKey>
    {
        private readonly ApplicationUserManager _userManager;

        public GetAuthenticatorKeyQueryHandler(ApplicationUserManager userManager)
            => _userManager = userManager;

        public async Task<Result<AuthenticatorKey>> HandleAsync
        (
            GetAuthenticatorKeyQuery query,
            CancellationToken cancellationToken
        )
        {
            User user = await _userManager.FindByIdAsync(query.UserId.ToString());
            if (user is null) return Result.NotFoundError("User not found.");

            string authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(authenticatorKey)) return Result.NotFoundError("Authenticator key not found.");

            return new AuthenticatorKey(authenticatorKey);
        }
    }
}