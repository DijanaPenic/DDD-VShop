using NodaTime;
using NodaTime.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.Modules.Identity.Infrastructure.DAL.Stores.Contracts;

namespace VShop.Modules.Identity.Infrastructure.DAL.Stores;

internal sealed class ApplicationUserStore :
    IUserPasswordStore<User>,
    IUserEmailStore<User>,
    IUserRoleStore<User>,
    IUserSecurityStampStore<User>,
    IUserClaimStore<User>,
    IUserTwoFactorStore<User>,
    IUserPhoneNumberStore<User>,
    IUserLockoutStore<User>,
    IUserAuthenticationTokenStore<User>,
    IUserAuthenticatorKeyStore<User>,
    IUserTwoFactorRecoveryCodeStore<User>,

    // Custom implementation
    IApplicationUserStore,
    IApplicationLoginUserStore
{
    private const string InternalLoginProvider = "[AspNetUserStore]";
    private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
    private const string RecoveryCodeTokenName = "RecoveryCodes";
    private readonly IdentityDbContext _dbContext;

    public ApplicationUserStore(IdentityDbContext dbContext) => _dbContext = dbContext;

    #region UserStore<User> Members

    public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
                throw new ArgumentNullException(nameof(user));

            await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError {Code = ex.Message, Description = ex.Message});
        }
    }

    public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
                throw new ArgumentNullException(nameof(user));

            await _dbContext.DeleteByKeyAsync<User>(cancellationToken, user.Id);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError {Code = ex.Message, Description = ex.Message});
        }
    }

    public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        if (!Guid.TryParse(userId, out Guid id))
            throw new ArgumentOutOfRangeException(nameof(userId), $"{nameof(userId)} is not a valid GUID");

        User user = await _dbContext.FindByKeyAsync<User>(cancellationToken, id);

        return user;
    }

    public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        User user = await _dbContext.Users.Where(r => r.NormalizedUserName == normalizedUserName)
            .SingleOrDefaultAsync(cancellationToken);

        return user;
    }

    public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.NormalizedUserName);
    }

    public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.Id.ToString());
    }

    public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.UserName);
    }

    public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        user.NormalizedUserName = normalizedName;

        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        user.UserName = userName;

        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
                throw new ArgumentNullException(nameof(user));

            await _dbContext.UpdateAsync(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError {Code = ex.Message, Description = ex.Message});
        }
    }

    #endregion

    #region UserPasswordStore<User> Members

    public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        user.PasswordHash = passwordHash;

        return Task.CompletedTask;
    }

    public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.PasswordHash);
    }

    public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
    }

    #endregion

    #region UserEmailStore<User> Members

    public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        user.Email = email;

        return Task.CompletedTask;
    }

    public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.EmailConfirmed);
    }

    public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        user.EmailConfirmed = confirmed;

        return Task.CompletedTask;
    }

    public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(normalizedEmail))
            throw new ArgumentNullException(nameof(normalizedEmail));

        User user = await _dbContext.Users.Where(r => r.NormalizedEmail == normalizedEmail)
            .SingleOrDefaultAsync(cancellationToken);

        return user;
    }

    public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.NormalizedEmail);
    }

    public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        user.NormalizedEmail = normalizedEmail;

        return Task.CompletedTask;
    }

    #endregion

    #region UserLoginStore<User> Members

    public async Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        if (login is null)
            throw new ArgumentNullException(nameof(login));

        UserLogin loginEntity = new()
        {
            LoginProvider = login.LoginProvider,
            ProviderDisplayName = login.ProviderDisplayName,
            ProviderKey = login.ProviderKey,
            UserId = user.Id,
            IsConfirmed = true // Token is not issued, so confirmation is not required
        };

        await _dbContext.AddAsync(loginEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveLoginAsync
    (
        User user,
        string loginProvider,
        string providerKey,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrWhiteSpace(loginProvider))
            throw new ArgumentNullException(nameof(loginProvider));

        if (string.IsNullOrWhiteSpace(providerKey))
            throw new ArgumentNullException(nameof(providerKey));

        await _dbContext.DeleteByKeyAsync<UserLogin>(cancellationToken, new UserLoginKey
            { LoginProvider = loginProvider, ProviderKey = providerKey }.ToArray());
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        IList<UserLoginInfo> result = await _dbContext.UserLogins.Where(ul => ul.UserId == user.Id)
            .Select(ul => new UserLoginInfo(ul.LoginProvider, ul.ProviderKey, ul.ProviderDisplayName))
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<User> FindByLoginAsync
    (
        string loginProvider,
        string providerKey,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(loginProvider))
            throw new ArgumentNullException(nameof(loginProvider));

        if (string.IsNullOrWhiteSpace(providerKey))
            throw new ArgumentNullException(nameof(providerKey));

        UserLogin login = await _dbContext.FindByKeyAsync<UserLogin>(cancellationToken, new UserLoginKey
            {LoginProvider = loginProvider, ProviderKey = providerKey}.ToArray());
        if (login is null)
            return default;

        User user = await _dbContext.FindByKeyAsync<User>(cancellationToken, login.UserId);

        return user;
    }

    #endregion

    #region IApplicationUserLoginStore<User> Members

    public async Task AddLoginAsync(User user, UserLoginInfo login, string token, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        if (login is null)
            throw new ArgumentNullException(nameof(login));

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentNullException(nameof(token));

        UserLogin loginEntity = new()
        {
            LoginProvider = login.LoginProvider,
            ProviderDisplayName = login.ProviderDisplayName,
            ProviderKey = login.ProviderKey,
            UserId = user.Id,
            Token = token,
            IsConfirmed = false
        };

        await _dbContext.AddAsync(loginEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateLoginAsync(UserLogin login, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (login is null)
            throw new ArgumentNullException(nameof(login));

        await _dbContext.UpdateAsync(login);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserLogin> FindLoginAsync(UserLoginInfo login, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (login is null)
            throw new ArgumentNullException(nameof(login));

        UserLogin result = await _dbContext.FindByKeyAsync<UserLogin>(cancellationToken, new UserLoginKey
            {LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey}.ToArray());

        return result;
    }

    public async Task<UserLogin> FindLoginAsync(User user, string token, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));

        UserLogin result = await _dbContext.UserLogins
            .SingleOrDefaultAsync(ul => ul.UserId == user.Id && ul.Token == token, cancellationToken);

        return result;
    }

    public async Task<User> FindByLoginAsync
    (
        UserLoginInfo login,
        bool loginConfirmed,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (login is null)
            throw new ArgumentNullException(nameof(login));

        UserLogin loginEntity = await _dbContext.FindByKeyAsync<UserLogin>(cancellationToken, new UserLoginKey
            {LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey}.ToArray());
        if (loginEntity is null || loginEntity.IsConfirmed != loginConfirmed)
            return default;

        User result = await _dbContext.FindByKeyAsync<User>(cancellationToken, loginEntity.UserId);

        return result;
    }

    public async Task ConfirmLoginAsync(UserLogin login, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (login is null)
            throw new ArgumentNullException(nameof(login));

        login.IsConfirmed = true;

        await _dbContext.UpdateAsync(login);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region UserRoleStore<User> Members

    public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentNullException(nameof(roleName));

        Role role = await _dbContext.Roles
            .SingleAsync(r => r.NormalizedName == roleName, cancellationToken);
        
        UserRole userRole = new()
        {
            RoleId = role.Id,
            UserId = user.Id
        };
        
        await _dbContext.UserRoles.AddAsync(userRole, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentNullException(nameof(roleName));

        Role role = await _dbContext.Roles
            .SingleAsync(r => r.NormalizedName == roleName, cancellationToken);

        UserRole userRole = await _dbContext.UserRoles
            .SingleAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id, cancellationToken);

        await _dbContext.DeleteAsync(userRole);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        IList<string> result = await _dbContext.UserRoles.Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Role.NormalizedName)
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentNullException(nameof(roleName));

        bool result = await _dbContext.UserRoles.Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Role.NormalizedName)
            .AnyAsync(r => r == roleName, cancellationToken);

        return result;
    }

    public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentNullException(nameof(roleName));

        IList<User> result = await _dbContext.UserRoles.Where(ur => ur.Role.NormalizedName == roleName)
            .Select(ur => ur.User)
            .ToListAsync(cancellationToken);

        return result;
    }

    #endregion

    #region UserSecurityStampStore<User> Members

    public Task SetSecurityStampAsync(User user, string stamp, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        user.SecurityStamp = stamp;

        return Task.CompletedTask;
    }

    public Task<string> GetSecurityStampAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.SecurityStamp);
    }

    #endregion

    #region UserClaimStore<User> Members

    public async Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        IList<Claim> result = await _dbContext.UserClaims.Where(uc => uc.UserId == user.Id)
            .Select(uc => new Claim(uc.ClaimType, uc.ClaimValue))
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        if (claims is null)
            throw new ArgumentNullException(nameof(claims));

        List<UserClaim> userClaims = claims.Select(c => GetUserClaimEntity(c, user.Id)).ToList();

        if (userClaims.Any())
        {
            async void Action(UserClaim userClaim)
            {
                await _dbContext.AddAsync(userClaim);
            }

            userClaims.ForEach(Action);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        if (claim is null)
            throw new ArgumentNullException(nameof(claim));

        if (newClaim is null)
            throw new ArgumentNullException(nameof(newClaim));

        UserClaim claimEntity = await _dbContext.UserClaims.SingleOrDefaultAsync(uc =>
            uc.UserId == user.Id && uc.ClaimType == claim.Type && uc.ClaimValue == claim.Value, cancellationToken);

        if (claimEntity != null)
        {
            claimEntity.ClaimType = newClaim.Type;
            claimEntity.ClaimValue = newClaim.Value;

            await _dbContext.UpdateAsync(claimEntity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        if (claims is null)
            throw new ArgumentNullException(nameof(claims));

        IEnumerable<UserClaim> userClaims = _dbContext.UserClaims.Where(uc => uc.UserId == user.Id);
        IEnumerable<Claim> enumerable = claims as Claim[] ?? claims.ToArray();
        
        if (enumerable.Any())
        {
            async void Action(Claim userClaim)
            {
                UserClaim userClaimEntity = userClaims
                    .SingleOrDefault(uc => uc.ClaimType == userClaim.Type && uc.ClaimValue == userClaim.Value);

                if (userClaimEntity != null) await _dbContext.DeleteByKeyAsync<UserClaim>(cancellationToken, userClaimEntity.Id);
            }

            enumerable.ToList().ForEach(Action);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (claim is null)
            throw new ArgumentNullException(nameof(claim));
        
        IList<User> result  = await _dbContext.UserClaims
            .Where(uc => uc.ClaimType == claim.Type && uc.ClaimValue == claim.Value).Select(uc => uc.User)
            .ToListAsync(cancellationToken);

        return result;
    }

    #endregion

    #region UserAuthenticationTokenStore<User> Members

    public async Task SetTokenAsync(User user, string loginProvider, string name, string value,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrWhiteSpace(loginProvider))
            throw new ArgumentNullException(nameof(loginProvider));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        UserToken userToken = await _dbContext.FindByKeyAsync<UserToken>(cancellationToken, new UserTokenKey
            {UserId = user.Id, LoginProvider = loginProvider, Name = name}.ToArray());
        if (userToken is null)
        {
            userToken = new UserToken
            {
                LoginProvider = loginProvider,
                Name = name,
                Value = value,
                UserId = user.Id
            };

            await _dbContext.AddAsync(userToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            userToken.Value = value;

            await _dbContext.UpdateAsync(userToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RemoveTokenAsync(User user, string loginProvider, string name,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrWhiteSpace(loginProvider))
            throw new ArgumentNullException(nameof(loginProvider));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        UserToken userToken = await _dbContext.FindByKeyAsync<UserToken>(cancellationToken, new UserTokenKey
            {UserId = user.Id, LoginProvider = loginProvider, Name = name}.ToArray());

        if (userToken is not null)
        {
            await _dbContext.DeleteAsync(userToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<string> GetTokenAsync(User user, string loginProvider, string name,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrWhiteSpace(loginProvider))
            throw new ArgumentNullException(nameof(loginProvider));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        UserToken userToken = await _dbContext.FindByKeyAsync<UserToken>(cancellationToken, new UserTokenKey
            {UserId = user.Id, LoginProvider = loginProvider, Name = name}.ToArray());

        return userToken?.Value;
    }

    #endregion

    #region UserTwoFactorStore<User> Members

    public Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        user.TwoFactorEnabled = enabled;

        return Task.CompletedTask;
    }

    public Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.TwoFactorEnabled);
    }

    #endregion

    #region UserPhoneNumberStore<User> Members

    public Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        user.PhoneNumber = phoneNumber;

        return Task.CompletedTask;
    }

    public Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.PhoneNumber);
    }

    public Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.PhoneNumberConfirmed);
    }

    public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        user.PhoneNumberConfirmed = confirmed;

        return Task.CompletedTask;
    }

    #endregion

    #region UserLockoutStore<User> Members

    public Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        Instant? lockoutEndDate = user.LockoutEndDate;
        DateTimeOffset? result = default;

        if (lockoutEndDate.HasValue)
            result = lockoutEndDate.Value.ToDateTimeOffset();

        return Task.FromResult(result);
    }

    public Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        user.LockoutEndDate = lockoutEnd is null ? null : Instant.FromDateTimeOffset(lockoutEnd.Value);

        return Task.CompletedTask;
    }

    public Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(++user.AccessFailedCount);
    }

    public Task ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        user.AccessFailedCount = 0;

        return Task.CompletedTask;
    }

    public Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.AccessFailedCount);
    }

    public Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.LockoutEnabled);
    }

    public Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        user.LockoutEnabled = enabled;

        return Task.CompletedTask;
    }

    #endregion

    #region IApplicationUserStore<User> Members

    public Task ApproveUserAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        user.IsApproved = true;

        return Task.CompletedTask;
    }

    public Task DisapproveUserAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        user.IsApproved = false;

        return Task.CompletedTask;
    }

    #endregion

    #region UserAuthenticatorKeyStore<User, Guid> Members

    public async Task<string> GetAuthenticatorKeyAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        string userToken =
            await GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);

        return userToken;
    }

    public Task SetAuthenticatorKeyAsync(User user, string key, CancellationToken cancellationToken)
        => SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);

    #endregion

    #region UserTwoFactorRecoveryCodeStore<User> Members

    public async Task<int> CountCodesAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        string mergedCodes =
            await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? string.Empty;

        return mergedCodes.Length > 0 ? mergedCodes.Split(';').Length : 0;
    }

    public async Task<bool> RedeemCodeAsync(User user, string code, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        if (code is null)
            throw new ArgumentNullException(nameof(code));

        string mergedCodes =
            await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? string.Empty;
        string[] splitCodes = mergedCodes.Split(';');

        if (!splitCodes.Contains(code)) return false;
        
        IList<string> updatedCodes = new List<string>(splitCodes.Where(s => s != code));
        await ReplaceCodesAsync(user, updatedCodes, cancellationToken);

        return true;

    }

    public Task ReplaceCodesAsync(User user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
    {
        string mergedCodes = string.Join(";", recoveryCodes);

        return SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
        // Lifetimes of dependencies are managed by the IoC container, so disposal here is unnecessary.
    }

    #endregion

    #region Private Methods

    private static UserClaim GetUserClaimEntity(Claim value, Guid userId)
    {
        return value is null
            ? default
            : new UserClaim
            {
                ClaimType = value.Type,
                ClaimValue = value.Value,
                UserId = userId
            };
    }

    #endregion
}