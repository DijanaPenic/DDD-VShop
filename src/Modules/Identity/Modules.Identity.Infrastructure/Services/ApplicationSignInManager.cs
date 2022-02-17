using Serilog;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;

using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.SharedKernel.Infrastructure.Auth.Constants;

namespace VShop.Modules.Identity.Infrastructure.Services;

internal sealed class ApplicationSignInManager
{
    private const string LoginProviderKey = "LoginProvider";
    private const string XsrfKey = "XsrfId";

    public ApplicationSignInManager
    (
        ApplicationUserManager userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<User> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<User> confirmation
    )
    {
        UserManager = userManager;
        _contextAccessor = contextAccessor;
        ClaimsFactory = claimsFactory;
        Options = optionsAccessor?.Value ?? new IdentityOptions();
        Logger = logger;
        _schemes = schemes;
        _confirmation = confirmation;
    }

    private HttpContext _context;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IAuthenticationSchemeProvider _schemes;
    private readonly IUserConfirmation<User> _confirmation;

    public ILogger Logger { get; }
    public ApplicationUserManager UserManager { get; }
    public IUserClaimsPrincipalFactory<User> ClaimsFactory { get; }
    public IdentityOptions Options { get; }
    public HttpContext Context
    {
        get
        {
            HttpContext context = _context ?? _contextAccessor?.HttpContext;
            if (context is null)
                throw new InvalidOperationException("HttpContext must not be null.");

            return context;
        }
        set => _context = value;
    }

    public async Task<ClaimsPrincipal> CreateUserPrincipalAsync(User user) 
        => await ClaimsFactory.CreateAsync(user);

    /// <summary>
    /// Returns a flag indicating whether the specified user can sign in.
    /// </summary>
    /// <param name="user">The user whose sign-in status should be returned.</param>
    /// <returns>
    /// The task object representing the asynchronous operation, containing a flag that is true
    /// if the specified user can sign-in, otherwise false.
    /// </returns>
    public async Task<bool> CanSignInAsync(User user)
    {
        if (Options.SignIn.RequireConfirmedEmail && !(await UserManager.IsEmailConfirmedAsync(user)))
        {
            Logger.Warning("User {userId} cannot sign in without a confirmed email.",
                await UserManager.GetUserIdAsync(user));

            return false;
        }

        if (Options.SignIn.RequireConfirmedPhoneNumber && !(await UserManager.IsPhoneNumberConfirmedAsync(user)))
        {
            Logger.Warning("User {userId} cannot sign in without a confirmed phone number.",
                await UserManager.GetUserIdAsync(user));

            return false;
        }

        if (Options.SignIn.RequireConfirmedAccount && !(await _confirmation.IsConfirmedAsync(UserManager, user)))
        {
            Logger.Warning("User {userId} cannot sign in without a confirmed account.",
                await UserManager.GetUserIdAsync(user));

            return false;
        }

        return true;
    }

    /// <summary>
    /// Signs the current user out of the application.
    /// </summary>
    public async Task SignOutAsync()
    {
        await Context.SignOutAsync(IdentityConstants.ExternalScheme);
        await Context.SignOutAsync(IdentityConstants.TwoFactorUserIdScheme);
        await Context.SignOutAsync(ApplicationIdentityConstants.AccountVerificationScheme);
    }

    /// <summary>
    /// Validates the security stamp for the specified <paramref name="principal"/> against
    /// the persisted stamp for the current user, as an asynchronous operation.
    /// </summary>
    /// <param name="principal">The principal whose stamp should be validated.</param>
    /// <returns>The task object representing the asynchronous operation. The task will contain the user
    /// if the stamp matches the persisted value, otherwise it will return false.</returns>
    public async Task<User> ValidateSecurityStampAsync(ClaimsPrincipal principal)
    {
        if (principal is null) return null;

        User user = await UserManager.GetUserAsync(principal);
        if (await ValidateSecurityStampAsync(user, principal.FindFirstValue(Options.ClaimsIdentity.SecurityStampClaimType))) 
            return user;

        Logger.Debug("Failed to validate a security stamp.");

        return null;
    }

    /// <summary>
    /// Validates the security stamp for the specified <paramref name="principal"/> from one of
    /// the two factor principals (remember client or user id) against
    /// the persisted stamp for the current user, as an asynchronous operation.
    /// </summary>
    /// <param name="principal">The principal whose stamp should be validated.</param>
    /// <returns>The task object representing the asynchronous operation. The task will contain the user
    /// if the stamp matches the persisted value, otherwise it will return false.</returns>
    public async Task<User> ValidateTwoFactorSecurityStampAsync(ClaimsPrincipal principal)
    {
        if (principal?.Identity?.Name is null) return null;

        User user = await UserManager.FindByIdAsync(principal.Identity.Name);
        if (await ValidateSecurityStampAsync(user, principal.FindFirstValue(Options.ClaimsIdentity.SecurityStampClaimType))) 
            return user;

        Logger.Debug("Failed to validate a security stamp.");

        return null;
    }

    /// <summary>
    /// Validates the security stamp for the specified <paramref name="user"/>. Will always return false
    /// if the userManager does not support security stamps.
    /// </summary>
    /// <param name="user">The user whose stamp should be validated.</param>
    /// <param name="securityStamp">The expected security stamp value.</param>
    /// <returns>True if the stamp matches the persisted value, otherwise it will return false.</returns>
    public async Task<bool> ValidateSecurityStampAsync(User user, string securityStamp)
        => user is not null &&
        // Only validate the security stamp if the store supports it
        (!UserManager.SupportsUserSecurityStamp || securityStamp == await UserManager.GetSecurityStampAsync(user));

    /// <summary>
    /// Attempts to sign in the specified <paramref name="user"/> and <paramref name="password"/> combination
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="user">The user to sign in.</param>
    /// <param name="password">The password to attempt to sign in with.</param>
    /// <param name="lockoutOnFailure">Flag indicating if the user account should be locked if the sign in fails.</param>
    /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult"/>
    /// for the sign-in attempt.</returns>
    public async Task<SignInResult> PasswordSignInAsync
    (
        Guid clientId,
        User user,
        string password,
        bool lockoutOnFailure
    )
    {
        if (user is null) throw new ArgumentNullException(nameof(user));

        SignInResult passwordAttempt = await CheckPasswordSignInAsync(clientId, user, password, lockoutOnFailure);
        if (!passwordAttempt.Succeeded) return passwordAttempt;
        
        return await SignInOrTwoFactorAsync(clientId, user);
    }

    /// <summary>
    /// Attempts to sign in the specified <paramref name="userName"/> and <paramref name="password"/> combination
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="userName">The user name to sign in.</param>
    /// <param name="password">The password to attempt to sign in with.</param>
    /// <param name="lockoutOnFailure">Flag indicating if the user account should be locked if the sign in fails.</param>
    /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult"/>
    /// for the sign-in attempt.</returns>
    public async Task<SignInResult> PasswordSignInAsync
    (
        Guid clientId,
        string userName,
        string password,
        bool lockoutOnFailure
    )
    {
        User user = await UserManager.FindByNameAsync(userName);
        if (user is null) return SignInResult.Failed;

        return await PasswordSignInAsync(clientId, user, password, lockoutOnFailure);
    }

    /// <summary>
    /// Attempts a password sign in for a user.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="user">The user to sign in.</param>
    /// <param name="password">The password to attempt to sign in with.</param>
    /// <param name="lockoutOnFailure">Flag indicating if the user account should be locked if the sign in fails.</param>
    /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult"/>
    /// for the sign-in attempt.</returns>
    /// <returns></returns>
    public async Task<SignInResult> CheckPasswordSignInAsync
    (
        Guid clientId,
        User user,
        string password,
        bool lockoutOnFailure
    )
    {
        if (user is null) throw new ArgumentNullException(nameof(user));

        SignInResult error = await PreSignInCheck(clientId, user);
        if (error is not null) return error;

        if (await UserManager.CheckPasswordAsync(user, password))
        {
            bool alwaysLockout = AppContext.TryGetSwitch
            (
                "Microsoft.AspNetCore.Identity.CheckPasswordSignInAlwaysResetLockoutOnSuccess",
                    out bool enabled
            ) && enabled;
            // Only reset the lockout when TFA is not enabled when not in quirks mode
            if (alwaysLockout || !await IsTfaEnabled(user)) await ResetLockout(user);

            return SignInResult.Success;
        }

        Logger.Warning("User {userId} failed to provide the correct password.",
            await UserManager.GetUserIdAsync(user));

        if (UserManager.SupportsUserLockout && lockoutOnFailure)
        {
            // If lockout is requested, increment access failed count which might lock out the user
            await UserManager.AccessFailedAsync(user);
            if (await UserManager.IsLockedOutAsync(user)) return await LockedOut(user);
        }

        return SignInResult.Failed;
    }

    /// <summary>
    /// Returns a flag indicating if the current client browser has been remembered by two factor authentication
    /// for the user attempting to login, as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user attempting to login.</param>
    /// <returns>
    /// The task object representing the asynchronous operation containing true if the browser has been remembered
    /// for the current user.
    /// </returns>
    public async Task<bool> IsTwoFactorClientRememberedAsync(User user)
    {
        string userId = await UserManager.GetUserIdAsync(user);
        AuthenticateResult result = await Context.AuthenticateAsync(IdentityConstants.TwoFactorRememberMeScheme);

        return (result?.Principal is not null && result.Principal.FindFirstValue(ClaimTypes.NameIdentifier) == userId);
    }

    /// <summary>
    /// Sets a flag on the browser to indicate the user has selected "Remember this browser" for two factor authentication purposes,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user who choose "remember this browser".</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public async Task RememberTwoFactorClientAsync(User user)
    {
        ClaimsPrincipal principal = await CreateTwoFactorRememberMePrincipal(user);

        await Context.SignInAsync
        (
            IdentityConstants.TwoFactorRememberMeScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true }
        );
    }

    /// <summary>
    /// Clears the "Remember this browser flag" from the current browser, as an asynchronous operation.
    /// </summary>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public Task ForgetTwoFactorClientAsync()
        => Context.SignOutAsync(IdentityConstants.TwoFactorRememberMeScheme);

    /// <summary>
    /// Signs in the user without two factor authentication using a two factor recovery code.
    /// </summary>
    /// <param name="recoveryCode">The two factor recovery code.</param>
    /// <returns></returns>
    public async Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode)
    {
        TwoFactorAuthenticationInfo twoFactorInfo = await GetTwoFactorInfoAsync();
        if (twoFactorInfo?.UserId is null) return SignInResult.Failed;

        User user = await UserManager.FindByIdAsync(twoFactorInfo.UserId);
        if (user is null) return SignInResult.Failed;

        IdentityResult result = await UserManager.RedeemTwoFactorRecoveryCodeAsync(user, recoveryCode);
        if (!result.Succeeded) return SignInResult.Failed;
        
        await DoTwoFactorSignInAsync(user, twoFactorInfo, rememberClient: false);
        return SignInResult.Success;

        // We don't protect against brute force attacks since codes are expected to be random.
    }

    private async Task DoTwoFactorSignInAsync
    (
        User user,
        TwoFactorAuthenticationInfo twoFactorInfo,
        bool rememberClient
    )
    {
        // When token is verified correctly, clear the access failed count used for lockout
        await ResetLockout(user);

        // Cleanup external cookie
        if (twoFactorInfo.LoginProvider is not null)
            await Context.SignOutAsync(IdentityConstants.ExternalScheme);

        // Cleanup two factor user id cookie
        await Context.SignOutAsync(IdentityConstants.TwoFactorUserIdScheme);
        if (rememberClient) await RememberTwoFactorClientAsync(user);
        
        // JWT token will be returned to the user.
    }

    /// <summary>
    /// Validates the sign in code from an authenticator app and creates and signs in the user, as an asynchronous operation.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="code">The two factor authentication code to validate.</param>
    /// <param name="rememberClient">Flag indicating whether the current browser should be remember, suppressing all further 
    /// two factor authentication prompts.</param>
    /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult"/>
    /// for the sign-in attempt.</returns>
    public async Task<SignInResult> TwoFactorAuthenticatorSignInAsync(Guid clientId, string code,
        bool rememberClient)
    {
        TwoFactorAuthenticationInfo twoFactorInfo = await GetTwoFactorInfoAsync();
        if (twoFactorInfo?.UserId is null) return SignInResult.Failed;

        User user = await UserManager.FindByIdAsync(twoFactorInfo.UserId);
        if (user is null) return SignInResult.Failed;

        SignInResult error = await PreSignInCheck(clientId, user);
        if (error is not null) return error;

        if (await UserManager.VerifyTwoFactorTokenAsync(user, Options.Tokens.AuthenticatorTokenProvider, code))
        {
            await DoTwoFactorSignInAsync(user, twoFactorInfo, rememberClient);
            return SignInResult.Success;
        }

        // If the token is incorrect, record the failure which also may cause the user to be locked out
        await UserManager.AccessFailedAsync(user);
        return SignInResult.Failed;
    }

    /// <summary>
    /// Validates the two factor sign in code and creates and signs in the user, as an asynchronous operation.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="provider">The two factor authentication provider to validate the code against.</param>
    /// <param name="code">The two factor authentication code to validate.</param>
    /// <param name="rememberClient">Flag indicating whether the current browser should be remember, suppressing all further 
    /// two factor authentication prompts.</param>
    /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult"/>
    /// for the sign-in attempt.</returns>
    public async Task<SignInResult> TwoFactorSignInAsync
    (
        Guid clientId,
        string provider,
        string code,
        bool rememberClient
    )
    {
        TwoFactorAuthenticationInfo twoFactorInfo = await GetTwoFactorInfoAsync();
        if (twoFactorInfo?.UserId is null) return SignInResult.Failed;

        User user = await UserManager.FindByIdAsync(twoFactorInfo.UserId);
        if (user is null) return SignInResult.Failed;

        SignInResult error = await PreSignInCheck(clientId, user);
        if (error is not null) return error;

        if (await UserManager.VerifyTwoFactorTokenAsync(user, provider, code))
        {
            await DoTwoFactorSignInAsync(user, twoFactorInfo, rememberClient);
            return SignInResult.Success;
        }

        // If the token is incorrect, record the failure which also may cause the user to be locked out
        await UserManager.AccessFailedAsync(user);
        return SignInResult.Failed;
    }

    /// <summary>
    /// Gets the user for the current two factor authentication login, as an asynchronous operation.
    /// </summary>
    /// <returns>The task object representing the asynchronous operation containing the user
    /// for the sign-in attempt.</returns>
    public async Task<User> GetTwoFactorAuthenticationUserAsync()
    {
        TwoFactorAuthenticationInfo info = await GetTwoFactorInfoAsync();
        if (info is null) return null;

        return await UserManager.FindByIdAsync(info.UserId);
    }

    /// <summary>
    /// Signs in a user via a previously registered third party login, as an asynchronous operation.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="loginProvider">The login provider to use.</param>
    /// <param name="providerKey">The unique provider identifier for the user.</param>
    /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult"/>
    /// for the sign-in attempt.</returns>
    public Task<SignInResult> ExternalLoginSignInAsync(Guid clientId, string loginProvider, string providerKey)
        => ExternalLoginSignInAsync(clientId, loginProvider, providerKey, bypassTwoFactor: false);

    /// <summary>
    /// Signs in a user via a previously registered third party login, as an asynchronous operation.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="loginProvider">The login provider to use.</param>
    /// <param name="providerKey">The unique provider identifier for the user.</param>
    /// <param name="bypassTwoFactor">Flag indicating whether to bypass two factor authentication.</param>
    /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult"/>
    /// for the sign-in attempt.</returns>
    public async Task<SignInResult> ExternalLoginSignInAsync
    (
        Guid clientId,
        string loginProvider,
        string providerKey,
        bool bypassTwoFactor
    )
    {
        User user = await UserManager.FindByLoginAsync(loginProvider, providerKey);
        if (user is null) return SignInResult.Failed;

        SignInResult error = await PreSignInCheck(clientId, user);
        if (error is not null) return error;

        return await SignInOrTwoFactorAsync(clientId, user, loginProvider, bypassTwoFactor);
    }

    /// <summary>
    /// Gets a collection of <see cref="AuthenticationScheme"/>s for the known external login providers.		
    /// </summary>		
    /// <returns>A collection of <see cref="AuthenticationScheme"/>s for the known external login providers.</returns>		
    public async Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
    {
        IEnumerable<AuthenticationScheme> schemes = await _schemes.GetAllSchemesAsync();
        return schemes.Where(s => !string.IsNullOrEmpty(s.DisplayName));
    }

    /// <summary>
    /// Gets the external login information for the current login, as an asynchronous operation.
    /// </summary>
    /// <param name="expectedXsrf">Flag indication whether a Cross Site Request Forgery token was expected in the current request.</param>
    /// <returns>The task object representing the asynchronous operation containing the <see name="ExternalLoginInfo"/>
    /// for the sign-in attempt.</returns>
    public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null)
    {
        AuthenticateResult auth = await Context.AuthenticateAsync(IdentityConstants.ExternalScheme);
        IDictionary<string, string> items = auth?.Properties?.Items;

        if (auth?.Principal is null || items is null || !items.ContainsKey(LoginProviderKey)) return null;

        if (expectedXsrf is not null)
        {
            if (!items.ContainsKey(XsrfKey)) return null;

            string userId = items[XsrfKey];
            if (userId != expectedXsrf) return null;
        }

        string providerKey = auth.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (providerKey is null) return null;

        string provider = items[LoginProviderKey];

        string providerDisplayName = (await GetExternalAuthenticationSchemesAsync())
                                     .FirstOrDefault(p => p.Name == provider)?.DisplayName ?? provider;

        return new ExternalLoginInfo(auth.Principal, provider, providerKey, providerDisplayName)
        {
            AuthenticationTokens = auth.Properties.GetTokens()
        };
    }

    /// <summary>
    /// Stores any authentication tokens found in the external authentication cookie into the associated user.
    /// </summary>
    /// <param name="externalLogin">The information from the external login provider.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing
    /// the <see cref="IdentityResult"/> of the operation.</returns>
    public async Task<IdentityResult> UpdateExternalAuthenticationTokensAsync(ExternalLoginInfo externalLogin)
    {
        if (externalLogin is null) throw new ArgumentNullException(nameof(externalLogin));

        if (externalLogin.AuthenticationTokens == null || !externalLogin.AuthenticationTokens.Any())
            return IdentityResult.Success;
        
        User user = await UserManager.FindByLoginAsync(externalLogin.LoginProvider, externalLogin.ProviderKey);
        if (user is null) return IdentityResult.Failed();

        foreach (AuthenticationToken token in externalLogin.AuthenticationTokens)
        {
            IdentityResult result = await UserManager.SetAuthenticationTokenAsync
            (
                user,
                externalLogin.LoginProvider,
                token.Name,
                token.Value
            );
            
            if (!result.Succeeded) return result;
        }

        return IdentityResult.Success;
    }

    /// <summary>
    /// Configures the redirect URL and user identifier for the specified external login <paramref name="provider"/>.
    /// </summary>
    /// <param name="provider">The provider to configure.</param>
    /// <param name="redirectUrl">The external login URL users should be redirected to during the login flow.</param>
    /// <param name="userId">The current user's identifier, which will be used to provide CSRF protection.</param>
    /// <returns>A configured <see cref="AuthenticationProperties"/>.</returns>
    public AuthenticationProperties ConfigureExternalAuthenticationProperties
    (
        string provider,
        string redirectUrl,
        string userId = null
    )
    {
        AuthenticationProperties properties = new()
        {
            RedirectUri = redirectUrl,
            Items =
            {
                [LoginProviderKey] = provider
            }
        };

        if (userId is not null) properties.Items[XsrfKey] = userId;

        return properties;
    }

    /// <summary>
    /// Creates a claims principal for the specified 2fa information.
    /// </summary>
    /// <param name="clientId">The client id.</param>
    /// <param name="userId">The user whose is logging in via 2fa.</param>
    /// <param name="loginProvider">The 2fa provider.</param>
    /// <returns>A <see cref="ClaimsPrincipal"/> containing the user 2fa information.</returns>
    private static ClaimsPrincipal CreateTwoFactorPrincipal(string clientId, string userId, string loginProvider)
    {
        ClaimsIdentity identity = new(IdentityConstants.TwoFactorUserIdScheme);

        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
        identity.AddClaim(new Claim(ApplicationClaimTypes.ClientIdentifier, clientId));

        if (loginProvider is not null)
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, loginProvider));

        return new ClaimsPrincipal(identity);
    }

    private async Task<ClaimsPrincipal> CreateTwoFactorRememberMePrincipal(User user)
    {
        string userId = await UserManager.GetUserIdAsync(user);
        ClaimsIdentity rememberBrowserIdentity = new(IdentityConstants.TwoFactorRememberMeScheme);
        rememberBrowserIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));

        if (!UserManager.SupportsUserSecurityStamp) return new ClaimsPrincipal(rememberBrowserIdentity);
        
        string stamp = await UserManager.GetSecurityStampAsync(user);
        rememberBrowserIdentity.AddClaim(new Claim(Options.ClaimsIdentity.SecurityStampClaimType, stamp));

        return new ClaimsPrincipal(rememberBrowserIdentity);
    }

    private async Task<bool> IsTfaEnabled(User user)
        => UserManager.SupportsUserTwoFactor &&
        await UserManager.GetTwoFactorEnabledAsync(user) &&
        (await UserManager.GetValidTwoFactorProvidersAsync(user)).Count > 0;

    /// <summary>
    /// Signs in the specified <paramref name="user"/> if <paramref name="bypassTwoFactor"/> is set to false.
    /// Otherwise stores the <paramref name="user"/> for use after a two factor check.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="user"></param>
    /// <param name="loginProvider">The login provider to use. Default is null</param>
    /// <param name="bypassTwoFactor">Flag indicating whether to bypass two factor authentication. Default is false</param>
    /// <returns>Returns a <see cref="SignInResult"/></returns>
    public async Task<SignInResult> SignInOrTwoFactorAsync
    (
        Guid clientId,
        User user,
        string loginProvider = null,
        bool bypassTwoFactor = false
    )
    {
        if (!bypassTwoFactor && await IsTfaEnabled(user) && !await IsTwoFactorClientRememberedAsync(user))
        {
            // Store the userId for use after two factor check
            string userId = await UserManager.GetUserIdAsync(user);

            await Context.SignInAsync
            (
                IdentityConstants.TwoFactorUserIdScheme,
                CreateTwoFactorPrincipal(clientId.ToString(), userId, loginProvider)
            );

            return SignInResult.TwoFactorRequired;
        }

        // Cleanup external cookie
        if (loginProvider is not null)
            await Context.SignOutAsync(IdentityConstants.ExternalScheme);
        
        // else - JWT token will be returned to the user.

        return SignInResult.Success;
    }

    public async Task<TwoFactorAuthenticationInfo> GetTwoFactorInfoAsync()
    {
        AuthenticateResult result = await Context.AuthenticateAsync(IdentityConstants.TwoFactorUserIdScheme);
        if (result?.Principal is not null)
        {
            return new TwoFactorAuthenticationInfo
            {
                UserId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier),
                LoginProvider = result.Principal.FindFirstValue(ClaimTypes.AuthenticationMethod),
                ClientId = result.Principal.FindFirstValue(ApplicationClaimTypes.ClientIdentifier)
            };
        }

        return null;
    }

    /// <summary>
    /// Used to determine if a user is considered locked out.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Whether a user is considered locked out.</returns>
    public async Task<bool> IsLockedOut(User user)
        => UserManager.SupportsUserLockout && await UserManager.IsLockedOutAsync(user);

    /// <summary>
    /// Returns a locked out SignInResult.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>A locked out SignInResult</returns>
    public async Task<SignInResult> LockedOut(User user)
    {
        Logger.Warning("User {userId} is currently locked out.", await UserManager.GetUserIdAsync(user));
        return SignInResult.LockedOut;
    }

    /// <summary>
    /// Used to ensure that a user is allowed to sign in.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="user">The user</param>
    /// <returns>Null if the user should be allowed to sign in, otherwise the SignInResult why they should be denied.</returns>
    public async Task<SignInResult> PreSignInCheck(Guid clientId, User user)
    {
        if (!await CanSignInAsync(user))
        {
            string userId = await UserManager.GetUserIdAsync(user);
            await Context.SignInAsync
            (
                ApplicationIdentityConstants.AccountVerificationScheme,
                CreateAccountVerificationPrincipal(clientId.ToString(), userId)
            );

            return SignInResult.NotAllowed;
        }

        if (await IsLockedOut(user)) return await LockedOut(user);
        
        return null;
    }

    /// <summary>
    /// Used to reset a user's lockout count.
    /// </summary>
    /// <param name="user">The user</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing
    /// the <see cref="IdentityResult"/> of the operation.</returns>
    public Task ResetLockout(User user)
        => UserManager.SupportsUserLockout ? UserManager.ResetAccessFailedCountAsync(user) : Task.CompletedTask;
    
    public async Task<SignInResult> AccountVerificationSignInAsync(Guid clientId)
    {
        User user = await GetAccountVerificationUserAsync();
        if (user is null) return SignInResult.Failed;

        if (!await CanSignInAsync(user)) return SignInResult.NotAllowed;
        if (await IsLockedOut(user)) return await LockedOut(user);

        // Cleanup the account verification user id cookie.
        await Context.SignOutAsync(ApplicationIdentityConstants.AccountVerificationScheme);

        return await SignInOrTwoFactorAsync(clientId, user);
    }

    public async Task<User> GetAccountVerificationUserAsync()
    {
        AccountVerificationInfo verificationInfo = await GetAccountVerificationInfoAsync();

        if (verificationInfo?.UserId is not null) return await UserManager.FindByIdAsync(verificationInfo.UserId);
        
        return default;
    }

    public async Task<AccountVerificationInfo> GetAccountVerificationInfoAsync()
    {
        AuthenticateResult result = await Context.AuthenticateAsync(ApplicationIdentityConstants.AccountVerificationScheme);
        if (result?.Principal is not null)
            return new AccountVerificationInfo
            {
                UserId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier),
                ClientId = result.Principal.FindFirstValue(ApplicationClaimTypes.ClientIdentifier)
            };

        return null;
    }

    private static ClaimsPrincipal CreateAccountVerificationPrincipal(string clientId, string userId)
    {
        ClaimsIdentity identity = new(ApplicationIdentityConstants.AccountVerificationScheme);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
        identity.AddClaim(new Claim(ApplicationClaimTypes.ClientIdentifier, clientId));

        return new ClaimsPrincipal(identity);
    }
}

internal class TwoFactorAuthenticationInfo
{
    public string UserId { get; init; }
    public string LoginProvider { get; init; }
    public string ClientId { get; init; }
}

internal class AccountVerificationInfo
{
    public string UserId { get; init; }
    public string ClientId { get; init; }
}