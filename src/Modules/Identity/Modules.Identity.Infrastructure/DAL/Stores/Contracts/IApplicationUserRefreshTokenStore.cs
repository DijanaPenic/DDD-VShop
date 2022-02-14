using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.DAL.Stores.Contracts;

internal interface IApplicationUserRefreshTokenStore
{
    Task AddRefreshTokenAsync(UserRefreshToken refreshToken, CancellationToken cancellationToken);
    Task RemoveRefreshTokenByKeyAsync(UserRefreshTokenKey key, CancellationToken cancellationToken);
    Task RemoveExpiredRefreshTokensAsync(CancellationToken cancellationToken);
    Task<UserRefreshToken> FindRefreshTokenByValueAsync(string value, CancellationToken cancellationToken);
}