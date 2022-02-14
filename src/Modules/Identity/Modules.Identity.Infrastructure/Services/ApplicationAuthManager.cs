using NodaTime;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

using VShop.SharedKernel.Infrastructure.Auth;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.Modules.Identity.Infrastructure.Constants;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.Modules.Identity.Infrastructure.DAL.Stores.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Services;

internal sealed class ApplicationAuthManager
{
    private static readonly Dictionary<string, IEnumerable<string>> EmptyClaims = new();
    private readonly ApplicationUserManager _userManager;
    private readonly IApplicationClientStore _clientStore;
    private readonly IApplicationUserRefreshTokenStore _refreshTokenStore;
    private readonly AuthOptions _options;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly IClockService _clockService;
    private readonly SigningCredentials _signingCredentials;

    public ApplicationAuthManager
    (
        ApplicationUserManager userManager,
        IApplicationClientStore clientStore,
        IApplicationUserRefreshTokenStore refreshTokenStore,
        AuthOptions options,
        TokenValidationParameters tokenValidationParameters, 
        IClockService clockService
    )
    {
        string issuerSigningKey = options.IssuerSigningKey;
        if (issuerSigningKey is null)
            throw new InvalidOperationException("Issuer signing key not set.");

        _userManager = userManager;
        _clientStore = clientStore;
        _refreshTokenStore = refreshTokenStore;
        _options = options;
        _tokenValidationParameters = tokenValidationParameters;
        _clockService = clockService;
        
        _signingCredentials = new SigningCredentials
        (
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.IssuerSigningKey)),
            SecurityAlgorithms.HmacSha256
        );
    }

    public async Task<JsonWebToken> CreateTokenAsync
    (
        Guid userId,
        Guid clientId,
        string externalLoginProvider = null,
        IDictionary<string, IEnumerable<string>> claims = null,
        CancellationToken cancellationToken = default
    )
    {
        if (SequentialGuid.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));

        if (SequentialGuid.IsNullOrEmpty(clientId))
            throw new ArgumentNullException(nameof(clientId));
        
        User user = await _userManager.FindByIdAsync(userId.ToString());
        IList<string> roles = await _userManager.GetRolesAsync(user);
        
        Instant now = _clockService.Now;
        List<Claim> jwtClaims = new()
        {
            new Claim(ClaimTypes.Name, user.NormalizedUserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ApplicationClaimTypes.ClientIdentifier, clientId.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, SequentialGuid.Create().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeMilliseconds().ToString())
        };
        
        jwtClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        if (!string.IsNullOrEmpty(externalLoginProvider))
            jwtClaims.Add(new Claim(ClaimTypes.AuthenticationMethod, externalLoginProvider));

        if (claims?.Any() is true)
        {
            List<Claim> customClaims = new();
            foreach ((string claim, IEnumerable<string> values) in claims)
                customClaims.AddRange(values.Select(value => new Claim(claim, value)));

            jwtClaims.AddRange(customClaims);
        }

        Client client = await _clientStore.FindClientByKeyAsync(clientId, cancellationToken);
        Instant accessTokenExpires = now.Plus(Duration.FromMinutes(client.AccessTokenLifeTime));

        JwtSecurityToken jwt = new
        (
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: jwtClaims,
            notBefore: now.ToDateTimeUtc(),
            expires: accessTokenExpires.ToDateTimeUtc(),
            signingCredentials: _signingCredentials
        );
        
        string accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
        UserRefreshToken refreshToken = await SetNewRefreshTokenAsync(user, client, now, cancellationToken);

        return new JsonWebToken
        {
            AccessToken = accessToken,
            AccessTokenExpiry = accessTokenExpires.ToUnixTimeMilliseconds(),
            RefreshToken = refreshToken.Value,
            RefreshTokenExpiry = refreshToken.DateExpires.ToUnixTimeMilliseconds(),
            UserId = userId,
            Email = user.Email,
            Roles = roles,
            Claims = claims ?? EmptyClaims
        };
    }

    public async Task<JsonWebToken> RenewTokensAsync
    (
        string refreshToken,
        string accessToken,
        Guid clientId,
        CancellationToken cancellationToken = default
    )
    {
        (ClaimsPrincipal claimsPrincipal, JwtSecurityToken jwtToken) = await DecodeAccessTokenAsync(accessToken);

        if (jwtToken is null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            throw new SecurityTokenException("Invalid token.");
        
        UserRefreshToken dbRefreshToken = await _refreshTokenStore.FindRefreshTokenByValueAsync(refreshToken, cancellationToken);
        if (dbRefreshToken is null)
            throw new SecurityTokenException("Invalid token.");

        if (dbRefreshToken.ClientId != clientId)
            throw new SecurityTokenException("Invalid client.");

        string userName = claimsPrincipal.Identity?.Name;
        User user = await _userManager.FindByIdAsync(dbRefreshToken.UserId.ToString());

        if (user.NormalizedUserName != userName || dbRefreshToken.DateExpires < _clockService.Now)
            throw new SecurityTokenException("Invalid token.");

        // Retrieve provider information
        Claim authMethodClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthenticationMethod);
        string provider = authMethodClaim?.Value;

        return await CreateTokenAsync(user.Id, clientId, provider, null, cancellationToken);
    }

    public Task RemoveExpiredRefreshTokensAsync(CancellationToken cancellationToken = default)
        => _refreshTokenStore.RemoveExpiredRefreshTokensAsync(cancellationToken);

    private async Task<UserRefreshToken> SetNewRefreshTokenAsync
    (
        User user,
        Client client,
        Instant now,
        CancellationToken cancellationToken = default
    )
    {
        // Delete the existing refresh token from the database (if found).
        UserRefreshTokenKey userRefreshTokenKey = new() 
        { 
            ClientId = client.Id,
            UserId = user.Id
        };
        await _refreshTokenStore.RemoveRefreshTokenByKeyAsync(userRefreshTokenKey, cancellationToken);
        
        // Generate new refresh token.
        byte[] randomNumber = new byte[32];
        using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);

        string token = Convert.ToBase64String(randomNumber);
        
        // Set the expiration date.
        Instant expires = now.Plus(Duration.FromMinutes(client.RefreshTokenLifeTime));
        
        UserRefreshToken refreshToken = new()
        {
            UserId = user.Id,
            ClientId = client.Id,
            Value = token,
            DateExpires = expires
        };

        // Save refresh token to the database.
        await _refreshTokenStore.AddRefreshTokenAsync(refreshToken, cancellationToken);

        return refreshToken;
    }
    
    private Task<(ClaimsPrincipal, JwtSecurityToken)> DecodeAccessTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new SecurityTokenException("Invalid token.");

        ClaimsPrincipal claimsPrincipal = new JwtSecurityTokenHandler()
            .ValidateToken(token, _tokenValidationParameters, out SecurityToken validatedToken);

        return Task.FromResult((claimsPrincipal, validatedToken as JwtSecurityToken));
    }
}