using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;

using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.Modules.Identity.Infrastructure.DAL.Stores.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Services;

internal sealed class ApplicationUserManager : UserManager<User>
{
    private readonly IApplicationUserStore _userStore;
    private readonly IApplicationLoginUserStore _loginStore;
    private readonly IUserPasswordStore<User> _passwordStore;
    private readonly IUserRoleStore<User> _userRoleStore;

    public ApplicationUserManager
    (
        IUserStore<User> userStore,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<User> passwordHasher,
        IEnumerable<UserValidator<User>> userValidators,
        IEnumerable<IPasswordValidator<User>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<ApplicationUserManager> logger
    ) : base(userStore, optionsAccessor, passwordHasher, userValidators, 
            passwordValidators, keyNormalizer, errors, services, logger)
    {
        _userStore = (IApplicationUserStore)userStore;
        _loginStore = (IApplicationLoginUserStore)userStore;
        _passwordStore = (IUserPasswordStore<User>)userStore;
        _userRoleStore = (IUserRoleStore<User>)userStore;
    }

    public async Task<IdentityResult> AddToRoleAsync(string normalizedUserName, string role)
    {
        User user = await _userStore.FindByNameAsync(normalizedUserName, CancellationToken);
        if (user is null)
            return IdentityResult.Failed(new IdentityError { Code = "User Update", Description = "User not found." });

        string normalizedRole = NormalizeName(role);
        if (await _userRoleStore.IsInRoleAsync(user, normalizedRole, CancellationToken))
        {
            Logger.LogWarning(5, "User {userId} is already in role {role}.", await GetUserIdAsync(user), role);
            return IdentityResult.Failed(ErrorDescriber.UserAlreadyInRole(role));
        }

        await _userRoleStore.AddToRoleAsync(user, normalizedRole, CancellationToken);
        return await UpdateUserAsync(user);
    }

    public async Task<IdentityResult> ChangePasswordAsync(User user, string newPassword)
    {
        foreach (IPasswordValidator<User> passwordValidator in PasswordValidators)
        {
            IdentityResult result = await passwordValidator.ValidateAsync(this, user, newPassword);
            if (!result.Succeeded) return result;
        }

        string newPasswordHash = PasswordHasher.HashPassword(user, newPassword);

        await _passwordStore.SetPasswordHashAsync(user, newPasswordHash, CancellationToken);
        return await _userStore.UpdateAsync(user, CancellationToken);
    }

    // User can initiate external login request multiple times, so need to support update of the existing login record.
    public async Task<IdentityResult> AddOrUpdateLoginAsync(User user, UserLoginInfo login, string token)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));
        if (login is null) throw new ArgumentNullException(nameof(login));

        try
        {
            UserLogin loginEntity = await _loginStore.FindLoginAsync(login, CancellationToken);

            // Insert new login
            if (loginEntity is null)
            {
                await _loginStore.AddLoginAsync(user, login, token, CancellationToken);
                return IdentityResult.Success;
            }

            // Update the existing login
            loginEntity.Token = token;

            await _loginStore.UpdateLoginAsync(loginEntity, CancellationToken);
            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError { Code = ex.Message, Description = ex.Message });
        }
    }

    public async Task<IdentityResult> ConfirmLoginAsync(User user, string token)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));

        try
        {
            UserLogin login = await _loginStore.FindLoginAsync(user, token, CancellationToken);
            if (login is null)
                return IdentityResult.Failed(new IdentityError
                    { Code = "External Login", Description = "External Login not found." });

            if (login.IsConfirmed)
                return IdentityResult.Failed(new IdentityError
                    { Code = "External Login", Description = "External Login is already confirmed." });

            await _loginStore.ConfirmLoginAsync(login, CancellationToken);
            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError {Code = ex.Message, Description = ex.Message});
        }
    }

    public async Task<IdentityResult> ApproveUserAsync(User user)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));

        await _userStore.ApproveUserAsync(user, CancellationToken);
        return await _userStore.UpdateAsync(user, CancellationToken);
    }

    public async Task<IdentityResult> DisapproveUserAsync(User user)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));

        await _userStore.DisapproveUserAsync(user, CancellationToken);
        return await _userStore.UpdateAsync(user, CancellationToken);
    }
}