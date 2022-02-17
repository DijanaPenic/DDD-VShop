using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Auth.Constants;

namespace VShop.SharedKernel.Infrastructure.Auth;

public static class AuthExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, params Module[] modules)
    {
        AuthOptions options = services.GetOptions<AuthOptions>("Auth");

        TokenValidationParameters tokenValidationParameters = new()
        {
            RequireAudience = options.RequireAudience,
            ValidIssuer = options.ValidIssuer,
            ValidIssuers = options.ValidIssuers,
            ValidateActor = options.ValidateActor,
            ValidAudience = options.ValidAudience,
            ValidAudiences = options.ValidAudiences,
            ValidateAudience = options.ValidateAudience,
            ValidateIssuer = options.ValidateIssuer,
            ValidateLifetime = options.ValidateLifetime,
            ValidateTokenReplay = options.ValidateTokenReplay,
            ValidateIssuerSigningKey = options.ValidateIssuerSigningKey,
            SaveSigninToken = options.SaveSigninToken,
            RequireExpirationTime = options.RequireExpirationTime,
            RequireSignedTokens = options.RequireSignedTokens,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        if (string.IsNullOrWhiteSpace(options.IssuerSigningKey))
            throw new ArgumentException("Missing issuer signing key.");

        byte[] rawKey = Encoding.UTF8.GetBytes(options.IssuerSigningKey);
        tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(rawKey);
        
        if (!string.IsNullOrWhiteSpace(options.AuthenticationType))
            tokenValidationParameters.AuthenticationType = options.AuthenticationType;

        if (!string.IsNullOrWhiteSpace(options.NameClaimType))
            tokenValidationParameters.NameClaimType = options.NameClaimType;

        if (!string.IsNullOrWhiteSpace(options.RoleClaimType))
            tokenValidationParameters.RoleClaimType = options.RoleClaimType;

        CookieOptions cookieOptions = new()
        {
            HttpOnly = options.Cookie.HttpOnly,
            Secure = options.Cookie.Secure,
            SameSite =  GetSameSiteMode(options)
        };
        
        void CookieAuthOptions(CookieAuthenticationOptions cookieAuthOptions)
        {
            cookieAuthOptions.Cookie.HttpOnly = options.Cookie.HttpOnly;
            cookieAuthOptions.Cookie.SameSite = GetSameSiteMode(options);
            cookieAuthOptions.ExpireTimeSpan = TimeSpan.FromMinutes(options.Cookie.ExpireTime);
            cookieAuthOptions.SlidingExpiration = options.Cookie.SlidingExpiration;

            // Use 401 status code instead of Login redirect which is used by default.
            cookieAuthOptions.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
        }
        
        services
            .AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.Authority = options.Authority;
                o.Audience = options.Audience;
                o.MetadataAddress = options.MetadataAddress;
                o.SaveToken = options.SaveToken;
                o.RefreshOnIssuerKeyNotFound = options.RefreshOnIssuerKeyNotFound;
                o.RequireHttpsMetadata = options.RequireHttpsMetadata;
                o.IncludeErrorDetails = options.IncludeErrorDetails;
                o.TokenValidationParameters = tokenValidationParameters;
                
                if (!string.IsNullOrWhiteSpace(options.Challenge))
                    o.Challenge = options.Challenge;
                
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // JWT token is sent in a different field than what is expected. This allows us to feed it in.
                        if (context.Request.Cookies.TryGetValue(ApplicationIdentityConstants.AccessTokenScheme, out string token))
                            context.Token = token;

                        return Task.CompletedTask;
                    },
                };
            })
            .AddCookie(ApplicationIdentityConstants.AccountVerificationScheme, CookieAuthOptions)
            .AddCookie(IdentityConstants.TwoFactorRememberMeScheme, CookieAuthOptions)
            .AddCookie(IdentityConstants.TwoFactorUserIdScheme, CookieAuthOptions)
            .AddCookie(IdentityConstants.ExternalScheme, CookieAuthOptions);

        services.AddSingleton(options);
        services.AddSingleton(cookieOptions);
        services.AddSingleton(tokenValidationParameters);

        IEnumerable<string> policies = modules.SelectMany(m => m.Policies ?? Enumerable.Empty<string>())
            .Select(p => p.ToLowerInvariant());

        services
            .AddAuthorization(authorization =>
            {
                foreach (string policy in policies)
                    authorization.AddPolicy(policy, b => b.RequireClaim("permission", policy));
            }).AddAuthorization(authorization =>
            {
                authorization.AddPolicy
                (
                    "ClientAuthentication",
                    new AuthorizationPolicyBuilder(ApplicationAuthSchemes.ClientAuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build()
                );
            });

        return services;
    }
    
    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.Use(async (ctx, next) =>
        {
            string authScheme = ctx.Request.Cookies.ContainsKey(ApplicationIdentityConstants.AccessTokenScheme) // Is user logged in?
                ? JwtBearerDefaults.AuthenticationScheme
                : ApplicationAuthSchemes.ClientAuthenticationScheme;

            AuthenticateResult authenticateResult = await ctx.AuthenticateAsync(authScheme);
            if (authenticateResult.Succeeded && authenticateResult.Principal is not null)
                ctx.User = authenticateResult.Principal;
        
            await next();
        });

        return app;
    }

    private static SameSiteMode GetSameSiteMode(AuthOptions options)
        => options.Cookie.SameSite?.ToLowerInvariant() switch
        {
            "strict" => SameSiteMode.Strict,
            "lax" => SameSiteMode.Lax,
            "none" => SameSiteMode.None,
            "unspecified" => SameSiteMode.Unspecified,
            _ => SameSiteMode.Unspecified
        };
}