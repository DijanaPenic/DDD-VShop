using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;
using VShop.Modules.Identity.Infrastructure.DAL;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;

using ExternalLoginInfo = VShop.Modules.Identity.Infrastructure.Models.ExternalLoginInfo;

namespace VShop.Modules.Identity.Infrastructure.Queries.Handlers
{
    internal class GetExternalLoginQueryHandler : IQueryHandler<GetExternalLoginQuery, List<ExternalLoginInfo>>
    {
        private readonly IdentityDbContext _dbContext;

        public GetExternalLoginQueryHandler(IdentityDbContext dbContext)
            => _dbContext = dbContext;

        public async Task<Result<List<ExternalLoginInfo>>> HandleAsync
        (
            GetExternalLoginQuery query,
            CancellationToken cancellationToken
        )
        {
            User user = await _dbContext.FindByKeyAsync<User>(cancellationToken, query.UserId);
            if (user is null) return Result.NotFoundError("User not found.");
            
            List<ExternalLoginInfo> userLogins = (await _dbContext.UserLogins
                .Where(ul => ul.UserId == query.UserId && ul.IsConfirmed == true)
                .ToListAsync(cancellationToken))
                .Select(ul => new ExternalLoginInfo
                (
                    ul.LoginProvider,
                    ul.ProviderKey,
                    ul.ProviderDisplayName
                ))
                .ToList();

            return userLogins;
        }
    }
}