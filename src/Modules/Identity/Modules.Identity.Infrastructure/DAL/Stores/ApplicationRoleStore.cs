using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.DAL.Stores;

internal sealed class ApplicationRoleStore : IRoleClaimStore<Role>
{
    private readonly IdentityDbContext _dbContext;

    public ApplicationRoleStore(IdentityDbContext dbContext) => _dbContext = dbContext;

    #region RoleStore<Role> Members

    public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role is null)
                throw new ArgumentNullException(nameof(role));

            await _dbContext.AddAsync(role);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError {Code = ex.Message, Description = ex.Message});
        }
    }

    public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role is null)
                throw new ArgumentNullException(nameof(role));

            await _dbContext.DeleteByKeyAsync<Role>(cancellationToken, role.Id);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError {Code = ex.Message, Description = ex.Message});
        }
    }

    public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(roleId))
            throw new ArgumentNullException(nameof(roleId));

        if (!Guid.TryParse(roleId, out Guid id))
            throw new ArgumentOutOfRangeException(nameof(roleId), $"{nameof(roleId)} is not a valid GUID");

        Role role = await _dbContext.FindByKeyAsync<Role>(cancellationToken, id);

        return role;
    }

    public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(normalizedRoleName))
            throw new ArgumentNullException(nameof(normalizedRoleName));

        Role role = await _dbContext.Roles
            .SingleOrDefaultAsync(r => r.NormalizedName == normalizedRoleName, cancellationToken);

        return role;
    }

    public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (role is null)
            throw new ArgumentNullException(nameof(role));

        return Task.FromResult(role.NormalizedName);
    }

    public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (role is null)
            throw new ArgumentNullException(nameof(role));

        return Task.FromResult(role.Id.ToString());
    }

    public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (role is null)
            throw new ArgumentNullException(nameof(role));

        return Task.FromResult(role.Name);
    }

    public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (role is null)
            throw new ArgumentNullException(nameof(role));

        role.NormalizedName = normalizedName;

        return Task.CompletedTask;
    }

    public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (role is null)
            throw new ArgumentNullException(nameof(role));

        role.Name = roleName;

        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role is null)
                throw new ArgumentNullException(nameof(role));

            await _dbContext.UpdateAsync(role);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError {Code = ex.Message, Description = ex.Message});
        }
    }

    #endregion

    #region RoleClaimStore<Role> Members

    public Task<IList<Claim>> GetClaimsAsync(Role role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (role is null)
            throw new ArgumentNullException(nameof(role));

        IList<Claim> result = _dbContext.RoleClaims.Where(rc => rc.RoleId == role.Id)
            .Select(rc => new Claim(rc.ClaimType, rc.ClaimValue)).ToList();

        return Task.FromResult(result);
    }

    public async Task AddClaimAsync(Role role, Claim claim, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (role is null)
            throw new ArgumentNullException(nameof(role));

        if (claim is null)
            throw new ArgumentNullException(nameof(claim));

        RoleClaim roleClaim = new RoleClaim
        {
            ClaimType = claim.Type,
            ClaimValue = claim.Value,
            RoleId = role.Id
        };

        await _dbContext.AddAsync(roleClaim);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveClaimAsync(Role role, Claim claim, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (role is null)
            throw new ArgumentNullException(nameof(role));

        if (claim is null)
            throw new ArgumentNullException(nameof(claim));

        RoleClaim roleClaim = await _dbContext.RoleClaims.SingleOrDefaultAsync
        (
            rc => rc.RoleId == role.Id && rc.ClaimType == claim.Type && rc.ClaimValue == claim.Value,
            cancellationToken
        );

        if (roleClaim != null)
        {
            await _dbContext.DeleteByKeyAsync<RoleClaim>(cancellationToken, roleClaim.Id);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
    
    #endregion

    #region IDisposable Members

    public void Dispose()
    {
        // Lifetimes of dependencies are managed by the IoC container, so disposal here is unnecessary.
    }

    #endregion
}