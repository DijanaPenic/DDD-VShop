using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;
using VShop.Modules.Identity.Infrastructure.DAL;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.Queries.Handlers
{
    internal class GetUserQueryHandler : IQueryHandler<GetUserQuery, User>
    {
        private readonly IdentityDbContext _dbContext;

        public GetUserQueryHandler(IdentityDbContext dbContext)
            => _dbContext = dbContext;

        public async Task<Result<User>> HandleAsync
        (
            GetUserQuery query,
            CancellationToken cancellationToken
        )
        {
            User user = await _dbContext.FindByKeyAsync<User>(cancellationToken, query.UserId);
            if (user is null) return Result.NotFoundError("User not found.");

            return user;
        }
    }
}