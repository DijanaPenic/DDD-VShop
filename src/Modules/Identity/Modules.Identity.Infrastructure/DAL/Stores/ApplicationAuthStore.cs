using Microsoft.EntityFrameworkCore;

using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.Modules.Identity.Infrastructure.DAL.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Infrastructure.Types;

namespace VShop.Modules.Identity.Infrastructure.DAL.Stores;

internal sealed class ApplicationAuthStore : IApplicationClientStore, IApplicationUserRefreshTokenStore
{
    private readonly IdentityDbContext _dbContext;
    private readonly IClockService _clockService;

    public ApplicationAuthStore(IdentityDbContext dbContext, IClockService clockService)
    {
        _dbContext = dbContext;
        _clockService = clockService;
    }

    #region IApplicationUserRefreshTokenStore Members

    public async Task AddRefreshTokenAsync(UserRefreshToken refreshToken, CancellationToken cancellationToken)
    {
        if (refreshToken is null)
            throw new ArgumentNullException(nameof(refreshToken));

        await _dbContext.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRefreshTokenByKeyAsync(UserRefreshTokenKey key, CancellationToken cancellationToken)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        await _dbContext.DeleteByKeyAsync<UserRefreshToken>(cancellationToken, key.ToArray());
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveExpiredRefreshTokensAsync(CancellationToken cancellationToken)
    {
        IList<UserRefreshToken> expired = await _dbContext.UserRefreshTokens
            .Where(urt => urt.DateExpires < _clockService.Now)
            .ToListAsync(cancellationToken);
        
        foreach (UserRefreshToken userRefreshToken in expired)
            await _dbContext.DeleteAsync(userRefreshToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserRefreshToken> FindRefreshTokenByValueAsync(string value, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));

        UserRefreshToken userRefreshToken = await _dbContext.UserRefreshTokens
            .SingleOrDefaultAsync(urt => urt.Value == value, cancellationToken);

        return userRefreshToken;
    }

    #endregion

    #region IApplicationClientStore Members

    public async Task<Client> FindClientByKeyAsync(Guid clientId, CancellationToken cancellationToken)
    {
        if (SequentialGuid.IsNullOrEmpty(clientId))
            throw new ArgumentNullException(nameof(clientId));

        Client client = await _dbContext.FindByKeyAsync<Client>(cancellationToken, clientId);

        return client;
    }

    public Task<Client> FindClientByNameAsync(string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        return _dbContext.Clients.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    #endregion
}